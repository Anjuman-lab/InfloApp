using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IActivityLogService
{
    Task AddAsync(ActivityLog log);

    Task<List<ActivityLog>> GetAllAsync();

    Task<List<ActivityLog>> GetByUserAsync(long userId);

    Task<ActivityLog?> GetByIdAsync(Guid id);
}
