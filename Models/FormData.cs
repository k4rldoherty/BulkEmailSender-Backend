using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace BulkEmailSender.Models
{
    public class FormData
    {
        public string From { get; set; }
        public string Password { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public IFormFile PhotoFooter { get; set; }
        public IFormFile CsvFile { get; set; }
    }
}
