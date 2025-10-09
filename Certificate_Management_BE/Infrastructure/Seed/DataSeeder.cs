using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(Context db)
        {
            await db.Database.MigrateAsync();

            var utcNow = DateTime.UtcNow;

            // Roles
            var roles = new[]
            {
                new Role { RoleId = 1, RoleName = "Administrator" },
                new Role { RoleId = 2, RoleName = "Instructor" },
                new Role { RoleId = 4, RoleName = "Trainee" },
                new Role { RoleId = 5, RoleName = "Education Officer" },
                new Role { RoleId = 6, RoleName = "Director" },
                new Role { RoleId = 7, RoleName = "AOC Manager" }
            };
            foreach (var r in roles)
            {
                if (!await db.Roles.AnyAsync(x => x.RoleId == r.RoleId))
                {
                    db.Roles.Add(r);
                }
            }

            // Specialties
            var specialties = new[]
            {             
               new Specialty { SpecialtyId = "SPL-PS", SpecialtyName = "Passenger service", Description = "Passenger service specialty", CreatedAt = utcNow, UpdatedAt = utcNow },
                new Specialty { SpecialtyId = "SPL-RA", SpecialtyName = "Ramp agent", Description = "Ramp agent specialty", CreatedAt = utcNow, UpdatedAt = utcNow },
                new Specialty { SpecialtyId = "SPL-DOC", SpecialtyName = "Document Staff", Description = "Document staff specialty", CreatedAt = utcNow, UpdatedAt = utcNow },
                new Specialty { SpecialtyId = "SPL-CC", SpecialtyName = "Cabin Crew", Description = "Cabin crew specialty", CreatedAt = utcNow, UpdatedAt = utcNow },
                new Specialty { SpecialtyId = "SPL-TAM", SpecialtyName = "Technical Aircraft Maintenance", Description = "Technical aircraft maintenance specialty", CreatedAt = utcNow, UpdatedAt = utcNow },
                new Specialty { SpecialtyId = "SPL-FC", SpecialtyName = "Flight Crew", Description = "Flight crew specialty", CreatedAt = utcNow, UpdatedAt = utcNow }
            };
            foreach (var s in specialties)
            {
                if (!await db.Specialties.AnyAsync(x => x.SpecialtyId == s.SpecialtyId))
                {
                    db.Specialties.Add(s);
                }
            }

            // Users
            var users = new[]
            {
                new User { UserId = "USR-ADMIN", Username = "admin", FullName = "Administrator", Sex = Sex.Male, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "admin@example.com", CitizenId = "000000000000", RoleId = 1, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-INST", Username = "instructor", FullName = "Default Instructor", Sex = Sex.Male, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "instructor@example.com", CitizenId = "111111111111", RoleId = 2, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                // Trainees for each specialty
                new User { UserId = "USR-TRN-PS", Username = "trainee.ps", FullName = "Trainee - Passenger service", Sex = Sex.Female, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "trainee.ps@example.com", CitizenId = "222222222205", RoleId = 4, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-TRN-RA", Username = "trainee.ramp", FullName = "Trainee - Ramp Agent", Sex = Sex.Male, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "trainee.ramp@example.com", CitizenId = "222222222206", RoleId = 4, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-TRN-DOC", Username = "trainee.doc", FullName = "Trainee - Document Staff", Sex = Sex.Female, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "trainee.doc@example.com", CitizenId = "222222222201", RoleId = 4, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-TRN-CC",  Username = "trainee.cabin", FullName = "Trainee - Cabin Crew", Sex = Sex.Female, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "trainee.cabin@example.com", CitizenId = "222222222202", RoleId = 4, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-TRN-TAM", Username = "trainee.tech", FullName = "Trainee - Technical AM", Sex = Sex.Male, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "trainee.tech@example.com", CitizenId = "222222222203", RoleId = 4, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-TRN-FC",  Username = "trainee.flight", FullName = "Trainee - Flight Crew", Sex = Sex.Male, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "trainee.flight@example.com", CitizenId = "222222222204", RoleId = 4, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-EDU", Username = "education", FullName = "Education Officer", Sex = Sex.Male, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "education@example.com", CitizenId = "333333333333", RoleId = 5, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-DIR", Username = "director", FullName = "Director", Sex = Sex.Male, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "director@example.com", CitizenId = "444444444444", RoleId = 6, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow },
                new User { UserId = "USR-AOC", Username = "aocmanager", FullName = "AOC Manager", Sex = Sex.Female, DateOfBirth = new DateOnly(1995,3,1), PasswordHash = "$2a$11$p/7olMKkz3Dk20j7Nn3cV.MTSTPEa.i7ZUy8kqSyX1L3ZulR9U/t2", Email = "aocmanager@example.com", CitizenId = "555555555555", RoleId = 7, Status = AccountStatus.Active, CreatedAt = utcNow, UpdatedAt = utcNow }
            };

            foreach (var u in users)
            {
                if (!await db.Users.AnyAsync(x => x.UserId == u.UserId))
                {
                    db.Users.Add(u);
                }
            }

            // UserSpecialties - only for trainees
            var traineeMap = new (string userId, string specialtyId)[]
            {
                ("USR-TRN-PS", "SPL-PS"),
                ("USR-TRN-RA", "SPL-RA"),
                ("USR-TRN-DOC", "SPL-DOC"),
                ("USR-TRN-CC",  "SPL-CC"),
                ("USR-TRN-TAM", "SPL-TAM"),
                ("USR-TRN-FC",  "SPL-FC")
            };
            foreach (var (userId, specialtyId) in traineeMap)
            {
                if (!await db.Set<UserSpecialty>().AnyAsync(x => x.UserId == userId && x.SpecialtyId == specialtyId))
                {
                    db.Set<UserSpecialty>().Add(new UserSpecialty
                    {
                        UserId = userId,
                        SpecialtyId = specialtyId,
                        CreatedAt = utcNow
                    });
                }
            }

            await db.SaveChangesAsync();
        }
    }
}


