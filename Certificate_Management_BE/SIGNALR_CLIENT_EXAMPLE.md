# SignalR Notification Client - Quick Start Guide

## 🚀 How to Connect to `/hubs/notification`

### **Prerequisites**
1. You have a valid JWT token (from login)
2. Your backend is running on `http://localhost:5000` (adjust URL as needed)
3. Install SignalR client library

---

## 📦 1. Install SignalR Client

### For **JavaScript/HTML**:
```html
<!-- CDN -->
<script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
```

### For **React/Vue/Angular**:
```bash
npm install @microsoft/signalr
```

---

## 🔌 2. Connect to SignalR Hub

### **JavaScript Example (Vanilla JS)**

```javascript
// Get your JWT token (from login response or localStorage)
const jwtToken = "your-jwt-token-here"; // Replace with actual token

// Create connection with JWT in query string
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/hubs/notification", {
        accessTokenFactory: () => jwtToken  // JWT passed here
    })
    .withAutomaticReconnect() // Auto-reconnect if disconnected
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Listen for incoming notifications
connection.on("ReceiveNotification", (data) => {
    console.log("📩 New notification received:", data);
    
    // Display notification to user
    alert(`${data.title}\n${data.message}`);
    
    // Or update your UI
    displayNotification(data);
});

// Start the connection
connection.start()
    .then(() => {
        console.log("✅ Connected to SignalR NotificationHub");
    })
    .catch(err => {
        console.error("❌ Connection failed:", err);
    });

// Function to display notification in your UI
function displayNotification(data) {
    const notificationDiv = document.createElement("div");
    notificationDiv.className = "notification";
    notificationDiv.innerHTML = `
        <h4>${data.title}</h4>
        <p>${data.message}</p>
        <small>${new Date(data.timestamp).toLocaleString()}</small>
    `;
    document.getElementById("notifications-container").prepend(notificationDiv);
}
```

---

## ⚛️ 3. React Example

```jsx
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

function NotificationComponent() {
    const [notifications, setNotifications] = useState([]);
    const [connection, setConnection] = useState(null);

    useEffect(() => {
        // Get JWT token from your auth context/localStorage
        const jwtToken = localStorage.getItem('jwt_token');

        // Create SignalR connection
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/hubs/notification", {
                accessTokenFactory: () => jwtToken
            })
            .withAutomaticReconnect()
            .build();

        // Listen for notifications
        newConnection.on("ReceiveNotification", (data) => {
            console.log("📩 Notification:", data);
            
            // Add to notification list
            setNotifications(prev => [data, ...prev]);
            
            // Show browser notification
            if (Notification.permission === "granted") {
                new Notification(data.title, {
                    body: data.message,
                    icon: "/notification-icon.png"
                });
            }
        });

        // Start connection
        newConnection.start()
            .then(() => {
                console.log("✅ Connected to SignalR");
                setConnection(newConnection);
            })
            .catch(err => console.error("❌ Connection error:", err));

        // Cleanup on unmount
        return () => {
            if (newConnection) {
                newConnection.stop();
            }
        };
    }, []);

    return (
        <div className="notifications">
            <h3>Real-time Notifications</h3>
            {notifications.map((notif, index) => (
                <div key={index} className="notification-item">
                    <h4>{notif.title}</h4>
                    <p>{notif.message}</p>
                    <small>{new Date(notif.timestamp).toLocaleString()}</small>
                </div>
            ))}
        </div>
    );
}

export default NotificationComponent;
```

---

## 🎯 4. Vue Example

```vue
<template>
  <div class="notifications">
    <h3>Real-time Notifications</h3>
    <div v-for="(notif, index) in notifications" :key="index" class="notification-item">
      <h4>{{ notif.title }}</h4>
      <p>{{ notif.message }}</p>
      <small>{{ formatDate(notif.timestamp) }}</small>
    </div>
  </div>
</template>

<script>
import * as signalR from '@microsoft/signalr';

export default {
  name: 'NotificationComponent',
  data() {
    return {
      notifications: [],
      connection: null
    };
  },
  mounted() {
    this.connectToHub();
  },
  beforeUnmount() {
    if (this.connection) {
      this.connection.stop();
    }
  },
  methods: {
    connectToHub() {
      // Get JWT from Vuex store or localStorage
      const jwtToken = localStorage.getItem('jwt_token');

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/hubs/notification", {
          accessTokenFactory: () => jwtToken
        })
        .withAutomaticReconnect()
        .build();

      // Listen for notifications
      this.connection.on("ReceiveNotification", (data) => {
        console.log("📩 Notification:", data);
        this.notifications.unshift(data);
        
        // Show notification
        this.$notify({
          title: data.title,
          message: data.message,
          type: 'info'
        });
      });

      // Start connection
      this.connection.start()
        .then(() => console.log("✅ Connected to SignalR"))
        .catch(err => console.error("❌ Connection error:", err));
    },
    formatDate(date) {
      return new Date(date).toLocaleString();
    }
  }
};
</script>
```

---

## 📡 5. Test Your Connection

### **Step 1: Check Backend is Running**
```bash
# Backend should be running on http://localhost:5000
# Check if hub is accessible
curl http://localhost:5000/hubs/notification
```

### **Step 2: Get Your JWT Token**
Login via API and get the token:
```javascript
fetch('http://localhost:5000/Authentication/Login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
        usernameOrEmail: 'admin@example.com',
        password: 'yourpassword'
    })
})
.then(res => res.json())
.then(data => {
    console.log('JWT Token:', data.data.token);
    localStorage.setItem('jwt_token', data.data.token);
});
```

### **Step 3: Connect to SignalR**
Use the code examples above with your JWT token.

