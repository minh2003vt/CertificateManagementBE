# SignalR Real-Time Notification Guide

## 🎯 Tổng Quan

Hệ thống notification sử dụng **SignalR** để gửi thông báo real-time từ server đến client. SignalR tự động chọn protocol tốt nhất (WebSocket → Server-Sent Events → Long Polling).

---

## 🏗️ Architecture

```
Client (Frontend)           SignalR Hub              NotificationService
     │                           │                            │
     │ 1. Connect với JWT        │                            │
     │ ────────────────────────> │                            │
     │                           │                            │
     │ 2. Join group user_{id}   │                            │
     │ <──────────────────────── │                            │
     │                           │                            │
     │                           │   3. SendNotification      │
     │                           │ <────────────────────────  │
     │                           │                            │
     │ 4. ReceiveNotification    │                            │
     │ <──────────────────────── │                            │
     │                           │                            │
```

---

## 🔧 Backend Configuration

### 1. **SignalR Hub** (`Certificate_Management_BE/Hubs/NotificationHub.cs`)

```csharp
[Authorize]
public class NotificationHub : Hub<INotificationHub>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            // Join user-specific group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            // Join admin group if user is admin
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Admin")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
            }
        }
        
        await base.OnConnectedAsync();
    }
}
```

**Features:**
- ✅ Requires JWT authentication (`[Authorize]`)
- ✅ Auto-joins user to their personal group: `user_{userId}`
- ✅ Auto-joins admins to `admins` group
- ✅ Strongly typed with `INotificationHub` interface

### 2. **Hub Interface** (`Application/IHubs/INotificationHub.cs`)

```csharp
public interface INotificationHub
{
    Task ReceiveNotification(object notificationData);
    Task NotificationMarkedAsRead(int notificationId);
}
```

### 3. **JWT Authentication for SignalR** (`Program.cs`)

```csharp
options.Events = new JwtBearerEvents
{
    OnMessageReceived = context =>
    {
        // Allow SignalR to read JWT from query string
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

**Why?** SignalR không thể gửi header trong WebSocket, nên JWT phải được gửi qua query string.

### 4. **Hub Mapping** (`Program.cs`)

```csharp
app.MapHub<Certificate_Management_BE.Hubs.NotificationHub>("/hubs/notification");
```

**Endpoint**: `wss://your-domain/hubs/notification`

---

## 💻 Frontend Integration

### 1. **Install SignalR Client**

```bash
npm install @microsoft/signalr
```

### 2. **Connect to Hub (TypeScript/JavaScript)**

```typescript
import * as signalR from "@microsoft/signalr";

// Get JWT token (from login response or localStorage)
const token = localStorage.getItem("authToken");

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://your-api-domain/hubs/notification", {
    accessTokenFactory: () => token || ""
  })
  .withAutomaticReconnect() // Auto-reconnect if disconnected
  .configureLogging(signalR.LogLevel.Information)
  .build();

// Start connection
connection.start()
  .then(() => {
    console.log("✅ Connected to NotificationHub");
  })
  .catch(err => {
    console.error("❌ Connection failed:", err);
  });
```

### 3. **Listen for Notifications**

```typescript
connection.on("ReceiveNotification", (notificationData) => {
  console.log("📬 New notification:", notificationData);
  
  // notificationData structure:
  // {
  //   userId: "VJA250001",
  //   title: "New Trainees Imported",
  //   message: "John Doe imported 5 trainee(s): 4 succeeded, 1 failed.",
  //   timestamp: "2025-10-08T10:30:00Z",
  //   data: {
  //     type: "trainee_import",
  //     successCount: 4,
  //     failureCount: 1,
  //     performedBy: "John Doe"
  //   }
  // }
  
  // Display notification to user (toast, bell icon, etc.)
  showToast(notificationData.title, notificationData.message);
  updateNotificationBadge();
});
```

### 4. **React Example (Complete)**

```typescript
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

interface NotificationData {
  userId: string;
  title: string;
  message: string;
  timestamp: string;
  data?: any;
}

export const useNotifications = () => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [notifications, setNotifications] = useState<NotificationData[]>([]);

  useEffect(() => {
    const token = localStorage.getItem('authToken');
    if (!token) return;

    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://your-api-domain/hubs/notification', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    newConnection.on('ReceiveNotification', (data: NotificationData) => {
      setNotifications(prev => [data, ...prev]);
      
      // Show toast notification
      toast.info(data.message, {
        title: data.title,
        duration: 5000
      });
    });

    newConnection.start()
      .then(() => console.log('✅ SignalR Connected'))
      .catch(err => console.error('❌ SignalR Error:', err));

    setConnection(newConnection);

    return () => {
      newConnection.stop();
    };
  }, []);

  return { notifications, connection };
};

// Usage in component
function NotificationBell() {
  const { notifications } = useNotifications();

  return (
    <div className="notification-bell">
      <BellIcon />
      {notifications.length > 0 && (
        <span className="badge">{notifications.length}</span>
      )}
    </div>
  );
}
```

### 5. **Angular Example**

```typescript
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private connection: signalR.HubConnection;
  public notifications$ = new BehaviorSubject<any[]>([]);

  constructor() {
    const token = localStorage.getItem('authToken');
    
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://your-api-domain/hubs/notification', {
        accessTokenFactory: () => token || ''
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveNotification', (data) => {
      const current = this.notifications$.value;
      this.notifications$.next([data, ...current]);
    });

    this.start();
  }

  start() {
    this.connection.start()
      .then(() => console.log('✅ SignalR Connected'))
      .catch(err => console.error('❌ SignalR Error:', err));
  }

  stop() {
    this.connection.stop();
  }
}
```

