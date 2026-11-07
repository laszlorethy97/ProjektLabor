namespace KeyManagement.Api.DTO
{
    public class RegisterUserDTO
    {
        public string Email { get; set; }
        public string PinCode { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
    }
}

