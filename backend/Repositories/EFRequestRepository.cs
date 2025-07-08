using backend.Data;
using backend.Models.External;
using backend.Models.Internal;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class EFRequestRepository : IRequestRepository
    {
        private readonly InternalDbContext _context;

        public async Task<IEnumerable<Request>> GetAllAsync()
        {
            return await _context.Requests
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();
        }
        public async Task<Request?> GetByIdAsync(string id)
        {
            return await _context.Requests.FirstOrDefaultAsync(r => r.Id == id);
        }
        public EFRequestRepository(InternalDbContext context)
        {
            _context = context;
        }

        public async Task<List<Request>> GetByDateRange(DateTime from, DateTime to)
        {
            from = EnsureUtc(from);
            to = EnsureUtc(to);

            return await _context.Requests
                .Where(r => r.CreationDate >= from && r.CreationDate <= to)
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();
        }

        private DateTime EnsureUtc(DateTime dt)
        {
            return dt.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(dt, DateTimeKind.Utc)
                : dt.ToUniversalTime();
        }

        public async Task Add(Request request)
        {
            _context.Requests.Add(request);
            await Task.CompletedTask;
        }

        public async Task Remove(Request request)
        {
            _context.Requests.Remove(request);
            await Task.CompletedTask;
        }

        public async Task Update(Request request)
        {
            _context.Requests.Update(request);
            await Task.CompletedTask;
        }

        public async Task<bool> Exists(Request request)
        {
            return await _context.Requests.AnyAsync(r =>
            r.Id == request.Id &&
            r.ServiceId == request.ServiceId &&
            r.Title == request.Title &&
            r.CreationDate == request.CreationDate &&
            r.ClientName == request.ClientName &&
            r.ShortDescr == request.ShortDescr &&
            r.DescriptionRtf4096 == request.DescriptionRtf4096
            );
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}