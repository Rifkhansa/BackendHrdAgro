namespace BackendHrdAgro.Services.Email
{
    public class MailSettings
    {
        public string? Mail { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Password { get; set; }
        public string Host { get; set; } = null!;
        public int? Port { get; set; }
        public bool IsPlain { get; set; }
    }
}
