using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models;

public class ActivityLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// When the action happened (stored in UTC).
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Short action label e.g. "User Created", "User Updated", "User Deleted".
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// The affected user (may be null for non-user actions later).
    /// </summary>
    public long? UserId { get; set; }

    /// <summary>
    /// Cached full name of the affected user at the time of the action.
    /// </summary>
    [MaxLength(200)]
    public string? UserName { get; set; }

    /// <summary>
    /// Who performed the action (for this test we'll just store a string).
    /// </summary>
    [MaxLength(200)]
    public string? PerformedBy { get; set; }

    /// <summary>
    /// Longer description of what happened.
    /// </summary>
    [MaxLength(1000)]
    public string? Details { get; set; }
}
