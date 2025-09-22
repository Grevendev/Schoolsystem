# LearnPoint Console Application

A simple school management system built in **C#**, running in the console.  
It simulates roles such as **Admin**, **Teacher**, and **Student** with different permissions.  

---

## Features

### Login System
- Users log in with `username` and `password`.
- Supports **Admins**, **Teachers**, and **Students**.
- Tracks active sessions and logouts.

---

### Admin Features
Admins have full control over the system.

- **User Management**
  - Add new Admins, Teachers, and Students.
  - Remove Teachers or Students.
  - Edit user information.
  - Reset user passwords.
  - Search for users.
  - View all users (SHOWALL).

- **Schedule Management**
  - Create new schedule entries.
  - Update existing schedule entries.
  - Remove schedule entries.
  - View all schedules.

- **Statistics**
  - Show counts of Admins, Teachers, Students, and Schedules.
  - Grade distribution statistics.
  - Course statistics (number of sessions per course).
  - User activity statistics (logins).

- **Messaging**
  - Send notifications to:
    - A specific Admin
    - All Teachers
    - All Students
  - View system-wide messages.
  - Inbox to read personal messages.

- **Logs**
  - View system logs.
  - Save logs to a file (`system_logs.txt`).
  - Clear logs.

- **System Control**
  - Force logout of a user.
  - Backup and restore all system data.

---

### Teacher Features
Teachers can manage assignments and interact with students.

- **View Users**
  - See all Teachers (but not Admins).
  - See all Students.

- **Messaging**
  - Send messages to other Teachers.
  - Send messages to Students.
  - Inbox to read personal messages.

- **Assignments**
  - Create new assignments.
  - View student submissions.
  - Grade submissions with an `enum Grade { A, B, C, D, E, F }`.

- **Schedule**
  - View the schedule (cannot edit).

---

### Student Features
Students can view tasks, submit work, and see their own results.

- **View Users**
  - See all Students.
  - See all Teachers.

- **Messaging**
  - Inbox to read personal messages.

- **Assignments**
  - View available assignments.
  - Submit solutions for assignments.
  - View graded submissions.

- **Schedule**
  - View the schedule (cannot edit).

---

## Technology
- **C# 10+**
- Console Application
- Object-Oriented Design
- File handling for logs and backups

---

## How to Run
1. Clone or download the repository.
2. Open the project in Visual Studio or VS Code.
3. Run the program (`Program.cs`).
4. Log in using one of the default accounts:
   - **Admin**: `username: Edvin@hotmail.se`, `password: password`
   - **Teacher**: `username: Alex@hotmail.se`, `password: password`
   - **Student**: `username: Jarl@hotmail.se`, `password: password`

---

## Future Improvements
- Persistent storage using a database or JSON files.
- Improved error handling and input validation.
- GUI version of the system.
- Advanced reporting and analytics.

---

## Author
Created as a school management simulation project in C# Console.

---


## Password
Created so you can change password, but i made it so it can be tracked. I know it's not safe. But for this school project i wanted to find it easily. 