### **Step 4: Test Notification**
Import a trainee (if you're an admin) or ask an admin to create a notification for you.

---

## 🔍 6. Fetch Notifications from API

### **Get All Notifications**
```javascript
const jwtToken = localStorage.getItem('jwt_token');

fetch('http://localhost:5000/Notification', {
    method: 'GET',
    headers: {
        'Authorization': `Bearer ${jwtToken}`
    }
})
.then(res => res.json())
.then(data => {
    console.log('Your notifications:', data.data);
    // Display notifications in your UI
});
```

### **Get Only Unread Notifications**
```javascript
fetch('http://localhost:5000/Notification?unreadOnly=true', {
    method: 'GET',
    headers: {
        'Authorization': `Bearer ${jwtToken}`
    }
})
.then(res => res.json())
.then(data => {
    console.log('Unread notifications:', data.data);
});
```

### **Mark Notification as Read**
```javascript
const notificationId = 123; // Replace with actual ID

fetch(`http://localhost:5000/Notification/${notificationId}/mark-read`, {
    method: 'PUT',
    headers: {
        'Authorization': `Bearer ${jwtToken}`
    }
})
.then(res => res.json())
.then(data => {
    console.log('Marked as read:', data);
});
```

---

## 🐛 7. Troubleshooting

### **Problem: Connection fails with 401 Unauthorized**
**Solution:** 
- Check your JWT token is valid
- Make sure token is passed in `accessTokenFactory`
- Verify backend JWT configuration

### **Problem: Connection established but no notifications received**
**Solution:**
- Check backend logs: `"User {userId} connected to NotificationHub"`
- Verify you're listening to the correct event: `connection.on("ReceiveNotification", ...)`
- Check if notifications are being created in the database

### **Problem: Connection drops frequently**
**Solution:**
- Enable automatic reconnection: `.withAutomaticReconnect()`
- Check firewall/proxy settings
- Verify WebSocket is supported

### **Problem: "No notifications in database"**
**Solution:**
- Make sure `SaveChangesAsync()` is called after creating users
- Check if admins exist in the database (Role = "Admin")
- Verify notification service is registered in `Program.cs`

---

## 📊 8. Notification Data Structure

```javascript
{
    userId: "VJA2501",
    title: "New Trainees Imported",
    message: "admin@example.com imported 5 trainee(s): 4 succeeded, 1 failed.",
    timestamp: "2024-01-15T10:30:00Z",
    data: {
        type: "trainee_import",
        successCount: 4,
        failureCount: 1,
        performedBy: "admin@example.com"
    }
}
```

---

## ✅ Complete Working Example (HTML + JavaScript)

```html
<!DOCTYPE html>
<html>
<head>
    <title>SignalR Notifications</title>
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js"></script>
    <style>
        .notification { 
            border: 1px solid #ddd; 
            padding: 10px; 
            margin: 10px 0; 
            border-radius: 5px;
            background: #f9f9f9;
        }
        .notification.new { background: #e3f2fd; }
    </style>
</head>
<body>
    <h1>Real-Time Notifications</h1>
    <button onclick="connectToSignalR()">Connect</button>
    <button onclick="disconnectFromSignalR()">Disconnect</button>
    <button onclick="fetchNotifications()">Fetch All Notifications</button>
    <div id="status">Disconnected</div>
    <div id="notifications"></div>

    <script>
        let connection = null;
        const JWT_TOKEN = "YOUR_JWT_TOKEN_HERE"; // Replace this!

        function connectToSignalR() {
            connection = new signalR.HubConnectionBuilder()
                .withUrl("http://localhost:5000/hubs/notification", {
                    accessTokenFactory: () => JWT_TOKEN
                })
                .withAutomaticReconnect()
                .build();

            connection.on("ReceiveNotification", (data) => {
                console.log("📩 Notification:", data);
                addNotificationToUI(data);
            });

            connection.start()
                .then(() => {
                    document.getElementById('status').textContent = '✅ Connected';
                    console.log("Connected to SignalR");
                })
                .catch(err => {
                    document.getElementById('status').textContent = '❌ Failed: ' + err;
                    console.error(err);
                });
        }

        function disconnectFromSignalR() {
            if (connection) {
                connection.stop();
                document.getElementById('status').textContent = 'Disconnected';
            }
        }

        function addNotificationToUI(data) {
            const div = document.createElement('div');
            div.className = 'notification new';
            div.innerHTML = `
                <h4>${data.title}</h4>
                <p>${data.message}</p>
                <small>${new Date(data.timestamp).toLocaleString()}</small>
            `;
            document.getElementById('notifications').prepend(div);
        }

        function fetchNotifications() {
            fetch('http://localhost:5000/Notification', {
                headers: { 'Authorization': `Bearer ${JWT_TOKEN}` }
            })
            .then(res => res.json())
            .then(result => {
                console.log('Notifications from API:', result.data);
                result.data.forEach(notif => {
                    addNotificationToUI({
                        title: notif.title,
                        message: notif.message,
                        timestamp: notif.createdAt
                    });
                });
            })
            .catch(err => console.error('Fetch error:', err));
        }
    </script>
</body>
</html>
```

---

## 🎓 Summary

1. **Connect:** Use `HubConnectionBuilder` with JWT in `accessTokenFactory`
2. **Listen:** Subscribe to `ReceiveNotification` event
3. **Fetch:** Use `/Notification` API endpoint to get existing notifications
4. **Mark Read:** Use `/Notification/{id}/mark-read` to update status
5. **Test:** Login → Get JWT → Connect → Trigger notification (e.g., import trainee)

**That's it!** 🎉

