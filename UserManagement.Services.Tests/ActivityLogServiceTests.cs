using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests
{
    [TestFixture]
    public class ActivityLogServiceTests
    {
        private DataContext _context = null!;
        private ActivityLogService _service = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"ActivityLogDb_{Guid.NewGuid()}")
                .Options;

            _context = new DataContext(options);
            _service = new ActivityLogService(_context);
        }

        // ------------------------------------------------------
        // ADD
        // ------------------------------------------------------
        [Test]
        public async Task AddAsync_WhenValid_ShouldAddLogAndSetTimestampIfMissing()
        {
            // Arrange
            var log = new ActivityLog
            {
                Action = "Created",
                UserId = 1,
                UserName = "John Doe",
                PerformedBy = "System",
                Details = "Created user account",
                Timestamp = default
            };

            // Act
            await _service.AddAsync(log);

            // Assert
            var saved = _context.ActivityLogs.FirstOrDefault();
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved!.Timestamp, Is.Not.EqualTo(default(DateTime)));
            Assert.That(saved.Action, Is.EqualTo("Created"));
        }

        [Test]
        public async Task AddAsync_WhenTimestampProvided_ShouldPreserveIt()
        {
            // Arrange
            var customTime = new DateTime(2025, 11, 17, 12, 0, 0, DateTimeKind.Utc);
            var log = new ActivityLog
            {
                Action = "Updated",
                UserId = 2,
                UserName = "Jane Doe",
                PerformedBy = "System",
                Details = "Updated user profile",
                Timestamp = customTime
            };

            // Act
            await _service.AddAsync(log);

            // Assert
            var saved = _context.ActivityLogs.First();
            Assert.That(saved.Timestamp, Is.EqualTo(customTime));
        }

        // ------------------------------------------------------
        // GET ALL
        // ------------------------------------------------------
        [Test]
        public async Task GetAllAsync_ShouldReturnAllLogsOrderedByDescendingTimestamp()
        {
            // Arrange
            _context.ActivityLogs.AddRange(new[]
            {
                new ActivityLog { Action = "A", Timestamp = DateTime.UtcNow.AddMinutes(-10) },
                new ActivityLog { Action = "B", Timestamp = DateTime.UtcNow.AddMinutes(-5) },
                new ActivityLog { Action = "C", Timestamp = DateTime.UtcNow }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.First().Action, Is.EqualTo("C")); // most recent first
        }

        // ------------------------------------------------------
        // GET BY USER
        // ------------------------------------------------------
        [Test]
        public async Task GetByUserAsync_ShouldReturnOnlyMatchingUserLogs()
        {
            // Arrange
            _context.ActivityLogs.AddRange(new[]
            {
                new ActivityLog { Action = "A", UserId = 1, Timestamp = DateTime.UtcNow },
                new ActivityLog { Action = "B", UserId = 2, Timestamp = DateTime.UtcNow }
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByUserAsync(1);

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.All(l => l.UserId == 1), Is.True);
        }

        // ------------------------------------------------------
        // GET BY ID
        // ------------------------------------------------------
        [Test]
        public async Task GetByIdAsync_WhenExists_ShouldReturnLog()
        {
            // Arrange
            var log = new ActivityLog { Action = "Test", Timestamp = DateTime.UtcNow };
            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(log.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Action, Is.EqualTo("Test"));
        }

        [Test]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
