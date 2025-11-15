using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using UserManagement.Models;
using UserManagement.Web.Models.Users;

namespace UserManagement.Web.Pages.Users
{
    public partial class UsersList
    {
        private string filter = "all";
        private List<User> all = new();
        private List<User> filtered = new();
        private string MaxDob => DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private UserListItemViewModel newUser = new UserListItemViewModel
        {
            IsActive = true
        };

        private string CurrentFilterLabel =>
            filter switch
            {
                "active" => "Active Users",
                "inactive" => "Inactive Users",
                _ => "All Users"
            };

        protected override async Task OnInitializedAsync()
        {
            all = await UserService.GetAllAsync();
            ApplyFilter();
        }

        private void SetFilter(string value)
        {
            filter = value;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            filtered = filter switch
            {
                "active" => all.Where(u => u.IsActive).ToList(),
                "inactive" => all.Where(u => !u.IsActive).ToList(),
                _ => all.ToList()
            };
        }

        private void SearchUsers(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ApplyFilter();
                return;
            }

            text = text.ToLower();

            filtered = all.Where(u =>
                (u.Forename?.ToLower().Contains(text) ?? false) ||
                (u.Surname?.ToLower().Contains(text) ?? false) ||
                (u.Email?.ToLower().Contains(text) ?? false)
            ).ToList();
        }


        private async Task SaveNewUser()
        {
            var user = new User
            {
                Forename = newUser.Forename,
                Surname = newUser.Surname,
                Email = newUser.Email,
                IsActive = newUser.IsActive,
                DateOfBirth = newUser.DateOfBirth
            };

            await UserService.CreateAsync(user);

            await JS.InvokeVoidAsync("hideModal", "addUserModal");

            all = await UserService.GetAllAsync();
            ApplyFilter();

            newUser = new UserListItemViewModel { IsActive = true };
        }

        private async Task SaveEditedUser()
        {
            await UserService.UpdateAsync(editModel!);

            showEditModal = false;

            all = await UserService.GetAllAsync();
            ApplyFilter();
        }


        private bool showEditModal = false;
        private User? editModel;

        private async Task GoToEdit(long id)
        {
            editModel = await UserService.GetByIdAsync(id);
            showEditModal = true;
        }


        private bool showDeleteModal;
        private long deleteUserId;
        private string? selectedUserName;

        private void GoToDelete(long id, string fullName)
        {
            deleteUserId = id;
            selectedUserName = fullName;
            showDeleteModal = true;
        }

        private void CancelDelete()
        {
            showDeleteModal = false;
        }

        private async Task ConfirmDelete()
        {
            showDeleteModal = false;

            await UserService.DeleteAsync(deleteUserId);

            // refresh the grid
            all = await UserService.GetAllAsync();
            ApplyFilter();
        }


        private bool showViewModal = false;
        private User? selectedUser;

        private async Task GoToView(long id)
        {
            selectedUser = await UserService.GetByIdAsync(id);
            showViewModal = true;
        }
        private string GetInitials(User? user)
        {
            if (user is null) return string.Empty;

            var first = string.IsNullOrWhiteSpace(user.Forename)
                ? ""
                : user.Forename.Trim()[0].ToString().ToUpperInvariant();

            var last = string.IsNullOrWhiteSpace(user.Surname)
                ? ""
                : user.Surname.Trim()[0].ToString().ToUpperInvariant();

            return (first + last).Trim();
        }


    }
}
