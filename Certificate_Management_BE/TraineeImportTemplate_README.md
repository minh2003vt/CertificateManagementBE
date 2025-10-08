# Trainee Import Template Guide

## Excel File Structure

Your Excel file must contain **2 sheets**:
1. **Sheet 1: Trainees** - Contains trainee information
2. **Sheet 2: ExternalCertificates** - Contains external certificate data

---

## Sheet 1: Trainees

### Columns (in order):

| Column | Field Name | Type | Required | Validation | Example |
|--------|-----------|------|----------|------------|---------|
| A | UserId | string | No | Auto-generated if empty (VJA25XXXX format) | VJA250001 |
| B | FullName | string | Yes | Must not be empty | Huu Duc Nguyen Tran |
| C | Gender | string | Yes | Must be "Male" or "Female" | Male |
| D | DateOfBirth | date | Yes | Must be 18+ years old | 10/06/2003 |
| E | Address | string | No | | Nha lau |
| F | Email | string | Yes | Valid email format, must be unique | huuduc20031501@gmail.com |
| G | PhoneNumber | string | Yes | Must be exactly 10 digits | 902757690 |
| H | CitizenId | string | Yes | Must be exactly 12 digits, must be unique | 79201252637 |
| I | SpecialtyId | string | Yes | Must exist in database | SPL-PS |

### Notes:
- **UserId**: If left empty, system will auto-generate with format `VJA{Year}{SequentialNumber}` (e.g., VJA250001, VJA250002)
- **Default Password**: All imported trainees will have password `VJA@2025`
- **Gender**: Case-insensitive ("male", "MALE", "Male" all work)
- **Age**: Calculated from DateOfBirth, must be at least 18 years old
- **Specialty**: Must match existing specialty IDs in system:
  - SPL-PS: Passenger service
  - SPL-RA: Ramp agent
  - SPL-DOC: Document Staff
  - SPL-CC: Cabin Crew
  - SPL-TAM: Technical Aircraft Maintenance
  - SPL-FC: Flight Crew

---

## Sheet 2: ExternalCertificates

### Columns (in order):

| Column | Field Name | Type | Required | Example |
|--------|-----------|------|----------|---------|
| A | CitizenId | string | Yes | 79201252637 |
| B | CertificateCode | string | Yes | DRIVE-003 |
| C | CertificateName | string | Yes | Bằng lái xe |
| D | IssuingOrganization | string | Yes | Bộ Công an |
| E | IssueDate | date | Yes | 1/1/2000 |
| F | ExpiredDate | date | Yes | 1/1/2002 |
| G | CertificateImageBase64 | string | No | [Base64 encoded image string] |

### Notes:
- **CitizenId**: Must match a CitizenId from Sheet 1 (Trainees)
- **CertificateImageBase64**: 
  - Can be left empty
  - If provided, should be a Base64-encoded image string
  - Image will be uploaded to Cloudinary automatically
  - If upload fails, the Base64 string will be stored as fallback

---

## API Endpoint

### Import Trainees
```http
POST /User/import-trainees
Authorization: Required (Education Officer role only)
Content-Type: multipart/form-data
```

**Request Body:**
- `file`: Excel file (.xlsx or .xls)

**Response:**
```json
{
  "success": true,
  "message": "Import completed: 5 succeeded, 2 failed",
  "data": {
    "totalRows": 7,
    "successCount": 5,
    "failureCount": 2,
    "errors": [
      {
        "rowNumber": 3,
        "reason": "Email 'duplicate@example.com' already exists",
        "fullName": "John Doe",
        "email": "duplicate@example.com"
      },
      {
        "rowNumber": 5,
        "reason": "Trainee must be at least 18 years old; Specialty 'INVALID' not found in database",
        "fullName": "Jane Smith",
        "email": "jane@example.com"
      }
    ]
  }
}
```

---

## Cloudinary Configuration

Before using the import feature, configure Cloudinary in `appsettings.json`:

```json
"Cloudinary": {
  "CloudName": "your-cloud-name",
  "ApiKey": "your-api-key",
  "ApiSecret": "your-api-secret"
}
```

To get Cloudinary credentials:
1. Sign up at https://cloudinary.com
2. Go to Dashboard → Account Details
3. Copy CloudName, API Key, and API Secret

---

## Common Validation Errors

1. **"Full name is required"** - Column B is empty
2. **"Invalid email format"** - Email doesn't match valid format
3. **"Phone number must be 10 digits"** - Phone is not exactly 10 digits
4. **"Citizen ID must be 12 digits"** - CitizenId is not exactly 12 digits
5. **"Gender must be 'Male' or 'Female'"** - Invalid gender value
6. **"Invalid date of birth format"** - Date cannot be parsed
7. **"Trainee must be at least 18 years old"** - Age < 18
8. **"Email 'xxx' already exists"** - Duplicate email in database
9. **"Citizen ID 'xxx' already exists"** - Duplicate CitizenId in database
10. **"Specialty 'xxx' not found in database"** - Invalid SpecialtyId

---

## Example Excel Data

### Sheet 1: Trainees
| UserId | FullName | Gender | DateOfBirth | Address | Email | PhoneNumber | CitizenId | SpecialtyId |
|--------|----------|--------|-------------|---------|-------|-------------|-----------|-------------|
| | Huu Duc Nguyen Tran | Male | 10/06/2003 | Nha lau | huuduc2003@gmail.com | 0902757690 | 792012526371 | SPL-PS |
| VJA250002 | Jane Doe | Female | 15/03/2000 | 123 Street | jane@example.com | 0901234567 | 123456789012 | SPL-CC |

### Sheet 2: ExternalCertificates
| CitizenId | CertificateCode | CertificateName | IssuingOrganization | IssueDate | ExpiredDate | CertificateImageBase64 |
|-----------|-----------------|-----------------|---------------------|-----------|-------------|------------------------|
| 792012526371 | DRIVE-003 | Bằng lái xe | Bộ Công an | 1/1/2000 | 1/1/2002 | [Base64 string] |
| 123456789012 | ENG-CERT-001 | English Certificate | British Council | 1/6/2020 | 1/6/2025 | |


