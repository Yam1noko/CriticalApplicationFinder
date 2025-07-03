using backend.Data;
using backend.Models.Internal;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class EFRequestRepository : IRequestRepository
{
    private readonly InternalDbContext _context;

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
        await _context.SaveChangesAsync();
    }

    public async Task Remove(Request request)
    {
        _context.Requests.Remove(request);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Request request)
    {
        _context.Requests.Update(request);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(int id)
    {
        return await _context.Requests.AnyAsync(r => r.Id == id.ToString());
    }
}
