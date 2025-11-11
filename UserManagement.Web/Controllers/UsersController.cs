using System.Linq;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;


    /// <summary>
    /// Optional query param "isActive" filters active/inactive users.
    /// </summary>
    /// <param name="isActive">If null => show all, otherwise filter by active state</param>
    /// <returns>List of the users.</returns>
    [HttpGet]
    public ViewResult List([FromQuery] bool? isActive)
    {
        var users = isActive.HasValue
        ? _userService.FilterByActive(isActive.Value)
        : _userService.GetAll();

        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }
}
