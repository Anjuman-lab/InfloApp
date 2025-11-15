using System;
using System.Linq;
using UserManagement.Models;


namespace UserManagement.Data
{
    public static class DbSeeder
    {
        public static void Seed(DataContext db)
        {
            // Seed Users
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User
                    {
                        Forename = "Haslam",
                        Surname = "Balasan",
                        Email = "haslam.balasan@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1992, 5, 14)
                    },
                    new User
                    {
                        Forename = "Lorrain",
                        Surname = "Mcdoughall",
                        Email = "lorrain.mcdoughall@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1988, 3, 21)
                    },
                    new User
                    {
                        Forename = "Alex",
                        Surname = "Johnson",
                        Email = "alex.johnson@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1990, 11, 2)
                    },
                    new User
                    {
                        Forename = "Priya",
                        Surname = "Sharma",
                        Email = "priya.sharma@example.com",
                        IsActive = false,
                        DateOfBirth = new DateTime(1995, 1, 30)
                    },
                    new User
                    {
                        Forename = "David",
                        Surname = "Nguyen",
                        Email = "david.nguyen@example.com",
                        IsActive = true,
                        DateOfBirth = new DateTime(1985, 7, 9)
                    }
                );

                db.SaveChanges();
            }
        }
    }
}
