using BulkEmailSender.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using Microsoft.VisualBasic.FileIO;

namespace BulkEmailSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult SendEmail(EmailDTO emailInfo)
        {
            _emailService.SendEmail(emailInfo);
            return Ok();
        }


        [HttpPost("processformdata")]
        public IActionResult ProcessData([FromForm] FormData data)
        {
            // The CSV File
            IFormFile csvFile = data.CsvFile;
            // Photo Footer
            IFormFile photoFooter = data.PhotoFooter;

            // for each entry in CSV file
            // construct the email using the custom name section
            // construct the emailDTO object
            // send the email.

            using (TextFieldParser parser = new TextFieldParser(csvFile.OpenReadStream()))
            {
                parser.Delimiters = new string[] { "," };

                if(!parser.EndOfData)
                {
                    parser.ReadLine();
                }

                while(!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    string email = fields[0];
                    string companyName = fields[1];

                    EmailDTO emailDTO = new EmailDTO();
                    emailDTO.From = data.From;
                    emailDTO.Password = data.Password;
                    emailDTO.To = email;
                    emailDTO.Subject = data.Subject;
                    
                    // Deal with the body.
                    string modifiedEmailBody = data.Body.Replace("{}", companyName);
                    emailDTO.Body = modifiedEmailBody;

                    // Include the photoFooter in the HTML body as an image
                    if (photoFooter != null && photoFooter.Length > 0)
                    {
                        using (Stream photoStream = photoFooter.OpenReadStream())
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            // Copy the photoFooter stream to a MemoryStream
                            photoStream.CopyTo(memoryStream);

                            // Convert the MemoryStream to a byte array
                            byte[] imageBytes = memoryStream.ToArray();

                            // Convert the byte array to base64
                            string base64Image = Convert.ToBase64String(imageBytes);

                            // Create the image source data URI
                            string imageSrc = $"data:image/jpeg;base64,{base64Image}";

                            // Append image tag to the HTML body
                            modifiedEmailBody += $"<br/><br/><img src='{imageSrc}' alt='Footer Image' />";
                        }
                    }

                    emailDTO.Body = modifiedEmailBody;

                    // Send the emails. 
                    SendEmail(emailDTO);
                }
            }

                return Ok();
        }
    }
}
