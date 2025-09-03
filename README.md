# 📝 Task Management System

A simple **Task Management System** built with:

- **Backend:** ASP.NET Core 8 (Web API) + Entity Framework Core + SQL Server + JWT Auth  
- **Frontend:** React + Axios + React Router  

Features:
- 🔑 Authentication (Login / Logout with JWT)  
- 👨‍💼 Admin Dashboard: view all tasks, create users, assign/update/delete tasks  
- 👨‍💻 Employee Dashboard: view & update own tasks  
- 📧 Password reset via email (forgot/reset flow)  

---

## 🚀 Run the Project

### 1. Backend (ASP.NET Core API)

```bash
cd TaskManagement.API
dotnet restore
dotnet build

Run database migrations:

cd ../TaskManagement.Infrastructure
dotnet ef migrations add InitialCreate -s ../TaskManagement.API
dotnet ef migrations add AddPasswordResetToken -s ../TaskManagement.API
dotnet ef database update -s ../TaskManagement.API

Start the API:

cd ../TaskManagement.API
dotnet run

By default it runs on https://localhost:7169

Open Swagger: https://localhost:7169/swagger/index.html

cd task-management-frontend
npm install
npm start

REACT_APP_API_BASE=https://localhost:7169/api

Then open: http://localhost:3000

Testing

Insert test users in SQL Server:

INSERT INTO Users (Username, Email, PasswordHash, Role)
VALUES ('admin1', 'admin@test.com', '1234', 'Admin');

INSERT INTO Users (Username, Email, PasswordHash, Role)
VALUES ('employee1', 'emp@test.com', '1234', 'Employee');

In Swagger:

Call POST /api/Auth/login with user credentials.

Copy the returned token.

Click Authorize 🔒 in Swagger → paste Bearer <token>

As Admin:

GET /api/Tasks → view all tasks

POST /api/Tasks → create task

DELETE /api/Tasks/{id} → delete task

POST /api/Auth/forgot-password → sends reset link (check Mailtrap or Gmail inbox)

As Employee:

GET /api/Tasks/my → view own tasks

PUT /api/Tasks/{id}/status → update own task status

Password Reset:

POST /api/Auth/forgot-password with { "email": "emp@test.com" }

Copy token from email (Mailtrap or Gmail)

Call POST /api/Auth/reset-password with:

{
  "token": "PASTE_TOKEN_HERE",
  "newPassword": "NewStrongPassword123!"
}


Notes

Update appsettings.Development.json with your SQL Server & email credentials.

Ensure SQL Server (or LocalDB) is running before dotnet run.

For Gmail SMTP you must use an App Password.

Frontend and backend are in the same repository (TaskManagementSystem/) for simplicity.



