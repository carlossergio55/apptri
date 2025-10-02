using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Features.Integracion.Commands.ViajeC;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aplicacion.Features.Integracion.Background
{
   
    public class DailyTripGeneratorHostedService : BackgroundService
    {
        private readonly ILogger<DailyTripGeneratorHostedService> _logger;
        private readonly IServiceProvider _provider;   // ⬅️ usamos IServiceProvider para crear scopes
        private readonly TripGenerationOptions _options;

        public DailyTripGeneratorHostedService(
            ILogger<DailyTripGeneratorHostedService> logger,
            IServiceProvider provider,
            IOptions<TripGenerationOptions> options)
        {
            _logger = logger;
            _provider = provider;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "DailyTripGenerator iniciado. Hora programada: {Time} (local), DaysAhead={Days}",
                _options.RunAtLocalTime, _options.DaysAhead);

            // (Opcional pero útil) una corrida al inicio para ponerse al día.
            try { await EjecutarGeneracionConScope(stoppingToken); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo en corrida inicial de DailyTripGenerator (continuará).");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var delay = CalcularDelay(_options.RunAtLocalTime, DateTime.Now);
                    _logger.LogInformation("Próxima generación en {Delay}", delay);
                    await Task.Delay(delay, stoppingToken);

                    await EjecutarGeneracionConScope(stoppingToken);
                }
                catch (TaskCanceledException) { /* apagando */ }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en DailyTripGenerator; reintento en 1 minuto.");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }

        /// <summary>
        /// Crea un scope DI por ejecución para resolver IMediator y cualquier servicio scoped (DbContext).
        /// </summary>
        private async Task EjecutarGeneracionConScope(CancellationToken ct)
        {
            using var scope = _provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var cmd = new GenerarViajesProximosCommand
            {
                Desde = DateTime.Today,
                Dias = _options.DaysAhead
            };

            var res = await mediator.Send(cmd, ct);
            _logger.LogInformation("Generación diaria completada. {Msg}", res.Message);
        }

        private static TimeSpan CalcularDelay(string hhmm, DateTime ahoraLocal)
        {
            // ✔️ Formato correcto para verbatim string: @"hh\:mm"
            if (!TimeSpan.TryParseExact(hhmm, @"hh\:mm", CultureInfo.InvariantCulture, out var target))
                target = new TimeSpan(2, 0, 0); // fallback 02:00

            var proxima = ahoraLocal.Date + target;
            if (proxima <= ahoraLocal) proxima = proxima.AddDays(1);
            return proxima - ahoraLocal;
        }

    }
}
