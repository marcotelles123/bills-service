using Bills.Service.RefreshPaids.Infra;
using Serilog;
using System;

namespace Bills.Service.RefreshPaids.Domain
{
    public class RefreshPaidsImpl : IRefreshPaids
    {
        private ILogger _logger;
        private IRefreshPaidsRepository _refreshPaidsRepository;
        public RefreshPaidsImpl(ILogger logger, IRefreshPaidsRepository refreshPaidsRepository)
        {
            _logger = logger;
            _refreshPaidsRepository = refreshPaidsRepository;
        }
        public bool Execute()
        {
            try
            {
                _logger.Debug($"Iniciando verificação de contas a serem ativadas");
                var results = _refreshPaidsRepository.GetAllResults();

                var currentMonth = DateTime.Now.Month;

                foreach (var bill in results) {
                    if (bill.paid) {
                        if (bill.lastPaidMonth < currentMonth)
                        {
                            _logger.Debug($"Marcando conta : {bill.bill} como ativa");
                            _refreshPaidsRepository.SetAsUnPaidAsync(bill._id);
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
    }
}
