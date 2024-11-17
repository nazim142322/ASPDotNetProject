//using EmpReManagement.Repository.Interface;
//using EmpReManagement.ViewModel;
//using System.Net;
//using System.Net.Mail;

//namespace EmpReManagement.Repository.Service
//{
//    public class EmailSender : IEmailSender
//    {
//        //to get application setting in appsettings.json
//        private readonly IConfiguration configuration;        
//        public EmailSender(IConfiguration configuration)
//        {
//            this.configuration = configuration;
//        }
//        public async Task<bool> EmailSendAsync(string email, string Subject, string message)
//        {
//            bool status = false;
//            try
//            {
//                //Retrieving email settings from appsettings.json.
//                //var getEmailSetting = new GetEmailSetting()
//                //{
//                //    SecretKey = configuration.GetValue<string>("AppSettings:SecretKey"),
//                //    From = configuration.GetValue<string>("AppSettings:EmailSettings:From"),
//                //    SmtpServer = configuration.GetValue<string>("AppSettings:EmailSettings:Smtp"),
//                //    Port = configuration.GetValue<int>("AppSettings:EmailSettings:port"),
//                //    EnableSSL = configuration.GetValue<bool>("AppSettings:EmailSettings:EnableSSL")
//                //};
//                //Creating the Email Message
//                MailMessage mailMessage = new MailMessage()
//                {
//                    From = new MailAddress(getEmailSetting.From),
//                    Subject = Subject,
//                    Body = message
//                };
//                //to
//                mailMessage.To.Add(email);

//                //Setting Up the SMTP Client
//                SmtpClient smtpClient = new SmtpClient(getEmailSetting.SmtpServer)
//                {
//                    Port = getEmailSetting.Port,
//                    Credentials = new NetworkCredential(getEmailSetting.From, getEmailSetting.SecretKey),
//                    EnableSsl = getEmailSetting.EnableSSL
//                };

//                //sends the email asynchronously
//                await smtpClient.SendMailAsync(mailMessage);
//                status =true;
//            }
//            catch (Exception ex)
//            {
//                status = false;
//            }
//            return status;
//        }
//    }
//}
