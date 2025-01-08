using System;
using System.Net;
using System.Net.Mail;

namespace EmailServiceManager
{
    public class EmailService
    {
        private readonly string smtpHost = "smtp.gmail.com"; // Gmail SMTP server
        private readonly int smtpPort = 587; // Port for TLS
        private readonly string senderEmail = "therahaptics@gmail.com"; // Your Gmail
        private readonly string senderPassword = "fqbx blss fdqi wdkl"; // Replace with the app password

        public void SendEmail(string recipientEmail, string subject, string body)
        {
            try
            {
                Console.WriteLine($"Attempting to send email to: {recipientEmail}");

                using (SmtpClient smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                    smtpClient.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, "TheraHaptics"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(recipientEmail);

                    smtpClient.Send(mailMessage);

                    Console.WriteLine($"Email sent successfully to: {recipientEmail}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw; // Re-throw the exception for visibility
            }
        }

    }
}
