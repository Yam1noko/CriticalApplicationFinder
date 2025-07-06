using backend.Models.External;
using backend.Models.Internal;

namespace backend.Repositories
{
    public interface IRequestRepository
    {
        Task<IEnumerable<Request>> GetAllAsync();
        Task<Request?> GetByIdAsync(string id);
        Task<List<Request>> GetByDateRange(DateTime from, DateTime to);
        Task Add(Request request);
        Task Remove(Request request);
        Task Update(Request request);
        Task<bool> Exists(Request request);
        Task SaveChangesAsync();
    }
}

