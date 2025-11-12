using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive">true => only active users, false => only inactive users</param>
    /// <returns>IEnumerable of users matching isActive</returns>
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>()
            .Where(u => u.IsActive == isActive)
            .ToList();
    }

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();


    /// <summary>
    /// Return all users (asynchronous)
    /// </summary>
    public async Task<List<User>> GetAllAsync(CancellationToken ct = default)
    {
        // Simulate async for now (since IDataContext.GetAll is synchronous)
        return await Task.FromResult(_dataAccess.GetAll<User>().ToList());
    }

    /// <summary>
    /// Return users by active state (asynchronous)
    /// </summary>
    public async Task<List<User>> FilterByActiveAsync(bool isActive, CancellationToken ct = default)
    {
        var result = _dataAccess.GetAll<User>()
            .Where(u => u.IsActive == isActive)
            .ToList();

        // Simulate async until we connect a real SQL DB
        return await Task.FromResult(result);
    }
}
