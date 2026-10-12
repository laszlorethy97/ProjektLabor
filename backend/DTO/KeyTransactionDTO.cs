namespace KeyManagement.Api.DTO
{
    public class KeyTransactionDTO
    {
        public string Email { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public int KeyId { get; set; }
    }
}