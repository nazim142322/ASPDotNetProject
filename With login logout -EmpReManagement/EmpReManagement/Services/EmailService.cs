using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Mail;

namespace EmpReManagement.Services
{
    public class EmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }
        public async Task<bool> SendEmailAsync(string Name, string Email, string Phone, string Message)
        {
            bool status = false;
            try
            {
                //Retrieving email settings from appsettings.json.
                var smtpServer = config["EmailSettings:SmtpServer"]; // return string
                var smtpPort = config.GetValue<int>("EmailSettings:SmtpPort");
                var senderEmail = config["EmailSettings:SenderEmail"];
                var senderPass = config.GetValue<string>("EmailSettings:SenderPassword");
                var enableSSL = config.GetValue<bool>("EmailSettings:EnableSSL");

                //setting up smtp client for sending email
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, senderPass),
                    EnableSsl = enableSSL
                };
                //create email message
                var mailMessage = new MailMessage()
                {
                    From = new MailAddress("servicedoon@gmail.com"),
                    Subject = "New form enquiry from employee management system",
                    Body= $"Name : {Name}\n Email Address : {Email}\n Phone No : {Phone}\n Message : {Message}",
                    IsBodyHtml = false,//set true to enable HTML content
                };

                //set the Reply-To Address
                mailMessage.ReplyToList.Add(new MailAddress(Email));

                // add recipient address
                mailMessage.To.Add("nazim142322@gmail.com");

                //send email
                await smtpClient.SendMailAsync(mailMessage);
                status = true;

            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;

        }
    }
}
