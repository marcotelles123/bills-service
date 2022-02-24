using Bills.Service.Common.VO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bills.Service.Email.Infra
{
    public interface IEmailSMTP
    {
        bool SendEmail(string subject, string body, string fromEmail, string fromPassword, string toEmail);

        IList<EmailVO> GetEmails(string login, string password, string subject);
    }
}
