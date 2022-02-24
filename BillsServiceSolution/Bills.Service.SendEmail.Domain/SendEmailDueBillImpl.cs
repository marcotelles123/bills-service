using Bills.Service.RefreshPaids.Infra;
using System;
using Serilog;
using Bills.Service.Email.Infra;
using Microsoft.Extensions.Configuration;
using Bills.Service.Util;
using Bills.Service.CheckReceipts.Domain;
using Bills.Service.Common.VO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Bills.Service.SendEmail.Domain
{
    public class SendEmailDueBillImpl : ISendEmailDueBill
    {
        private ILogger _logger;
        private IRefreshPaidsRepository _refreshPaidsRepository;
        private IEmailSMTP _sendEmailDueBill;
        private String _fromEmail;
        private String _fromPassword;
        private String _toEmail;
        private ICheckReceipts _checkReceipts;
        public SendEmailDueBillImpl(ILogger logger, IRefreshPaidsRepository refreshPaidsRepository, IEmailSMTP sendEmailDueBill, ICheckReceipts checkReceipts, IConfiguration configuration)
        {
            _logger = logger;
            _refreshPaidsRepository = refreshPaidsRepository;
            _sendEmailDueBill = sendEmailDueBill;
            _fromEmail = configuration["Email:FromEmail"];
            _fromPassword = configuration["Email:FromPassword"];
            _toEmail = configuration["Email:ToEmail"];
            _checkReceipts = checkReceipts;
        }

        public bool Execute()
        {
            try
            {
                _logger.Debug($"Iniciando verificação de contas vencidas");
                var results = _refreshPaidsRepository.GetAllResults();

                var currentMonth = DateTime.Now.Month;
                var currentDay = DateTime.Now.Day;
                var receipts = _checkReceipts.GetAllReceipts().ToList().FindAll(r => r.ReceivedDate > DateTime.Now.AddDays(-15));
                foreach (var bill in results)
                {
                    if (!bill.paid)
                    {
                        if (bill.lastPaidMonth <= currentMonth && bill.dueDate >= currentDay)
                        {
                            var haveReceipt = "";
                            if (HaveReceipt(receipts, bill.bill)) {
                                haveReceipt = " mas tem comprovante recente, confira!!!";
                            }

                            _logger.Debug($"Marcando conta : {bill.bill} como ativa");
                            _sendEmailDueBill.SendEmail($"Conta {bill.bill} venceu/vence dia {bill.dueDate}" + haveReceipt, "Pague esta merda", _fromEmail, Security.DecryptString(_fromPassword), _toEmail);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"ERRO: Ao atualizar conta: {e.Message}");
                throw e;
            }
        }

        private bool HaveReceipt(IList<EmailVO> receipts, string billName)
        {
            _logger.Debug($"Verificando se existe comprovante.");
            foreach (var receipt in receipts)
            {
                if (string.Compare(billName, receipt.Subject.Replace("Comprovante ", ""), CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) > 0)
                    return true;
            }
             
            _logger.Debug($"Comprovante para a conta {billName} não encontrada.");
            return false;
        }
    }
}
