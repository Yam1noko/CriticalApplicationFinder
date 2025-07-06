using backend.Data;
using backend.Models.External;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class EFExternalRequestRepository : IExternalRequestRepository
    {
        private readonly ExternalDbContext _context;

        public EFExternalRequestRepository(ExternalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExternalRequest>> GetAllAsync()
        {
            return await _context.Requests
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();
        }

        public async Task<ExternalRequest?> GetByIdAsync(string id)
        {
            return await _context.Requests.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<ExternalRequest>> GetInRangeAsync(DateTime from, DateTime to)
        {
            var fromUtc = DateTime.SpecifyKind(from, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(to, DateTimeKind.Utc);

            return await _context.Requests
                .Where(r => r.CreationDate >= fromUtc && r.CreationDate <= toUtc)
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();
        }
    }
}
