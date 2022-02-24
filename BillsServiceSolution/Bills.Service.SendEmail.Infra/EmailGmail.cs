using Bills.Service.Common.VO;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace Bills.Service.Email.Infra
{
    public class EmailGmail : IEmailSMTP
    {
        public IList<EmailVO> GetEmails(string login, string password, string subject)
        {
            var messages = new List<EmailVO>();

            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(login, password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                var results = inbox.Search(SearchOptions.All, SearchQuery.SubjectContains(subject));
                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(new EmailVO { From = message.From.ToString(), Subject = message.Subject, Body = message.Body.ToString(), ReceivedDate = message.Date.DateTime});

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }


            return messages;
        }

        public bool SendEmail(string subject, string body, string fromEmail, string fromPassword, string toEmail)
        {
            var fromAddress = new MailAddress(fromEmail, "Contas Vencendo");
            var toAddress = new MailAddress(toEmail, "Tonhus");

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }

            return true;
        }

    }
}
