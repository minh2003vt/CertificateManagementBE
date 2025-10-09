# 🐛 Notification System Debug Guide

## Issue: Empty Notifications Array

If you're getting `"data": []` when fetching notifications, follow this step-by-step debugging guide.

---

## ✅ Step 1: Check if Admin Users Exist in Database

```sql
-- Connect to your PostgreSQL database and run:
SELECT "UserId", "Username", "FullName", "RoleId" 
FROM "Users" 
WHERE "RoleId" = 1;  -- 1 = Admin role

-- Also check Role table
SELECT * FROM "Roles" WHERE "RoleId" = 1;
```

**Expected Result:** At least one admin user should exist.

**If No Admins:**
- The notification service logs: `"No admin users found to notify"`
- Solution: Create an admin user or check if your admin user has `RoleId = 1`

---

## ✅ Step 2: Test Notification Endpoint

### **2.1 Use Test Endpoint (Easiest)**

```bash
POST http://localhost:5000/Notification/test-admin-notification
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Test notification sent to all admins. Check your notifications!"
}
```

### **2.2 Check Your Notifications**

```bash
GET http://localhost:5000/Notification
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Response:**
```json
{
  "data": [
    {
      "notificationId": 1,
      "userId": "USR-ADMIN",
      "title": "New Trainees Imported",
      "message": "Test User imported 7 trainee(s): 5 succeeded, 2 failed.",
      "notificationType": "Trainee Import",
      "createdAt": "2025-01-15T10:30:00Z",
      "isRead": false
    }
  ],
  "success": true,
  "message": "Notifications retrieved successfully"
}
```

---

## ✅ Step 3: Check Database for Notifications

```sql
-- Check if notifications were created
SELECT * FROM "Notifications" ORDER BY "CreatedAt" DESC LIMIT 10;

-- Count notifications per user
SELECT "UserId", COUNT(*) as NotificationCount 
FROM "Notifications" 
GROUP BY "UserId";
```

**Expected Result:** You should see notification records in the database.

---

## ✅ Step 4: Check Application Logs

Look for these log messages in your console/logs:

### **Success Logs:**
```
info: Application.Services.NotificationService[0]
      Successfully sent SignalR notification to user USR-ADMIN
      
info: Application.Services.NotificationService[0]
      Notified 1 admin(s) about trainee import
```

### **Error Logs:**
```
error: Application.Services.NotificationService[0]
       Unable to access DbContext for querying admin users
       
warn: Application.Services.NotificationService[0]
      No admin users found to notify
      
error: Application.Services.NotificationService[0]
       Error notifying admins about new trainees
```

---

## ✅ Step 5: Verify Import Success Condition

The notification is only sent if **trainees were successfully imported**:

```csharp
if (result.TraineeData.SuccessCount > 0 || result.ExternalCertificateData.SuccessCount > 0)
{
    await _notificationService.NotifyAdminsAboutNewTraineesAsync(...);
}
```

**Check Your Import Response:**
```json
{
  "data": {
    "traineeData": {
      "totalRows": 1,
      "successCount": 1,  // <-- Must be > 0
      "failureCount": 0,
      "errors": []
    },
    "externalCertificateData": {
      "totalRows": 1,
      "successCount": 0,
      "failureCount": 0,
      "errors": []
    }
  },
  "success": true,
  "message": "Trainee Import: 1 succeeded, 0 failed | Certificate Import: 0 succeeded, 0 failed"
}
```

**If `successCount = 0`:**
- Notification will NOT be sent
- Check why trainees failed to import (see `errors` array)

---

## ✅ Step 6: Check Your User's Role

Make sure you're logged in as an **Admin** user to see notifications:

```sql
SELECT u."UserId", u."Username", r."RoleName"
FROM "Users" u
JOIN "Roles" r ON u."RoleId" = r."RoleId"
WHERE u."UserId" = 'YOUR_USER_ID';
```

**Expected:** `RoleName` should be `"Admin"`

---

## ✅ Step 7: Common Issues & Solutions

### **Issue 1: "No admin users found"**
**Cause:** No users with `RoleId = 1` or Role name not matching `"Admin"`

**Solution:**
```sql
-- Check role configuration
SELECT * FROM "Roles" WHERE "RoleId" = 1;

