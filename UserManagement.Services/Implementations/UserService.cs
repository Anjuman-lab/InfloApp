using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

/// <summary>
/// Implements user-related business logic.
/// </summary>
public class UserService : IUserService
{
    private readonly IDataContext _context;
    private readonly IActivityLogService _activityLogService;

    public UserService(IDataContext context, IActivityLogService activityLogService)
    {
        _context = context;
        _activityLogService = activityLogService;
    }

    /// <inheritdoc />
    public async Task<List<User>> GetAllAsync(bool? isActive = null)
    {
        IQueryable<User> query = _context.Users.AsNoTracking();

        // Optional filter by active state.
        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        return await query
            .OrderBy(u => u.Surname)
            .ThenBy(u => u.Forename)
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

   
    public async Task<User> CreateAsync(User user)
    {
        // Basic business rule example: new users default to active.
        if (!user.IsActive)
            user.IsActive = true;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        //Log activity
        await _activityLogService.AddAsync(new ActivityLog
        {
            Action = "User Created",
            UserId = user.Id,
            UserName = $"{user.Forename} {user.Surname}",
            PerformedBy = "System", // later you could plug in real username
            Details = $"Created user account for {user.Forename} {user.Surname}"
        });

        return user;
    }

    
    public async Task<bool> UpdateAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null)
        {
            return false;
        }

        // Map allowed fields from input to existing entity.
        existing.Forename = user.Forename;
        existing.Surname = user.Surname;
        existing.Email = user.Email;
        existing.IsActive = user.IsActive;
        existing.DateOfBirth = user.DateOfBirth;

        await _context.SaveChangesAsync();

        //Log activity 
        await _activityLogService.AddAsync(new ActivityLog
        {
            Action = "User Updated",
            UserId = existing.Id,
            UserName = $"{existing.Forename} {existing.Surname}",
            PerformedBy = "System",
            Details = $"Updated user account for {existing.Forename} {existing.Surname}"
        });

        return true;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        // FIX → Add this
        var fullName = $"{user.Forename} {user.Surname}";

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        await _activityLogService.AddAsync(new ActivityLog
        {
            Action = "User Deleted",
            UserId = id,
            UserName = fullName,
            PerformedBy = "System",
            Details = $"Deleted user account for {fullName}"
        });

        return true;
    }

}

