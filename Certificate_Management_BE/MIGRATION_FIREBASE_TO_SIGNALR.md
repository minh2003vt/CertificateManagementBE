# Migration: Firebase → SignalR

## ✅ Completed Migration

Hệ thống notification đã được migrate từ **Firebase Cloud Messaging** sang **SignalR** thành công.

---

## 📋 Changes Summary

### **Removed:**
- ❌ Firebase Admin SDK package
- ❌ `firebase-credentials.json` file
- ❌ Firebase configuration in `appsettings.json`
- ❌ Firebase initialization code in `NotificationService`
- ❌ `SendFirebaseNotificationAsync()` method

### **Added:**
- ✅ Microsoft.AspNetCore.SignalR package
- ✅ `NotificationHub` class (SignalR Hub)
- ✅ `INotificationHub` interface (typed Hub)
- ✅ SignalR configuration in `Program.cs`
- ✅ `SendSignalRNotificationAsync()` method
- ✅ JWT authentication for SignalR connections

---

## 🔧 Technical Changes

### 1. **Package Changes**

**Before:**
```xml
<PackageReference Include="FirebaseAdmin" Version="3.4.0" />
```

**After:**
```xml
<PackageReference Include="Microsoft.AspNetCore.SignalR" Version="1.2.0" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Core" Version="1.2.0" />
```

### 2. **Configuration Changes**

**Before (`appsettings.json`):**
```json
{
  "Firebase": {
    "CredentialPath": "firebase-credentials.json"
  }
}
```

**After:**
No configuration needed - SignalR works out of the box!

### 3. **Service Registration (`Program.cs`)**

**Before:**
```csharp
// No SignalR registration
```

**After:**
```csharp
// Add SignalR
builder.Services.AddSignalR();

// Map SignalR Hub
app.MapHub<Certificate_Management_BE.Hubs.NotificationHub>("/hubs/notification");

// JWT authentication for SignalR
options.Events = new JwtBearerEvents
{
    OnMessageReceived = context =>
    {
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;
        
        if (!string.IsNullOrEmpty(accessToken) && 
            path.StartsWithSegments("/hubs/notification"))
        {
            context.Token = accessToken;
        }
        return Task.CompletedTask;
    }
};
```

### 4. **NotificationService Changes**

**Before:**
```csharp
private readonly IConfiguration _configuration;
private static bool _firebaseInitialized = false;

public async Task<bool> SendFirebaseNotificationAsync(
    string userId, 
    string title, 
    string message, 
    Dictionary<string, string>? data = null)
{
    // Firebase implementation
    var firebaseMessage = new Message()
    {
        Topic = $"user_{userId}",
        Notification = new FirebaseAdmin.Messaging.Notification
        {
            Title = title,
            Body = message
        },
        Data = messageData
    };
    await FirebaseMessaging.DefaultInstance.SendAsync(firebaseMessage);
}
```

**After:**
```csharp
private readonly IHubContext<Hub<INotificationHub>, INotificationHub> _hubContext;

public async Task<bool> SendSignalRNotificationAsync(
    string userId, 
    string title, 
    string message, 
    object? data = null)
{
    // SignalR implementation
    var notificationData = new
    {
        userId = userId,
        title = title,
        message = message,
        timestamp = DateTime.UtcNow,
        data = data
    };
    
    await _hubContext.Clients.Group($"user_{userId}")
        .ReceiveNotification(notificationData);
}
```

---

## 🎯 Frontend Migration Guide

### **Before (Firebase):**

```javascript
// Install
npm install firebase

// Initialize
import { initializeApp } from 'firebase/app';
import { getMessaging, getToken, onMessage } from 'firebase/messaging';

const app = initializeApp(firebaseConfig);
const messaging = getMessaging(app);

// Get token
getToken(messaging, { vapidKey: 'YOUR_VAPID_KEY' })
  .then((token) => {
    // Subscribe to topic
  });

// Listen for messages
onMessage(messaging, (payload) => {
  console.log('Message:', payload);
});
```

### **After (SignalR):**

```javascript
// Install
npm install @microsoft/signalr

// Connect
import * as signalR from '@microsoft/signalr';

const token = localStorage.getItem('authToken');

const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://your-api/hubs/notification', {
    accessTokenFactory: () => token || ''
  })
  .withAutomaticReconnect()
  .build();

// Listen for notifications
connection.on('ReceiveNotification', (data) => {
  console.log('Notification:', data);
});

// Start connection
connection.start()
  .then(() => console.log('Connected'))
  .catch(err => console.error('Error:', err));
```

---

## ✅ Migration Checklist

- [x] Remove Firebase packages
- [x] Add SignalR packages
- [x] Create NotificationHub class
- [x] Create INotificationHub interface
- [x] Update NotificationService to use SignalR
- [x] Configure SignalR in Program.cs
- [x] Add JWT authentication for SignalR
- [x] Remove Firebase configuration from appsettings.json
- [x] Delete firebase-credentials.json
- [x] Update documentation
- [x] Test SignalR connections
- [x] Build succeeds without errors

---

## 📊 Comparison

| Feature | Firebase | SignalR |
|---------|----------|---------|
| **Cost** | Free tier limited | Completely free |
| **Setup** | Requires Google account + credentials file | No setup needed |
| **Dependencies** | External service | Native .NET |
| **Authentication** | FCM tokens | JWT (same as API) |
| **Connection** | Topic-based | Group-based |
| **Debugging** | External logs | Server logs |
| **Mobile Push** | Yes (OS-level) | No (requires app open) |
| **Web Real-time** | Yes | Yes |
| **Offline** | Limited | No |
| **Type Safety** | Weak | Strong (TypeScript/C#) |

---

## 🚀 Benefits of SignalR

### **For Development:**
- ✅ No external service setup
- ✅ No credential management
- ✅ Direct server logs for debugging
- ✅ Hot-reload works immediately

### **For Production:**
- ✅ No Firebase account needed
- ✅ No API quotas or limits
- ✅ No credential file deployment issues
- ✅ Works with existing infrastructure

### **For Frontend:**
- ✅ Simpler connection code
- ✅ Same JWT token as API
- ✅ Auto-reconnect built-in
- ✅ TypeScript support out-of-box

---

## ⚠️ Important Notes

### **SignalR Limitations:**
- **No OS-level push notifications** - User must have browser/app open
- **No offline notifications** - Messages only sent to connected clients

### **When to use Firebase instead:**
- Mobile apps that need push notifications when app is closed
- Need to send notifications to offline users
- Want OS-level notification badges

### **For this project:**
✅ SignalR is perfect because:
- Certificate management is a **web-based** admin system
- Users are typically **online** when using the system
- Real-time updates only matter when actively managing certificates
- No mobile app (web only)

---

## 🎉 Migration Complete!

**Status:** ✅ All tests passing, build successful

**Next Steps:**
1. Update frontend to use SignalR (see `SIGNALR_NOTIFICATION_GUIDE.md`)
2. Test real-time notifications
3. Monitor SignalR connections in production

**Documentation:**
- Full SignalR guide: `SIGNALR_NOTIFICATION_GUIDE.md`
- API reference: `NOTIFICATION_SYSTEM_SUMMARY.md`