-- Update role name if needed
UPDATE "Roles" SET "RoleName" = 'Admin' WHERE "RoleId" = 1;
```

### **Issue 2: SaveChangesAsync not called**
**Cause:** The `await _unitOfWork.SaveChangesAsync();` was missing

**Solution:** Already fixed in code at line 595 of `UserService.cs`

### **Issue 3: Wrong UserId in JWT token**
**Cause:** Your JWT token has wrong `ClaimTypes.NameIdentifier`

**Solution:** 
- Decode your JWT at https://jwt.io
- Check if `nameid` claim matches your `UserId` in database
- Re-login if needed

### **Issue 4: Notifications table doesn't exist**
**Cause:** Migration not applied

**Solution:**
```bash
dotnet ef database update --project Infrastructure --startup-project Certificate_Management_BE
```

---

## 🧪 Complete Test Flow

### **1. Login as Admin**
```bash
POST http://localhost:5000/Authentication/Login
Content-Type: application/json

{
  "usernameOrEmail": "admin@example.com",
  "password": "string"
}
```

Save the JWT token from response.

### **2. Test Notification System**
```bash
POST http://localhost:5000/Notification/test-admin-notification
Authorization: Bearer YOUR_JWT_TOKEN
```

### **3. Fetch Notifications**
```bash
GET http://localhost:5000/Notification
Authorization: Bearer YOUR_JWT_TOKEN
```

### **4. Import Trainees**
```bash
POST http://localhost:5000/User/import-trainees
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: multipart/form-data

[Upload Excel file]
```

### **5. Check Notifications Again**
```bash
GET http://localhost:5000/Notification
Authorization: Bearer YOUR_JWT_TOKEN
```

You should see a new notification about the trainee import!

---

## 🔍 Direct Database Check

If all else fails, manually check if the notification function works:

```sql
-- Insert a test notification directly
INSERT INTO "Notifications" ("UserId", "Title", "Message", "NotificationType", "CreatedAt", "IsRead")
VALUES ('USR-ADMIN', 'Test Notification', 'This is a test', 'Manual Test', NOW(), false);

-- Then fetch it
SELECT * FROM "Notifications" WHERE "UserId" = 'USR-ADMIN';
```

If you can see it in the database but not via API:
- Check your `GetUserNotificationsAsync` implementation
- Verify JWT token contains correct `UserId`
- Check `NotificationRepository.GetByUserIdAsync` implementation

---

## 📊 Expected Database State

After a successful import, your database should look like this:

**Users Table:**
```
UserId      | Username           | RoleId | ...
USR-ADMIN   | admin              | 1      | ...
VJA250001   | minhpthvja250001   | 4      | ...  (new trainee)
```

**Notifications Table:**
```
NotificationId | UserId    | Title                  | Message                      | IsRead
1              | USR-ADMIN | New Trainees Imported  | admin imported 1 trainee(s)... | false
```

---

## 🎯 Quick Checklist

- [ ] Admin user exists with `RoleId = 1`
- [ ] Role with `RoleId = 1` has `RoleName = 'Admin'`
- [ ] Trainee import succeeds (`successCount > 0`)
- [ ] `SaveChangesAsync()` is called (line 595 in UserService.cs)
- [ ] Test endpoint returns success
- [ ] Database has notification records
- [ ] JWT token is valid and has correct `UserId`
- [ ] Application logs show notification sent
- [ ] GET `/Notification` returns your notifications

---

## 💡 Still Not Working?

1. **Check application logs** for any errors
2. **Run test endpoint** to isolate the issue
3. **Check database directly** using SQL queries above
4. **Verify your JWT token** at https://jwt.io
5. **Restart the application** after code changes

If notifications appear in database but not in API response:
- Problem is in `GetUserNotificationsAsync` or repository
- Check if `UserId` from JWT matches database

If notifications don't appear in database:
- Problem is in `NotifyAdminsAboutNewTraineesAsync`
- Check logs for errors
- Verify admin users exist

---

## 📞 Support

If you've followed all steps and it still doesn't work, provide:
1. Application logs during import
2. SQL query results for admin users
3. SQL query results for notifications table
4. Import API response
5. GET /Notification API response


