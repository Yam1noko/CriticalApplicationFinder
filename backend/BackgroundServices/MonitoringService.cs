using AutoMapper;
using backend.Email;
using backend.Models.External;
using backend.Models.Internal;
using backend.Options;
using backend.Repositories;
using backend.Services;
using Microsoft.Extensions.Options;

namespace backend.BackgroundServices
{
    public class MonitoringService : BackgroundService
    {
        private readonly ILogger<MonitoringService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly MonitoringOptions _options;
        public MonitoringService(
            IServiceProvider serviceProvider,
            ILogger<MonitoringService> logger,
            IOptions<MonitoringOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MonitoringService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var internalRepo = scope.ServiceProvider.GetRequiredService<IRequestRepository>();
                    var externalRepo = scope.ServiceProvider.GetRequiredService<IExternalRequestRepository>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    var ruleService = scope.ServiceProvider.GetRequiredService<IRuleService>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    await CheckAndSync(internalRepo, externalRepo, mapper, ruleService, notificationService);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in MonitoringService");
                }

                await Task.Delay(TimeSpan.FromMinutes(_options.IntervalMinutes), stoppingToken);
            }

            _logger.LogInformation("MonitoringService stopped");
        }

        private async Task CheckAndSync(
            IRequestRepository internalRepo,
            IExternalRequestRepository externalRepo,
            IMapper mapper, IRuleService ruleService, INotificationService notificationService)
        {
            var external = (await externalRepo.GetAllAsync()).ToList();
            var internalList = (await internalRepo.GetAllAsync()).ToList();

            var internalById = internalList.ToDictionary(x => x.Id);
            var hasChanges = false;

            foreach (var externalReq in external)
            {
                if (!internalById.TryGetValue(externalReq.Id, out var internalReq))
                {
                    var newInternal = mapper.Map<Request>(externalReq);
                    newInternal = await ruleService.IsRequestCritical(newInternal);
                    if (newInternal.isCritical == true)
                    {
                        await notificationService.SendEmail(newInternal);
                    }
                    await internalRepo.Add(newInternal);
                    hasChanges = true;
                }
                else if (!IsSame(internalReq, externalReq))
                {
                    mapper.Map(externalReq, internalReq);
                    await internalRepo.Update(internalReq);
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await internalRepo.SaveChangesAsync();
                _logger.LogInformation("Changes synced at {time}", DateTime.UtcNow);
            }
            else
            {
                _logger.LogInformation("No changes detected at {time}", DateTime.UtcNow);
            }
        }

        private bool IsSame(Request internalReq, ExternalRequest externalReq)
        {
            return internalReq.Id == externalReq.Id &&
                   internalReq.ServiceId == externalReq.ServiceId &&
                   internalReq.Title == externalReq.Title &&
                   internalReq.CreationDate == externalReq.CreationDate &&
                   internalReq.ClientName == externalReq.ClientName &&
                   internalReq.ShortDescr == externalReq.ShortDescr &&
                   internalReq.DescriptionRtf4096 == externalReq.DescriptionRtf4096;
        }
    }
}

