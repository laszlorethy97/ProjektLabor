using KeyManagement.Api.Data;
using KeyManagement.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using KeyManagement.Api.DTO;


namespace KeyManagement.Api.Services
{
    public class KeyTransactionService
    {
        private readonly KeyManagementDbContext _context;

        public KeyTransactionService(KeyManagementDbContext context)
        {
            _context = context;
        }

        public async Task<KeyTransaction> RecordKeyCheckoutAsync(KeyTransactionDTO keyTransactionDTO, int startedByUserId)
        {
            var user = await FindUserAsync(keyTransactionDTO);
            var key = await FindKeyAsync(keyTransactionDTO.KeyCode);

            var startedByUser = await _context.Users.FindAsync(startedByUserId);
            if (startedByUser == null)
            {
                throw new KeyNotFoundException("Started by user not found.");
            }

            await EnsureKeyIsAvailableAsync(key);

            var reservation = await FindReservationAsync(user, key);

            EnsureCheckoutTimeIsValid(reservation);

            var keyTransaction = new KeyTransaction
            {
                Key = key,
                PinCode = keyTransactionDTO.PinCode,
                StartedAt = DateTime.UtcNow,
                StartedByUser = startedByUser,
                User = user
            };

            _context.KeyTransactions.Add(keyTransaction);
            await _context.SaveChangesAsync();
            
            return keyTransaction;
        }


        public async Task<KeyTransaction> RecordKeyReturnAsync(
            KeyTransactionDTO keyTransactionDTO,
            int endedByUserId)
        {
            var user = await FindUserAsync(keyTransactionDTO);
            var key = await FindKeyAsync(keyTransactionDTO.KeyCode);

            var keyTransaction = await _context.KeyTransactions
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.Key.Id == key.Id &&
                    t.EndedAt == null);

            if (keyTransaction == null)
            {
                throw new KeyNotFoundException(
                    "No open transaction found for this key.");
            }

            if (keyTransaction.User.Id != user.Id)
            {
                throw new InvalidOperationException(
                    "This key was checked out by a different user.");
            }

            var endedByUser = await _context.Users.FindAsync(endedByUserId);

            if (endedByUser == null)
            {
                throw new KeyNotFoundException(
                    "Ended by user not found.");
            }

            keyTransaction.EndedAt = DateTime.UtcNow;
            keyTransaction.EndedByUser = endedByUser;

            await _context.SaveChangesAsync();

            return keyTransaction;
        }

        private async Task<User> FindUserAsync(KeyTransactionDTO keyTransactionDTO)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == keyTransactionDTO.Email &&
                    u.PinCode == keyTransactionDTO.PinCode);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found or invalid pin code.");
            }

            return user;
        }

        private async Task<Key> FindKeyAsync(string keyCode)
        {
            var normalizedKeyCode = keyCode.Trim();
            var key = await _context.Keys
                .FirstOrDefaultAsync(k => k.Code == normalizedKeyCode);

            if (key == null)
            {
                throw new KeyNotFoundException("Key not found.");
            }

            return key;
        }

        private async Task EnsureKeyIsAvailableAsync(Key key)
        {
            bool isCheckedOut = await _context.KeyTransactions
                .AnyAsync(t =>
                    t.Key.Id == key.Id &&
                    t.EndedAt == null);

            if (isCheckedOut)
            {
                throw new InvalidOperationException("Key is already checked out.");
            }
        }

        private async Task<Reservation> FindReservationAsync(User user, Key key)
        {
            var reservation = await _context.Reservations
                .Where(r =>
                    r.Room.Keys.Contains(key) &&
                    r.User.Id == user.Id &&
                    r.EndTime > DateTime.UtcNow)
                .OrderBy(r => r.StartTime)
                .FirstOrDefaultAsync();

            if (reservation == null)
            {
                throw new KeyNotFoundException(
                    "No valid reservation found for this key and user.");
            }

            return reservation;
        }

        private static void EnsureCheckoutTimeIsValid(Reservation reservation)
        {
            var now = DateTime.UtcNow;

            bool isWithinCheckoutTime =
                reservation.StartTime.AddMinutes(-10) <= now &&
                now <= reservation.EndTime;

            if (!isWithinCheckoutTime)
            {
                throw new InvalidOperationException(
                    "Key can only be checked out within 10 minutes before the reservation start time or during the reservation.");
            }
        }


    }
}