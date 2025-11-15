using System;
using System.ComponentModel.DataAnnotations;
//using UserManagement.Models;

namespace UserManagement.Web.Models.Users;

public class UserListViewModel
{
    public List<UserListItemViewModel> Items { get; set; } = new();
}

public class UserListItemViewModel
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Forename is required")]
    public string Forename { get; set; } = string.Empty;

    [Required(ErrorMessage = "Surname is required")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        ErrorMessage = "Email must be in format: example@domain.com")]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    [CustomValidation(typeof(UserListItemViewModel), nameof(ValidateDOB))]
    public DateTime? DateOfBirth { get; set; }


    // -------------------------------
    // CUSTOM VALIDATOR FOR DOB
    // -------------------------------
    public static ValidationResult? ValidateDOB(DateTime? dob, ValidationContext context)
    {
        //if empty -> OK
        if (dob is null)
            return ValidationResult.Success;

        var today = DateTime.Today;

        // Cannot be in the future
        if (dob > today)
            return new ValidationResult("Date of birth cannot be in the future.");

        // Must be at least 18 years old
        var eighteenYearsAgo = today.AddYears(-18);
        if (dob > eighteenYearsAgo)
            return new ValidationResult("User must be at least 18 years old.");

        return ValidationResult.Success;
    }

}


