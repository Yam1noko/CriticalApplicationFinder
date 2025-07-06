namespace backend.Services
{
    using backend.DataTransferObject;

    public interface IRequestService
    {
        Task<IEnumerable<RequestDto>> GetRequestsInRange(DateTime from, DateTime to);
        Task<bool> ManualCheck();
    }

}
