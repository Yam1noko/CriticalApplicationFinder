namespace backend.Services
{
    using AutoMapper;
    using backend.DataTransferObject;

    using backend.Models.External;
    using backend.Models.Internal;
    using backend.Repositories;


    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _internalRepo;
        private readonly IExternalRequestRepository _externalRepo;
        private readonly IMapper _mapper;

        public RequestService(IRequestRepository repo, IExternalRequestRepository externalRepo, IMapper mapper)
        {
            _internalRepo = repo;
            _externalRepo = externalRepo;
            _mapper = mapper;
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
                    //Вызываем кусок кода Германа с этим newInternal
                    //Если критический, то вслед за Приколами Германа, мутим приколы Дениса
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
