using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IUserService> _userService = null!;
    private UsersController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _userService = new Mock<IUserService>();
        _controller = new UsersController(_userService.Object);
    }

    [Test]
    public void List_ShowAll_UsesGetAllAndReturnsAll()
    {
        var all = StubUsers(true, false, true); // 3 users
        _userService.Setup(s => s.GetAll()).Returns(all);

        var result = _controller.List(null);

        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().HaveCount(all.Length); // ✅ expect 3
    }


    [Test]
    public void List_WithIsActiveTrue_ShouldUseFilterAndReturnOnlyActiveUsers()
    {
        // Arrange (Active Only)
        var activeUsers = StubUsers(true);
        SetupFilterByActive(true, activeUsers);

        // Act
        var result = _controller.List(isActive: true);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().HaveCount(1)
            .And.OnlyContain(i => i.IsActive);
    }

    [Test]
    public void List_WithIsActiveFalse_ShouldUseFilterAndReturnOnlyInactiveUsers()
    {
        // Arrange (Non Active)
        var inactives = StubUsers(false);
        SetupFilterByActive(false, inactives);

        // Act
        var result = _controller.List(isActive: false);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().HaveCount(1)
            .And.OnlyContain(i => !i.IsActive);
    }

    // ---------- helpers ----------

    private static User[] StubUsers(params bool[] actives)
    {
        var list = new List<User>();
        for (int i = 0; i < actives.Length; i++)
        {
            list.Add(new User
            {
                Id = i + 1,
                Forename = "User",
                Surname = "Test",
                Email = $"user{i + 1}@example.com",
                IsActive = actives[i]
            });
        }
        return list.ToArray();
    }

    private void SetupGetAll(User[] users)
        => _userService.Setup(s => s.GetAll()).Returns(users);

    private void SetupFilterByActive(bool isActive, User[] users)
        => _userService.Setup(s => s.FilterByActive(isActive)).Returns(users);
}