### 6. **Vue.js 3 (Composition API) Example**

```typescript
import { ref, onMounted, onUnmounted } from 'vue';
import * as signalR from '@microsoft/signalr';

export function useNotifications() {
  const notifications = ref([]);
  let connection: signalR.HubConnection | null = null;

  onMounted(() => {
    const token = localStorage.getItem('authToken');
    if (!token) return;

    connection = new signalR.HubConnectionBuilder()
      .withUrl('https://your-api-domain/hubs/notification', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveNotification', (data) => {
      notifications.value.unshift(data);
    });

    connection.start()
      .then(() => console.log('✅ SignalR Connected'))
      .catch(err => console.error('❌ SignalR Error:', err));
  });

  onUnmounted(() => {
    connection?.stop();
  });

  return { notifications };
}
```

---

## 📡 Testing SignalR Connection

### 1. **Browser Console Test**

```javascript
// Open browser console and paste:
const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7001/hubs/notification", {
    accessTokenFactory: () => "YOUR_JWT_TOKEN_HERE"
  })
  .build();

connection.on("ReceiveNotification", (data) => {
  console.log("📬 Notification:", data);
});

connection.start().then(() => console.log("✅ Connected"));
```

### 2. **Postman/Thunder Client (Import Trainee to Trigger Notification)**

```http
POST https://localhost:7001/User/import-trainees
Authorization: Bearer {education_officer_token}
Content-Type: multipart/form-data

file: trainee_data.xlsx
```

**Result:** All admin users connected to SignalR will receive real-time notification.

---

## 🔐 Security Features

### ✅ **JWT Authentication Required**
- All SignalR connections require valid JWT token
- Token is validated on connection

### ✅ **Group-Based Authorization**
- Users can only join their own group: `user_{userId}`
- Admins automatically join `admins` group

### ✅ **HTTPS/WSS in Production**
- Use `wss://` (WebSocket Secure) in production
- TLS/SSL encryption for all connections

---

## 🎯 Current Notification Triggers

### 1. **Trainee Import** (implemented)

**Triggered when:** Education Officer imports trainees via Excel

**Sent to:** All Admin users

**Data structure:**
```json
{
  "userId": "ADMIN001",
  "title": "New Trainees Imported",
  "message": "John Doe imported 5 trainee(s): 4 succeeded, 1 failed.",
  "timestamp": "2025-10-08T10:30:00Z",
  "data": {
    "type": "trainee_import",
    "successCount": 4,
    "failureCount": 1,
    "performedBy": "John Doe"
  }
}
```

---

## 🚀 Expanding Notification System

### Add New Notification Type

1. **In your service:**
```csharp
await _notificationService.SendSignalRNotificationAsync(
    userId: "VJA250001",
    title: "Certificate Approved",
    message: "Your certificate #12345 has been approved",
    data: new {
        type = "certificate_approved",
        certificateId = 12345
    }
);
```

2. **In frontend:**
```typescript
connection.on("ReceiveNotification", (data) => {
  switch(data.data?.type) {
    case "trainee_import":
      // Handle trainee import
      break;
    case "certificate_approved":
      // Handle certificate approval
      break;
    case "course_enrollment":
      // Handle course enrollment
      break;
  }
});
```

---

## 📊 Connection Status Handling

```typescript
connection.onreconnecting((error) => {
  console.warn("⚠️ Reconnecting...", error);
  showReconnectingUI();
});

connection.onreconnected((connectionId) => {
  console.log("✅ Reconnected!", connectionId);
  hideReconnectingUI();
});

connection.onclose((error) => {
  console.error("❌ Connection closed", error);
  showDisconnectedUI();
});
```

---

## 🐛 Troubleshooting

### **Problem: "Cannot connect to SignalR"**
**Solution:**
- Check JWT token is valid
- Verify endpoint URL: `/hubs/notification`
- Check browser console for CORS errors
- Ensure backend is running

### **Problem: "Notifications not received"**
**Solution:**
- Check user is connected (check `OnConnectedAsync` logs)
- Verify user is in correct group
- Check notification is being sent (check backend logs)
- Ensure `ReceiveNotification` handler is registered before connection starts

### **Problem: "401 Unauthorized"**
**Solution:**
- Ensure JWT token is passed in `accessTokenFactory`
- Check token is not expired
- Verify token format (should be just the token, not "Bearer {token}")

---

## 📚 API Reference

### **Backend - Sending Notifications**

```csharp
Task<bool> SendSignalRNotificationAsync(
    string userId,      // Target user ID
    string title,       // Notification title
    string message,     // Notification message
    object? data = null // Additional data (optional)
)
```

### **Frontend - Client Methods**

| Method                     | Description                            |
|----------------------------|----------------------------------------|
| `ReceiveNotification`      | Receives new notifications from server |
| `NotificationMarkedAsRead` | Confirmation of notification read      |

---

## 🎉 Summary

**SignalR Implementation Benefits:**
- ✅ **Native .NET** - No external dependencies
- ✅ **Real-time** - Instant notifications
- ✅ **Auto-reconnect** - Handles disconnections
- ✅ **Type-safe** - Strongly typed interfaces
- ✅ **Scalable** - Supports multiple transports
- ✅ **Secure** - JWT authentication built-in
- ✅ **Cross-platform** - Works on web, mobile, desktop

**vs Firebase:**
- ✅ No credential files needed
- ✅ No external service dependencies
- ✅ Free (no quotas or limits)
- ✅ Easier to debug and monitor

**Next Steps:**
1. Test SignalR connection from frontend
2. Implement notification UI components
3. Add more notification types as needed
4. Consider adding notification history/persistence

**Happy Coding! 🚀**


