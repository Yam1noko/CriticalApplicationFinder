using backend.Models.Internal;

namespace backend.Repositories;

public interface IRequestRepository
{
    Task<List<Request>> GetByDateRange(DateTime from, DateTime to);
    Task Add(Request request);
    Task Remove(Request request);
    Task Update(Request request);
    Task<bool> Exists(int id);
}
