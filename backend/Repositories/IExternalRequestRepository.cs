using backend.Models.External;

namespace backend.Repositories
{
    public interface IExternalRequestRepository
    {
        Task<IEnumerable<ExternalRequest>> GetAllAsync();
        Task<ExternalRequest?> GetByIdAsync(string id);
        Task<IEnumerable<ExternalRequest>> GetInRangeAsync(DateTime from, DateTime to);
    }
}
