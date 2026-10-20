using KeyManagement.Api.Data;
using KeyManagement.Api.DTO;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeyManagement.Api.Services
{
    public class MasterKeyTransactionService
    {
        private readonly KeyManagementDbContext _context;

        public MasterKeyTransactionService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<MasterKeyTransaction> RecordMasterKeyCheckoutAsync(
            MasterKeyTransactionDTO dto,
            int startedByUserId)
        {
            var user = await FindUserAsync(dto);
            var masterKey = await FindMasterKeyAsync(dto.MasterKeyId);

            var startedByUser = await _context.Users.FindAsync(startedByUserId);
            if (startedByUser == null)
            {
                throw new KeyNotFoundException("Started by user not found.");
            }

            await EnsureMasterKeyIsAvailableAsync(masterKey);
            await EnsureUserHasPermissionAsync(user, masterKey);

            var transaction = new MasterKeyTransaction
            {
                MasterKey = masterKey,
                PinCode = dto.PinCode,
                StartedAt = DateTime.UtcNow,
                StartedByUser = startedByUser,
                ApprovedByUser = startedByUser,
                User = user
            };

            _context.MasterKeyTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<MasterKeyTransaction> RecordMasterKeyReturnAsync(
            MasterKeyTransactionDTO dto,
            int endedByUserId)
        {
            var user = await FindUserAsync(dto);
            var masterKey = await FindMasterKeyAsync(dto.MasterKeyId);

            var transaction = await FindOpenMasterKeyTransactionAsync(masterKey);

            if (transaction.User.Id != user.Id)
            {
                throw new InvalidOperationException(
                    "This master key was checked out by a different user.");
            }

            var endedByUser = await _context.Users.FindAsync(endedByUserId);
            if (endedByUser == null)
            {
                throw new KeyNotFoundException("Ended by user not found.");
            }

            transaction.EndedAt = DateTime.UtcNow;
            transaction.EndedByUser = endedByUser;

            await _context.SaveChangesAsync();

            return transaction;
        }

        private async Task<User> FindUserAsync(MasterKeyTransactionDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == dto.Email &&
                    u.PinCode == dto.PinCode);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found or invalid pin code.");
            }

            return user;
        }

        private async Task<MasterKey> FindMasterKeyAsync(int masterKeyId)
        {
            var masterKey = await _context.MasterKeys
                .FirstOrDefaultAsync(mk => mk.Id == masterKeyId);

            if (masterKey == null)
            {
                throw new KeyNotFoundException("Master key not found.");
            }

            return masterKey;
        }

        private async Task EnsureMasterKeyIsAvailableAsync(MasterKey masterKey)
        {
            bool isCheckedOut = await _context.MasterKeyTransactions
                .AnyAsync(t =>
                    t.MasterKey.Id == masterKey.Id &&
                    t.EndedAt == null);

            if (isCheckedOut)
            {
                throw new InvalidOperationException(
                    "Master key is already checked out.");
            }
        }

        private async Task EnsureUserHasPermissionAsync(
            User user,
            MasterKey masterKey)
        {
            bool hasPermission = await _context.Permissions
                .AnyAsync(p =>
                    p.User.Id == user.Id &&
                    p.MasterKey.Id == masterKey.Id);

            if (!hasPermission)
            {
                throw new InvalidOperationException(
                    "User does not have permission to use this master key.");
            }
        }

        private async Task<MasterKeyTransaction> FindOpenMasterKeyTransactionAsync(
            MasterKey masterKey)
        {
            var transaction = await _context.MasterKeyTransactions
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.MasterKey.Id == masterKey.Id &&
                    t.EndedAt == null);

            if (transaction == null)
            {
                throw new KeyNotFoundException(
                    "No open transaction found for this master key.");
            }

            return transaction;
        }
    }
}
