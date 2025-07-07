using backend.Data;
using backend.Models.Internal;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class EFNotificationRepository : INotificationRepository
{
    private readonly InternalDbContext _context;

    public EFNotificationRepository(InternalDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationEmail>> GetAllEmails()
    {
        return await _context.NotificationEmails.ToListAsync();
    }

    public async Task AddEmail(NotificationEmail email)
    {
        _context.NotificationEmails.Add(email);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveEmail(NotificationEmail email)
    {
        _context.NotificationEmails.Remove(email);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistEmail(string email)
    {
        return await _context.NotificationEmails.AnyAsync(r => r.Address == email);
    }

    public async Task<int> FindId(string email)
    {
        if (email == "max") {
            var maxId = await _context.NotificationEmails.MaxAsync(x => (int?)x.Id) ?? 0;
            return maxId;
        }
        else
        {
            return await _context.NotificationEmails.Where(e => e.Address == email)
        .Select(e => e.Id)
        .FirstOrDefaultAsync(); 
        }
    }

    public async Task<NotificationTemplate?> GetTemplate()
    {
        return await _context.NotificationTemplates.FirstOrDefaultAsync() ?? new NotificationTemplate { Id = 1, Template = "Пусто"};
    }

    public async Task AddTemplate(NotificationTemplate template)
    {
        _context.NotificationTemplates.Add(template);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTemplate(NotificationTemplate template)
    {
        _context.NotificationTemplates.Update(template);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistTemplate()
    {
        return await _context.NotificationTemplates.AsNoTracking().AnyAsync();
    }
}
