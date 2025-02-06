namespace BackendHrdAgro.Services.Email
{
    public interface IMailService
    {
        Task SendEmailAsync(Mailer mailer);
    }
    public class MailCode
    {
        public string Marketing = "M001";
        public string Claim = "M002";
        public string Accounting = "M003";
    }

    public class MailStructure
    {
        public string Header { get; set; } = null!;
        public string Opener { get; set; } = null!;
        public string? Closer { get; set; }
        public Dictionary<string, string>? Data { get; set; }
    }
}
