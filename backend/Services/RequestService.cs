namespace backend.Services
{
    using AutoMapper;
    using backend.BackgroundServices;
    using backend.DataTransferObject;

    using backend.Models.External;
    using backend.Models.Internal;
    using backend.Repositories;


    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _internalRepo;
        private readonly IExternalRequestRepository _externalRepo;
        private readonly IMapper _mapper;
        private readonly IRuleService _ruleService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<RequestService> _logger;

        public RequestService(IRequestRepository repo, IExternalRequestRepository externalRepo, IMapper mapper, IRuleService ruleService, INotificationService notificationService, ILogger<RequestService> logger)
        {
            _internalRepo = repo;
            _externalRepo = externalRepo;
            _mapper = mapper;
            _ruleService = ruleService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<IEnumerable<RequestDto>> GetRequestsInRange(DateTime from, DateTime to)
        {
            var requests = await _internalRepo.GetByDateRange(from, to);

            return requests.Select(r => new RequestDto
            {
                Id = r.Id,
                CreationDate = r.CreationDate,
                ClientName = r.ClientName,
                ShortDescr = r.ShortDescr,
                DescriptionRtf4096 = r.DescriptionRtf4096,
                IsCritical = r.isCritical
            });
        }

        public async Task<bool> ManualCheck()
        {
            var external = (await _externalRepo.GetAllAsync()).ToList();
            var internalList = (await _internalRepo.GetAllAsync()).ToList();

            var internalById = internalList.ToDictionary(x => x.Id);

            var hasChanges = false;

            foreach (var externalReq in external)
            {
                if (!internalById.TryGetValue(externalReq.Id, out var internalReq))
                {
                    var newInternal = _mapper.Map<Models.Internal.Request>(externalReq);
                    newInternal = await _ruleService.IsRequestCritical(newInternal);
                    if (newInternal.isCritical == true)
                    {
                        try
                        {
                            await _notificationService.SendEmail(newInternal);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Ошибка при отправке email, выполнение продолжается");
                        }
                    }
                    await _internalRepo.Add(newInternal);
                    hasChanges = true;
                }
                else
                {

                    if (!IsSame(internalReq, externalReq))
                    {
                        _mapper.Map(externalReq, internalReq); 
                        await _internalRepo.Update(internalReq);
                        hasChanges = true;
                    }
                }
            }

            if (hasChanges)
            {
                await _internalRepo.SaveChangesAsync();
            }

            return hasChanges;
        }

        private bool IsSame(Request internalReq, ExternalRequest externalReq)
        {
            return
                internalReq.Id == externalReq.Id &&
                internalReq.ServiceId == externalReq.ServiceId &&
                internalReq.Title == externalReq.Title &&
                internalReq.ClientName == externalReq.ClientName &&
                internalReq.ShortDescr == externalReq.ShortDescr &&
                internalReq.DescriptionRtf4096 == externalReq.DescriptionRtf4096 &&
                internalReq.CreationDate == externalReq.CreationDate;
        }
    }

}
