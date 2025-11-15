using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class ActivityLogService : IActivityLogService
{
    private readonly IDataContext _context;

    public ActivityLogService(IDataContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ActivityLog log)
    {
        if (log.Timestamp == default)
        {
            log.Timestamp = DateTime.UtcNow;
        }

        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public Task<List<ActivityLog>> GetAllAsync()
        => _context.ActivityLogs
            .AsNoTracking()
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();

    public Task<List<ActivityLog>> GetByUserAsync(long userId)
        => _context.ActivityLogs
            .AsNoTracking()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync();

    public Task<ActivityLog?> GetByIdAsync(Guid id)
        => _context.ActivityLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);
}
