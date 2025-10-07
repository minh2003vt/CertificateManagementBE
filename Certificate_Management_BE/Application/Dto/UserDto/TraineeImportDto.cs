using Domain.Enums;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application.Dto.UserDto
{
    public class TraineeImportDto
    {
        public int? RowNumber { get; set; }
        public string? UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string GenderStr { get; set; } = string.Empty;
        public string DateOfBirthStr { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CitizenId { get; set; } = string.Empty;
        public string SpecialtyId { get; set; } = string.Empty;

        // Parsed values
        public Sex Gender { get; private set; }
        public DateOnly DateOfBirth { get; private set; }

        public ValidationErrorsDto Validate(System.Collections.Generic.IEnumerable<Domain.Entities.User> existingUsers, 
                                           System.Collections.Generic.IEnumerable<Domain.Entities.Specialty> specialties)
        {
            var errors = new ValidationErrorsDto();

            // Full name
            if (string.IsNullOrEmpty(FullName))
                errors.Add("Full name is required");

            // Email
            if (string.IsNullOrEmpty(Email) || !IsValidEmail(Email))
                errors.Add("Invalid email format");
            else if (existingUsers.Any(u => u.Email == Email))
                errors.Add($"Email '{Email}' already exists");

            // Phone number
            if (string.IsNullOrEmpty(PhoneNumber) || !Regex.IsMatch(PhoneNumber, @"^\d{10}$"))
                errors.Add("Phone number must be 10 digits");

            // Citizen ID
            if (string.IsNullOrEmpty(CitizenId) || !Regex.IsMatch(CitizenId, @"^\d{12}$"))
                errors.Add("Citizen ID must be 12 digits");
            else if (existingUsers.Any(u => u.CitizenId == CitizenId))
                errors.Add($"Citizen ID '{CitizenId}' already exists");

            // Specialty
            if (string.IsNullOrEmpty(SpecialtyId) || !specialties.Any(s => s.SpecialtyId == SpecialtyId))
                errors.Add($"Specialty '{SpecialtyId}' not found in database");

            // Gender
            if (string.IsNullOrEmpty(GenderStr) ||
                (!GenderStr.Equals("Male", StringComparison.OrdinalIgnoreCase) &&
                 !GenderStr.Equals("Female", StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add("Gender must be 'Male' or 'Female'");
            }
            else
            {
                Gender = GenderStr.Equals("Male", StringComparison.OrdinalIgnoreCase) ? Sex.Male : Sex.Female;
            }

            // Date of Birth
            if (string.IsNullOrEmpty(DateOfBirthStr) || !DateOnly.TryParse(DateOfBirthStr, out var dob))
            {
                errors.Add("Invalid date of birth format");
            }
            else
            {
                DateOfBirth = dob;
                var age = DateTime.Now.Year - dob.Year;
                if (DateTime.Now.DayOfYear < dob.DayOfYear) age--;

                if (age < 18)
                    errors.Add("Trainee must be at least 18 years old");
            }

            return errors;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

