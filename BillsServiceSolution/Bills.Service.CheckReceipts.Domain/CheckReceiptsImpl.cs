using Bills.Service.Common.VO;
using Bills.Service.Email.Infra;
using Bills.Service.Util;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;

namespace Bills.Service.CheckReceipts.Domain
{
    public class CheckReceiptsImpl : ICheckReceipts
    {
        private ILogger _logger;
        private IEmailSMTP _emailSMTP;
        private String _receiptsDatabasePassword;
        private String _receiptsDatabaseEmail;
        public CheckReceiptsImpl(ILogger logger, IEmailSMTP emailSMTP, IConfiguration configuration) {
            _logger = logger;
            _emailSMTP = emailSMTP;
            _receiptsDatabaseEmail = configuration["Email:ReceiptsDatabaseEmail"];
            _receiptsDatabasePassword = configuration["Email:ReceiptsDatabasePassword"];
        }
        public IList<EmailVO> GetAllReceipts()
        {
            try
            {
                _logger.Debug($"Iniciando verificação de comprovantes");

                var emailWithReceiptSubject = _emailSMTP.GetEmails(_receiptsDatabaseEmail, Security.DecryptString(_receiptsDatabasePassword), "Comprovante");
               

                return emailWithReceiptSubject;
            }
            catch (Exception e)
            {
                _logger.Error($"ERRO: Ao verificar comprovantes: {e.Message}");
                throw e;
            }
        }
    }
}
