using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    IEnumerable<User> FilterByActive(bool isActive);
    IEnumerable<User> GetAll();


    //New async methods (preferred going forward)
    Task<List<User>> GetAllAsync(CancellationToken ct = default);
    Task<List<User>> FilterByActiveAsync(bool isActive, CancellationToken ct = default);
}
