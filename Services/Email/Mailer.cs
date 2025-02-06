namespace BackendHrdAgro.Services.Email
{
    public class Mailer
    {
        public string ToEmail { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public List<string?>? CC { get; set; }
        public List<IFormFile?>? Attachment { get; set; }
        public string? directoryLocation { get; set; }
    }

    public class MailUplaod
    {
        public string BatchId { get; set; } = null!;
        public string BatchName { get; set; } = null!;
        public string? UploadFileId { get; set; }
        public string? UplaodFileName { get; set; }
        public int? TotError { get; set; }
        public int? TotSpk { get; set; }
        public string EmailTo { get; set; } = null!;
        public List<IFormFile>? Attachment { get; set; }
    }

}
