using Bills.Service.CheckReceipts.Domain;
using Bills.Service.RefreshPaids.Domain;
using Bills.Service.SendEmail.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bills.Service.Host
{
    public class Worker : BackgroundService
    {
        private readonly ILogger _logger;
        private bool _running;
        private IRefreshPaids _refreshPaids;
        private ISendEmailDueBill _sendEmailDueBill;
        private ICheckReceipts _checkReceipts;
        private int _loopInHours;
        public Worker(ILogger logger, IRefreshPaids refreshPaids, ISendEmailDueBill sendEmailDueBill, ICheckReceipts checkReceipts,  IConfiguration configuration)
        {
            _logger = logger;
            _refreshPaids = refreshPaids;
            _sendEmailDueBill = sendEmailDueBill;
            _checkReceipts = checkReceipts;
            _loopInHours = Convert.ToInt32(configuration["LoopInHours"]) < 1 ? 1 : Convert.ToInt32(configuration["LoopInHours"]);
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _running = true;
            return base.StartAsync(cancellationToken);
        }
        public override void Dispose()
        {
            base.Dispose();
            _running = false;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _running = false;
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (_running)
            {
                try
                {
                    _logger.Information("Iniciando o processamento de Contas ");
                    _refreshPaids.Execute();
                    _sendEmailDueBill.Execute();
                    _logger.Information($"Finalizando o processamento de Atualização de Contas Pagas, proximo ciclo em {_loopInHours} horas");
                    await Task.Delay(TimeSpan.FromHours(_loopInHours), stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.Debug($"ERRO FATAL na de Contas: erro: {e.Message} ST: {e.StackTrace}");
                }
            }
            _logger.Information("Finalizando o processamento de Contas");

        }
    }
}
