using Application.IRepositories;
using Application.IServices;
using Application.ServiceResponse;
using Application.Dto.UserDto;
using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly INotificationService _notificationService;

        public UserService(
            IUnitOfWork unitOfWork, 
            ICloudinaryService cloudinaryService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _notificationService = notificationService;
        }

        #region GetProfile
        public async Task<ServiceResponse<UserProfileDto>> GetUserProfileAsync(string userId)
        {
            var response = new ServiceResponse<UserProfileDto>();
            try
            {
                var user = await _unitOfWork.UserRepository.GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                var profile = new UserProfileDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Sex = user.Sex,
                    DateOfBirth = user.DateOfBirth,
                    CitizenId = user.CitizenId,
                };

                response.Success = true;
                response.Data = profile;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to retrieve user profile.";
                response.Message = ex.Message;
            }

            return response;
        }
        #endregion

        #region ImportTrainees
        public async Task<ServiceResponse<ImportResultDto>> ImportTraineesAsync(IFormFile file, string performedByUsername)
        {
            var response = new ServiceResponse<ImportResultDto>();
            var result = new ImportResultDto();

            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                if (file == null || file.Length == 0)
                {
                    response.Success = false;
                    response.Message = "File is empty or not provided";
                    return response;
                }

                // Get all existing data for validation (needed for checking duplicates)
                var existingUsers = (await _unitOfWork.UserRepository.GetAll()).ToList();
                var specialties = await _unitOfWork.SpecialtyRepository.GetAll();
                var traineeRoleId = 4; // Trainee role

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var package = new ExcelPackage(stream))
                    {
                        // Validate workbook has at least 2 sheets
                        if (package.Workbook.Worksheets.Count < 2)
                        {
                            response.Success = false;
                            response.Message = "Excel file must have at least 2 sheets: Trainees and ExternalCertificates";
                            return response;
                        }

                        var traineeSheet = package.Workbook.Worksheets[0]; 
                        var certSheet = package.Workbook.Worksheets[1]; 

                        // Get total rows with data (excluding header)
                        if (traineeSheet.Dimension == null)
                        {
                            response.Success = false;
                            response.Message = "Trainee sheet is empty";
                            return response;
                        }

                        // Count actual rows with data (skip empty rows)
                        var traineeDataRowCount = 0;
                        for (int row = 2; row <= traineeSheet.Dimension.Rows; row++)
                        {
                            var fullName = traineeSheet.Cells[row, 2].Text?.Trim();
                            if (!string.IsNullOrEmpty(fullName))
                            {
                                traineeDataRowCount++;
                            }
                        }
                        
                        // Limit to maximum 20 records
                        if (traineeDataRowCount > 20)
                        {
                            response.Success = false;
                            response.Message = "Maximum 20 trainees allowed per import. Your file contains " + traineeDataRowCount + " records.";
                            return response;
                        }
                        
                        result.TraineeData.TotalRows = traineeDataRowCount;

                        // Store CitizenIDs from trainee sheet for certificate validation
                        var traineesCitizenIds = new HashSet<string>();
                        
                        // Store certificates grouped by CitizenId for later mapping
                        var certificatesByCitizenId = new Dictionary<string, List<ExternalCertificateData>>();
                        
                        // Store successfully created/updated users with their CitizenId
                        var processedUsersByCitizenId = new Dictionary<string, string>(); // CitizenId -> UserId
                        
                        // Read images from worksheet.Drawings and map to rows
                        var imagesByRow = new Dictionary<int, MemoryStream>();
                        foreach (var drawing in certSheet.Drawings)
                        {
                            if (drawing is OfficeOpenXml.Drawing.ExcelPicture pic)
                            {
                                // Get row number where image is located (0-based, so add 1)
                                var imageRow = pic.From.Row + 1;
                                
                                var imageBytes = pic.Image.ImageBytes;
                                
                                var imageStream = new MemoryStream(imageBytes);
                                imageStream.Position = 0;
                                
                                imagesByRow[imageRow] = imageStream;
                            }
                        }

                        // Count and read External Certificates sheet
                        var certDataRowCount = 0;
                        if (certSheet.Dimension != null)
                        {
                            for (int row = 2; row <= certSheet.Dimension.Rows; row++)
                            {
                                var citizenId = certSheet.Cells[row, 1].Text?.Trim();
                                if (string.IsNullOrEmpty(citizenId)) continue;

                                certDataRowCount++;

                                var certData = new ExternalCertificateData
                                {
                                    RowNumber = row,
                                    CitizenId = citizenId,
                                    CertificateCode = certSheet.Cells[row, 2].Text?.Trim() ?? "",
                                    CertificateName = certSheet.Cells[row, 3].Text?.Trim() ?? "",
                                    IssuingOrganization = certSheet.Cells[row, 4].Text?.Trim() ?? "",
                                    IssueDate = certSheet.Cells[row, 5].Text?.Trim() ?? "",
                                    ExpiredDate = certSheet.Cells[row, 6].Text?.Trim() ?? "",
                                    ImageStream = imagesByRow.ContainsKey(row) ? imagesByRow[row] : null
                                };

                                if (!certificatesByCitizenId.ContainsKey(citizenId))
                                {
                                    certificatesByCitizenId[citizenId] = new List<ExternalCertificateData>();
                                }
                                certificatesByCitizenId[citizenId].Add(certData);
                            }
                        }
                        result.ExternalCertificateData.TotalRows = certDataRowCount;

                        // Process Trainees
                        for (int row = 2; row <= traineeSheet.Dimension.Rows; row++)
                        {
                            try
                            {
                                // Read data from Excel into DTO
                                var traineeDto = new TraineeImportDto
                                {
                                    RowNumber = row,
                                    UserId = traineeSheet.Cells[row, 1].Text?.Trim(),
                                    FullName = traineeSheet.Cells[row, 2].Text?.Trim() ?? "",
                                    GenderStr = traineeSheet.Cells[row, 3].Text?.Trim() ?? "",
                                    DateOfBirthStr = traineeSheet.Cells[row, 4].Text?.Trim() ?? "",
                                    Address = traineeSheet.Cells[row, 5].Text?.Trim(),
                                    Email = traineeSheet.Cells[row, 6].Text?.Trim() ?? "",
                                    PhoneNumber = traineeSheet.Cells[row, 7].Text?.Trim() ?? "",
                                    CitizenId = traineeSheet.Cells[row, 8].Text?.Trim() ?? "",
                                    SpecialtyId = traineeSheet.Cells[row, 9].Text?.Trim() ?? ""
                                };
                                
                                // Skip empty rows
                                if (string.IsNullOrEmpty(traineeDto.FullName))
                                {
                                    continue;
                                }

                                // Add CitizenId to trainee list for certificate validation later
                                if (!string.IsNullOrEmpty(traineeDto.CitizenId))
                                {
                                    traineesCitizenIds.Add(traineeDto.CitizenId);
                                }

                                // Check if UserId is provided
                                bool isExistingUser = !string.IsNullOrEmpty(traineeDto.UserId);
                                User? existingUser = null;

                                if (isExistingUser)
                                {
                                    // Parse DateOfBirth and Gender first (needed for comparison)
                                    DateOnly parsedDateOfBirth = default;
                                    if (!DateOnly.TryParse(traineeDto.DateOfBirthStr, out parsedDateOfBirth))
                                    {
                                        result.TraineeData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = row,
                                            Reason = $"Invalid DateOfBirth format: '{traineeDto.DateOfBirthStr}'",
                                            FullName = traineeDto.FullName,
                                            Email = traineeDto.Email,
                                            CitizenId = traineeDto.CitizenId
                                        });
                                        result.TraineeData.FailureCount++;
                                        continue;
                                    }

                                    // User ID provided - validate it exists and info matches
                                    existingUser = existingUsers.FirstOrDefault(u => u.UserId == traineeDto.UserId);
                                    
                                    if (existingUser == null)
                                    {
                                        result.TraineeData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = row,
                                            Reason = $"User ID '{traineeDto.UserId}' not found in database",
                                            FullName = traineeDto.FullName,
                                            Email = traineeDto.Email,
                                            CitizenId = traineeDto.CitizenId
                                        });
                                        result.TraineeData.FailureCount++;
                                        continue;
                                    }

                                    // Validate info matches
                                    var infoMismatch = new List<string>();
                                    
                                    if (existingUser.FullName != traineeDto.FullName)
                                        infoMismatch.Add($"FullName mismatch (DB: '{existingUser.FullName}', Excel: '{traineeDto.FullName}')");
                                    
                                    if (existingUser.CitizenId != traineeDto.CitizenId)
                                        infoMismatch.Add($"CitizenId mismatch (DB: '{existingUser.CitizenId}', Excel: '{traineeDto.CitizenId}')");
                                    
                                    if (existingUser.DateOfBirth != parsedDateOfBirth)
                                        infoMismatch.Add($"DateOfBirth mismatch (DB: '{existingUser.DateOfBirth:yyyy-MM-dd}', Excel: '{parsedDateOfBirth:yyyy-MM-dd}')");
                                    
                                    if (infoMismatch.Any())
                                    {
                                        result.TraineeData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = row,
                                            Reason = $"User info mismatch: {string.Join("; ", infoMismatch)}",
                                            FullName = traineeDto.FullName,
                                            Email = traineeDto.Email,
                                            CitizenId = traineeDto.CitizenId
                                        });
                                        result.TraineeData.FailureCount++;
                                        continue;
                                    }

                                    // Check if UserSpecialty already exists
                                    var context = _unitOfWork.Context as DbContext;
                                    if (context != null)
                                    {
                                        var existingUserSpecialty = await context.Set<UserSpecialty>()
                                            .FirstOrDefaultAsync(us => us.UserId == traineeDto.UserId && us.SpecialtyId == traineeDto.SpecialtyId);
                                        
                                        if (existingUserSpecialty != null)
                                        {
                                            result.TraineeData.Errors.Add(new ImportErrorDto
                                            {
                                                RowNumber = row,
                                                Reason = $"User '{existingUser.FullName}' is already enrolled in this specialty",
                                                FullName = traineeDto.FullName,
                                                Email = traineeDto.Email,
                                                CitizenId = traineeDto.CitizenId
                                            });
                                            result.TraineeData.FailureCount++;
                                            continue;
                                        }

                                        // Create new UserSpecialty for existing user
                                        var newUserSpecialty = new UserSpecialty
                                        {
                                            UserId = traineeDto.UserId,
                                            SpecialtyId = traineeDto.SpecialtyId,
                                            CreatedAt = DateTime.UtcNow
                                        };
                                        await context.Set<UserSpecialty>().AddAsync(newUserSpecialty);
                                    }

                                    // Mark as successfully processed
                                    processedUsersByCitizenId[traineeDto.CitizenId] = traineeDto.UserId;
                                    result.TraineeData.SuccessCount++;
                                }
                                else
                                {
                                    // No UserId provided - validate and create new user
                                    var validationErrors = traineeDto.Validate(existingUsers, specialties);

                                    if (validationErrors.HasErrors)
                                    {
                                        result.TraineeData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = row,
                                            Reason = validationErrors.GetErrorMessage(),
                                            FullName = traineeDto.FullName,
                                            Email = traineeDto.Email,
                                            CitizenId = traineeDto.CitizenId
                                        });
                                        result.TraineeData.FailureCount++;
                                        continue;
                                    }

                                    // Generate new UserId
                                    var userId = await GenerateNextUserIdAsync(existingUsers);

                                    // Generate username from full name and userId
                                    var username = GenerateUsernameFromFullName(traineeDto.FullName, userId);

                                    // Create new User
                                    var newUser = new User
                                    {
                                        UserId = userId,
                                        Username = username,
                                        FullName = traineeDto.FullName,
                                        Sex = traineeDto.Gender,
                                        DateOfBirth = traineeDto.DateOfBirth,
                                        Address = traineeDto.Address ?? "",
                                        Email = traineeDto.Email,
                                        PhoneNumber = traineeDto.PhoneNumber,
                                        CitizenId = traineeDto.CitizenId,
                                        RoleId = traineeRoleId,
                                        PasswordHash = PasswordHashHelper.HashPassword("VJA@2025"),
                                        Status = AccountStatus.Pending,
                                        CreatedAt = DateTime.UtcNow,
                                        UpdatedAt = DateTime.UtcNow
                                    };

                                    await _unitOfWork.UserRepository.AddAsync(newUser);
                                    existingUsers.Add(newUser);

                                    // Create UserSpecialty
                                    var userSpecialty = new UserSpecialty
                                    {
                                        UserId = userId,
                                        SpecialtyId = traineeDto.SpecialtyId,
                                        CreatedAt = DateTime.UtcNow
                                    };
                                    var context = _unitOfWork.Context as DbContext;
                                    if (context != null)
                                    {
                                        await context.Set<UserSpecialty>().AddAsync(userSpecialty);
                                    }

                                    // Mark as successfully processed
                                    processedUsersByCitizenId[traineeDto.CitizenId] = userId;
                                    result.TraineeData.SuccessCount++;
                                }
                            }
                            catch (Exception ex)
                            {
                                result.TraineeData.Errors.Add(new ImportErrorDto
                                {
                                    RowNumber = row,
                                    Reason = $"Unexpected error: {ex.Message}",
                                    FullName = traineeSheet.Cells[row, 2].Text,
                                    Email = traineeSheet.Cells[row, 6].Text
                                });
                                result.TraineeData.FailureCount++;
                            }
                        }

                        foreach (var certGroup in certificatesByCitizenId)
                        {
                            var citizenId = certGroup.Key;
                            
                            // Check if CitizenId is in trainee sheet
                            if (!traineesCitizenIds.Contains(citizenId))
                            {
                                // CitizenId not in trainee sheet - skip with error
                                foreach (var certData in certGroup.Value)
                                {
                                    result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                    {
                                        RowNumber = certData.RowNumber,
                                        Reason = $"Citizen ID '{citizenId}' not found in Trainee sheet. Certificates can only be created for trainees in the current import.",
                                        CitizenId = citizenId,
                                        CertificateCode = certData.CertificateCode
                                    });
                                    result.ExternalCertificateData.FailureCount++;
                                }
                                continue;
                            }

                            // Check if this trainee was successfully processed
                            if (!processedUsersByCitizenId.ContainsKey(citizenId))
                            {
                                // Trainee was not successfully created/updated
                                foreach (var certData in certGroup.Value)
                                {
                                    result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                    {
                                        RowNumber = certData.RowNumber,
                                        Reason = $"Cannot create certificate: Trainee with Citizen ID '{citizenId}' failed validation or creation",
                                        CitizenId = citizenId,
                                        CertificateCode = certData.CertificateCode
                                    });
                                    result.ExternalCertificateData.FailureCount++;
                                }
                                continue;
                            }

                            // Get userId from processed trainees
                            var userId = processedUsersByCitizenId[citizenId];
                            
                            // Get user details
                            var user = await _unitOfWork.UserRepository
                                .GetSingleOrDefaultByNullableExpressionAsync(u => u.UserId == userId);
                            
                            if (user == null)
                            {
                                // This shouldn't happen, but safety check
                                foreach (var certData in certGroup.Value)
                                {
                                    result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                    {
                                        RowNumber = certData.RowNumber,
                                        Reason = $"User with Citizen ID '{citizenId}' not found in database",
                                        CitizenId = citizenId,
                                        CertificateCode = certData.CertificateCode
                                    });
                                    result.ExternalCertificateData.FailureCount++;
                                }
                                continue;
                            }

                            foreach (var certRawData in certGroup.Value)
                            {
                                try
                                {
                                    // Map to DTO
                                    var certDto = new ExternalCertificateImportDto
                                    {
                                        RowNumber = certRawData.RowNumber,
                                        CitizenId = certRawData.CitizenId,
                                        CertificateCode = certRawData.CertificateCode,
                                        CertificateName = certRawData.CertificateName,
                                        IssuingOrganization = certRawData.IssuingOrganization,
                                        IssueDateStr = certRawData.IssueDate,
                                        ExpiredDateStr = certRawData.ExpiredDate,
                                        CertificateImageBase64 = "" 
                                    };

                                    // Validate using DTO method
                                    var certValidation = certDto.Validate();

                                    if (certValidation.HasErrors)
                                    {
                                        result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = certDto.RowNumber,
                                            Reason = certValidation.GetErrorMessage(),
                                            CitizenId = citizenId,
                                            CertificateCode = certDto.CertificateCode
                                        });
                                        result.ExternalCertificateData.FailureCount++;
                                        continue;
                                    }

                                    // Check if image stream exists
                                    if (certRawData.ImageStream == null)
                                    {
                                        result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = certDto.RowNumber,
                                            Reason = "Certificate image is required. No image found in row.",
                                            CitizenId = citizenId,
                                            CertificateCode = certDto.CertificateCode
                                        });
                                        result.ExternalCertificateData.FailureCount++;
                                        continue;
                                    }

                                    // Upload certificate image to Cloudinary
                                    string imageUrl;
                                    try
                                    {
                                        // Reset stream position
                                        certRawData.ImageStream.Position = 0;
                                        
                                        var fileName = $"{citizenId}_{certDto.CertificateCode}_{DateTime.UtcNow.Ticks}.png";
                                        imageUrl = await _cloudinaryService.UploadImageAsync(certRawData.ImageStream, fileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = certDto.RowNumber,
                                            Reason = $"Failed to upload image to Cloudinary: {ex.Message}",
                                            CitizenId = citizenId,
                                            CertificateCode = certDto.CertificateCode
                                        });
                                        result.ExternalCertificateData.FailureCount++;
                                        continue;
                                    }

                                    // Verify imageUrl is valid
                                    if (string.IsNullOrEmpty(imageUrl))
                                    {
                                        result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                        {
                                            RowNumber = certDto.RowNumber,
                                            Reason = "Failed to get image URL from Cloudinary",
                                            CitizenId = citizenId,
                                            CertificateCode = certDto.CertificateCode
                                        });
                                        result.ExternalCertificateData.FailureCount++;
                                        continue;
                                    }

                                    // Create External Certificate with Cloudinary URL
                                    var externalCert = new ExternalCertificate
                                    {
                                        UserId = user.UserId,
                                        CertificateCode = certDto.CertificateCode,
                                        CertificateName = certDto.CertificateName,
                                        IssuingOrganization = certDto.IssuingOrganization,
                                        IssueDate = certDto.IssueDate,
                                        Exp_date = certDto.ExpiredDate,
                                        CertificateFileUrl = imageUrl, // Cloudinary URL
                                        CreatedAt = DateTime.UtcNow
                                    };

                                    await _unitOfWork.ExternalCertificateRepository.AddAsync(externalCert);
                                    result.ExternalCertificateData.SuccessCount++;
                                }
                                catch (Exception ex)
                                {
                                    result.ExternalCertificateData.Errors.Add(new ImportErrorDto
                                    {
                                        RowNumber = certRawData.RowNumber,
                                        Reason = $"Unexpected error: {ex.Message}",
                                        CitizenId = citizenId,
                                        CertificateCode = certRawData.CertificateCode
                                    });
                                    result.ExternalCertificateData.FailureCount++;
                                }
                            }
                        }
                    }
                }

                // Save all changes to database BEFORE sending notifications
                await _unitOfWork.SaveChangesAsync();

                response.Success = true;
                response.Data = result;
                response.Message = $"Trainee Import: {result.TraineeData.SuccessCount} succeeded, {result.TraineeData.FailureCount} failed | Certificate Import: {result.ExternalCertificateData.SuccessCount} succeeded, {result.ExternalCertificateData.FailureCount} failed";
                
                // Notify admins about the import (only if there were successful imports)
                if (result.TraineeData.SuccessCount > 0 || result.ExternalCertificateData.SuccessCount > 0)
                {
                    await _notificationService.NotifyAdminsAboutNewTraineesAsync(
                        result.TraineeData.SuccessCount,
                        result.TraineeData.FailureCount,
                        performedByUsername
                    );
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Failed to import trainees: {ex.Message}";
                response.Data = result;
                return response;
            }
        }

        private Task<string> GenerateNextUserIdAsync(List<User> existingUsers)
        {
            var currentYear = DateTime.Now.Year.ToString().Substring(2); // Get last 2 digits of year (e.g., "25")
            var prefix = $"VJA{currentYear}";

            // Find the highest existing number
            var existingNumbers = existingUsers
                .Where(u => u.UserId.StartsWith(prefix))
                .Select(u =>
                {
                    var numberPart = u.UserId.Substring(prefix.Length);
                    return int.TryParse(numberPart, out var num) ? num : 0;
                })
                .ToList();

            var nextNumber = existingNumbers.Any() ? existingNumbers.Max() + 1 : 1;

            // Format: VJA25XXXX (4 digits)
            return Task.FromResult($"{prefix}{nextNumber:D4}");
        }

        /// <summary>
        /// Generate username from full name and userId
        /// Example: "Pham Tran Hoang Minh" + "VJA250001" → "minhpthvja250001"
        /// </summary>
        private string GenerateUsernameFromFullName(string fullName, string userId)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return userId.ToLower();
            }

            // Split full name into parts
            var nameParts = fullName.Trim()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Where(part => !string.IsNullOrEmpty(part))
                .ToList();

            if (nameParts.Count == 0)
            {
                return userId.ToLower();
            }

            // Get first name (last part in Vietnamese names)
            var firstName = nameParts.Last();

            // Get initials from other parts (last name and middle names)
            var initials = "";
            for (int i = 0; i < nameParts.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(nameParts[i]))
                {
                    initials += nameParts[i][0];
                }
            }

            // Combine: firstname + initials + userid (all lowercase)
            var username = $"{firstName}{initials}{userId}".ToLower();

            return username;
        }
        #endregion
    }

    // Helper class for external certificate data
    internal class ExternalCertificateData
    {
        public int RowNumber { get; set; }
        public string CitizenId { get; set; } = string.Empty;
        public string CertificateCode { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
        public string IssuingOrganization { get; set; } = string.Empty;
        public string IssueDate { get; set; } = string.Empty;
        public string ExpiredDate { get; set; } = string.Empty;
        public MemoryStream? ImageStream { get; set; }
    }
}

