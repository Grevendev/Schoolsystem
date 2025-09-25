using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Learnpoint;

//===LISTOR FÖR ANVÄNDARE OCH MEDDELANDE===
List<IUSer> users = new List<IUSer>
{
    new Admin("Edvin@hotmail.se", "Edvin", "password"),
    new Teacher("Alex@hotmail.se", "Alex", "password"),
    new Student("Jarl@hotmail.se", "Jarl", "password"),
    new Student("Kiffle@hotmail.se", "Kiffle", "password")
};

List<Message> messages = new List<Message>();
List<Assignment> assignments = new List<Assignment>();
List<Submission> submissions = new List<Submission>();
List<Schedule> schedules = new List<Schedule>();
List<string> logs = new List<string>();
Dictionary<string, int> loginCounts = new Dictionary<string, int>();
HashSet<string> activeSessions = new HashSet<string>();

IUSer? active_user = null;

bool running = true;
while (running)
{
  if (active_user == null)
  {
    Console.Clear();
    Console.WriteLine("===LOGIN===");
    Console.Write("username: ");
    string username = Console.ReadLine() ?? string.Empty;
    Console.Write("password: ");
    string _password = Console.ReadLine() ?? string.Empty;

    var user = users.FirstOrDefault(u => u.GetUsername().Equals(username, StringComparison.OrdinalIgnoreCase));

    if (user == null)
    {
      Console.WriteLine("User not found.");
      Thread.Sleep(1500);
      continue;
    }

    if (!user.IsActive)
    {
      Console.WriteLine("Account is deactivated. Contact admin.");
      Thread.Sleep(2000);
      continue;
    }

    if (user.TryLogin(username, _password))
    {
      user.FailedLogins = 0;
      if (user.MustChangePassword)
      {
        Console.WriteLine("You must change your password before continuing.");
        Console.Write("New password: ");
        string newPass = Console.ReadLine() ?? string.Empty;
        user.SetPassword(newPass);
        user.MustChangePassword = false;
        Console.WriteLine("Password updated successfully!");
        Thread.Sleep(1500);
      }
      active_user = user;

      if (!loginCounts.ContainsKey(user.GetUsername()))
        loginCounts[user.GetUsername()] = 0;
      loginCounts[user.GetUsername()]++;
      activeSessions.Add(user.GetUsername());
    }
    else
    {
      user.FailedLogins++;
      if (user.FailedLogins >= 3)
      {
        user.IsActive = false;
        Console.WriteLine("Account locked after 3 failed login attempts. Contact admin.");
      }
      else
      {
        Console.WriteLine($"Login failed. {3 - user.FailedLogins} attempts remaining...");
      }
      Thread.Sleep(2000);
    }
  }
  else
  {
    Console.Clear();
    switch (active_user.GetRole())
    {
      case Role.Admin:
        bool adminMenuActive = true;
        while (adminMenuActive)
        {
          Console.Clear();
          Console.WriteLine("=== Welcome Admin ===");
          Console.WriteLine("1. User Management");
          Console.WriteLine("2. Schedule Management");
          Console.WriteLine("3. Assignment & Grades");
          Console.WriteLine("4. Messaging");
          Console.WriteLine("5. System & Logs");
          Console.WriteLine("6. Account Settings");
          Console.WriteLine("7. Logout");
          Console.Write("Select an option (1-7): ");
          string mainChoice = Console.ReadLine() ?? string.Empty;

          switch (mainChoice)
          {
            case "1":
              // --- User Management ---
              Console.Clear();
              Console.WriteLine("=== User Management ===");
              Console.WriteLine("a. Show all users");
              Console.WriteLine("b. Add Admin/Teacher/Student");
              Console.WriteLine("c. Remove a user");
              Console.WriteLine("d. Search user");
              Console.WriteLine("e. Edit user info");
              Console.WriteLine("f. Reset password");
              Console.WriteLine("g. Change user password");
              Console.WriteLine("h. Deactivate / Activate user");
              Console.WriteLine("i. Force change password");
              Console.WriteLine("j. Unlock account");
              Console.WriteLine("x. Back to main menu");
              Console.Write("Select an option: ");
              string userChoice = Console.ReadLine() ?? string.Empty;
              switch (userChoice)
              {
                case "a":
                  foreach (var u in users) u.Info();
                  Console.ReadLine();
                  break;

                case "b":
                  Console.Write("Username: ");
                  string newUser = Console.ReadLine() ?? string.Empty;
                  if (users.Any(u => u.GetUsername().Equals(newUser, StringComparison.OrdinalIgnoreCase)))
                  {
                    Console.WriteLine("Error: Username already exists!");
                    Console.ReadLine();
                    break;
                  }
                  Console.Write("Name: ");
                  string newName = Console.ReadLine() ?? string.Empty;
                  Console.Write("Role (Admin/Teacher/Student): ");
                  string roleInput = Console.ReadLine() ?? string.Empty;
                  Console.Write("Password: ");
                  string newPass = Console.ReadLine() ?? string.Empty;
                  if (roleInput.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    users.Add(new Admin(newUser, newName, newPass));
                  else if (roleInput.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
                    users.Add(new Teacher(newUser, newName, newPass));
                  else if (roleInput.Equals("Student", StringComparison.OrdinalIgnoreCase))
                    users.Add(new Student(newUser, newName, newPass));
                  Console.WriteLine($"{roleInput} added!");
                  if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} created {roleInput} {newUser}");
                  Console.ReadLine();
                  break;

                case "c":
                  Console.Write("Enter username to remove: ");
                  string rUser = Console.ReadLine() ?? string.Empty;
                  var rem = users.FirstOrDefault(u => u.GetUsername() == rUser && u.GetRole() != Role.Admin);
                  if (rem != null)
                  {
                    users.Remove(rem);
                    Console.WriteLine("User removed.");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} removed {rUser}");
                  }
                  else Console.WriteLine("User not found or cannot remove Admins.");
                  Console.ReadLine();
                  break;

                case "d":
                  Console.Write("Search username or name: ");
                  string search = Console.ReadLine() ?? string.Empty;
                  var found = users.Where(u => u.GetUsername().Contains(search, StringComparison.OrdinalIgnoreCase));
                  foreach (var u in found) u.Info();
                  Console.ReadLine();
                  break;

                case "e":
                  Console.Write("Enter username to edit: ");
                  string eUser = Console.ReadLine() ?? string.Empty;
                  var editTarget = users.FirstOrDefault(u => u.GetUsername() == eUser);
                  if (editTarget != null)
                  {
                    Console.Write("New name: ");
                    string newNameEdit = Console.ReadLine() ?? string.Empty;
                    if (editTarget is Admin adm) adm.Name = newNameEdit;
                    if (editTarget is Teacher t) t.Name = newNameEdit;
                    if (editTarget is Student s) s.Name = newNameEdit;
                    Console.WriteLine("User updated!");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} updated {eUser}");
                  }
                  else Console.WriteLine("User not found.");
                  Console.ReadLine();
                  break;

                case "f":
                  Console.Write("Enter username to reset password: ");
                  string resetUser = Console.ReadLine() ?? string.Empty;
                  var resetTarget = users.FirstOrDefault(u => u.GetUsername() == resetUser);
                  if (resetTarget != null)
                  {
                    Console.Write("New password: ");
                    string newPassReset = Console.ReadLine() ?? string.Empty;
                    resetTarget.SetPassword(newPassReset);
                    Console.WriteLine("Password reset!");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} reset password for {resetUser}");
                  }
                  else Console.WriteLine("User not found.");
                  Console.ReadLine();
                  break;

                case "g":
                  Console.Write("Enter username to change password: ");
                  string cpUser = Console.ReadLine() ?? string.Empty;
                  var changeTarget = users.FirstOrDefault(u => u.GetUsername() == cpUser);
                  if (changeTarget != null)
                  {
                    Console.Write("Enter new password: ");
                    string newPassChange = Console.ReadLine() ?? string.Empty;
                    changeTarget.SetPassword(newPassChange);
                    Console.WriteLine("Password updated successfully!");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} changed password for {cpUser}");
                  }
                  else Console.WriteLine("User not found.");
                  Console.ReadLine();
                  break;

                case "h":
                  Console.Write("Enter username to deactivate or activate: ");
                  string dUser = Console.ReadLine() ?? string.Empty;
                  var deactTarget = users.FirstOrDefault(u => u.GetUsername() == dUser && u.GetRole() != Role.Admin);
                  if (deactTarget != null)
                  {
                    deactTarget.IsActive = !deactTarget.IsActive;
                    string status = deactTarget.IsActive ? "activated" : "deactivated";
                    Console.WriteLine($"{dUser} has been {status}.");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} {status} {dUser}");
                  }
                  else Console.WriteLine("User not found or cannot modify Admins.");
                  Console.ReadLine();
                  break;

                case "i":
                  Console.Write("Enter username to force change password: ");
                  string fcUser = Console.ReadLine() ?? string.Empty;
                  var fcTarget = users.FirstOrDefault(u => u.GetUsername() == fcUser);
                  if (fcTarget != null)
                  {
                    fcTarget.MustChangePassword = true;
                    Console.WriteLine($"{fcUser} must change password on next login.");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} forced password change for {fcUser}");
                  }
                  else Console.WriteLine("User not found.");
                  Console.ReadLine();
                  break;

                case "j":
                  Console.Write("Enter username to unlock: ");
                  string unlockUser = Console.ReadLine() ?? string.Empty;
                  var unlockTarget = users.FirstOrDefault(u => u.GetUsername() == unlockUser);
                  if (unlockTarget != null)
                  {
                    unlockTarget.IsActive = true;
                    unlockTarget.FailedLogins = 0;
                    unlockTarget.MustChangePassword = true;
                    Console.WriteLine($"{unlockUser} has been unlocked and must change password next login.");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} unlocked account {unlockUser}");
                  }
                  else Console.WriteLine("User not found.");
                  Console.ReadLine();
                  break;

                case "x":
                  break;
              }
              break;

            case "2":
              // Schedule Management
              Console.Clear();
              Console.WriteLine("=== Schedule Management ===");
              Console.WriteLine("a. Create schedule entry");
              Console.WriteLine("b. Show schedules");
              Console.WriteLine("c. Update schedule");
              Console.WriteLine("d. Remove schedule");
              Console.WriteLine("e. Assign teacher/student");
              Console.WriteLine("x. Back to main menu");
              Console.Write("Select an option: ");
              string scheduleChoice = Console.ReadLine() ?? string.Empty;
              switch (scheduleChoice)
              {
                case "a":
                  Console.Write("Course: ");
                  string cCourse = Console.ReadLine() ?? string.Empty;
                  Console.Write("Teacher: ");
                  string cTeacher = Console.ReadLine() ?? string.Empty;
                  Console.Write("Day: ");
                  string cDay = Console.ReadLine() ?? string.Empty;
                  Console.Write("Time: ");
                  string cTime = Console.ReadLine() ?? string.Empty;
                  schedules.Add(new Schedule(cCourse, cTeacher, cDay, cTime));
                  Console.WriteLine("Schedule entry created!");
                  if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} created schedule {cCourse} {cDay} {cTime}");
                  Console.ReadLine();
                  break;

                case "b":
                  foreach (var sch in schedules) sch.Show();
                  Console.ReadLine();
                  break;

                case "c":
                  Console.Write("Enter course to update: ");
                  string uCourse = Console.ReadLine() ?? string.Empty;
                  var targetSch = schedules.FirstOrDefault(sch => sch.Course == uCourse);
                  if (targetSch != null)
                  {
                    Console.Write("New Day: ");
                    targetSch.Day = Console.ReadLine() ?? string.Empty;
                    Console.Write("New Time: ");
                    targetSch.Time = Console.ReadLine() ?? string.Empty;
                    Console.WriteLine("Schedule updated!");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} updated schedule {uCourse}");
                  }
                  else Console.WriteLine("Schedule not found.");
                  Console.ReadLine();
                  break;

                case "d":
                  Console.Write("Enter course to remove schedule: ");
                  string dCourse = Console.ReadLine() ?? string.Empty;
                  var remSch = schedules.FirstOrDefault(sch => sch.Course == dCourse);
                  if (remSch != null)
                  {
                    schedules.Remove(remSch);
                    Console.WriteLine("Schedule removed!");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} removed schedule {dCourse}");
                  }
                  else Console.WriteLine("Schedule not found.");
                  Console.ReadLine();
                  break;

                case "e":
                  Console.Write("Assign teacher/student to course (format: course,username): ");
                  string assignInput = Console.ReadLine() ?? string.Empty;
                  string[] parts = assignInput.Split(',');
                  if (parts.Length == 2)
                  {
                    string course = parts[0];
                    string userAssign = parts[1];
                    var schedule = schedules.FirstOrDefault(s => s.Course == course);
                    var userObj = users.FirstOrDefault(u => u.GetUsername() == userAssign);
                    if (schedule != null && userObj != null)
                    {
                      if (userObj.GetRole() == Role.Teacher)
                        schedule.Teacher = userAssign;
                      Console.WriteLine("Assignment done!");
                    }
                  }
                  Console.ReadLine();
                  break;

                case "x":
                  break;
              }
              break;

            case "3":
              // Assignment & Grades
              Console.Clear();
              Console.WriteLine("=== Assignment & Grades ===");
              Console.WriteLine("a. Show grade distribution");
              Console.WriteLine("b. Show course stats");
              Console.WriteLine("c. Show user activity");
              Console.WriteLine("x. Back to main menu");
              Console.Write("Select an option: ");
              string statsChoice = Console.ReadLine() ?? string.Empty;
              switch (statsChoice)
              {
                case "a":
                  foreach (Grade g in Enum.GetValues(typeof(Grade)))
                    Console.WriteLine($"{g}: {assignments.Count(a => a.Grade == g)}");
                  Console.ReadLine();
                  break;

                case "b":
                  foreach (var group in schedules.GroupBy(s => s.Course))
                    Console.WriteLine($"{group.Key}: {group.Count()} sessions, Teacher: {group.FirstOrDefault()?.Teacher ?? "N/A"}");
                  Console.ReadLine();
                  break;

                case "c":
                  foreach (var u in loginCounts)
                    Console.WriteLine($"{u.Key}: {u.Value} logins");
                  Console.ReadLine();
                  break;

                case "x":
                  break;
              }
              break;

            case "4":
              // Messaging
              Console.Clear();
              Console.WriteLine("=== Messaging ===");
              Console.WriteLine("a. Send notifications");
              Console.WriteLine("b. Read your inbox");
              Console.WriteLine("c. View all messages");
              Console.WriteLine("x. Back to main menu");
              Console.Write("Select an option: ");
              string msgChoice = Console.ReadLine() ?? string.Empty;
              switch (msgChoice)
              {
                case "a":
                  Console.Write("Message to (username): ");
                  string to = Console.ReadLine() ?? string.Empty;
                  Console.Write("Message: ");
                  string msg = Console.ReadLine() ?? string.Empty;
                  if (active_user != null) messages.Add(new Message(active_user.GetUsername(), to, msg));
                  Console.WriteLine("Message sent!");
                  Console.ReadLine();
                  break;

                case "b":
                  foreach (var m in messages.Where(m => m.To == (active_user?.GetUsername() ?? string.Empty)))
                    m.Show();
                  Console.ReadLine();
                  break;

                case "c":
                  foreach (var m in messages) m.Show();
                  Console.ReadLine();
                  break;

                case "x":
                  break;
              }
              break;

            case "5":
              // System & Logs
              Console.Clear();
              Console.WriteLine("=== System & Logs ===");
              Console.WriteLine("a. Show logs");
              Console.WriteLine("b. Save logs");
              Console.WriteLine("c. Clear logs");
              Console.WriteLine("d. Backup system");
              Console.WriteLine("e. Restore system");
              Console.WriteLine("f. Force logout user");
              Console.WriteLine("x. Back to main menu");
              Console.Write("Select an option: ");
              string sysChoice = Console.ReadLine() ?? string.Empty;
              switch (sysChoice)
              {
                case "a":
                  foreach (var log in logs) Console.WriteLine(log);
                  Console.ReadLine();
                  break;

                case "b":
                  File.WriteAllLines("system_logs.txt", logs);
                  Console.WriteLine("Logs saved to system_logs.txt");
                  Console.ReadLine();
                  break;

                case "c":
                  logs.Clear();
                  Console.WriteLine("Logs cleared.");
                  Console.ReadLine();
                  break;

                case "d":
                  BackupSystem(users, schedules, assignments, submissions, messages, logs);
                  Console.WriteLine("Backup saved.");
                  Console.ReadLine();
                  break;

                case "e":
                  RestoreSystem(out users, out schedules, out assignments, out submissions, out messages, out logs);
                  Console.WriteLine("System restored.");
                  Console.ReadLine();
                  break;

                case "f":
                  Console.Write("Enter username to force logout: ");
                  string flUser = Console.ReadLine() ?? string.Empty;
                  if (activeSessions.Contains(flUser))
                  {
                    activeSessions.Remove(flUser);
                    Console.WriteLine($"{flUser} has been logged out.");
                    if (active_user != null) logs.Add($"Admin {active_user.GetUsername()} forced logout of {flUser}");
                  }
                  else Console.WriteLine("User not active.");
                  Console.ReadLine();
                  break;

                case "x":
                  break;
              }
              break;

            case "6":
              // Account settings
              Console.Clear();
              Console.WriteLine("=== Account Settings ===");
              Console.WriteLine("a. Show changed passwords");
              Console.WriteLine("b. Logout");
              Console.WriteLine("x. Back to main menu");
              Console.Write("Select an option: ");
              string accChoice = Console.ReadLine() ?? string.Empty;
              switch (accChoice)
              {
                case "a":
                  foreach (var u in users.Where(u => u.MustChangePassword))
                    Console.WriteLine(u.GetUsername());
                  Console.ReadLine();
                  break;

                case "b":
                  adminMenuActive = false;
                  active_user = null;
                  break;

                case "x":
                  break;
              }
              break;

            case "7":
              adminMenuActive = false;
              active_user = null;
              break;
          }
        }
        break;

      case Role.Teacher:
        Console.WriteLine("Welcome Teacher");
        Console.WriteLine("Options: ");
        Console.WriteLine("SHOWTEACHERS - see all teachers");
        Console.WriteLine("SHOWSTUDENTS - see all students");
        Console.WriteLine("SHOWSCHEDULE - see schedule");
        Console.WriteLine("ASSIGNMENTS  - see student submissions and grade them");
        Console.WriteLine("MESSAGE      - send notifications");
        Console.WriteLine("INBOX        - read your messages");
        Console.WriteLine("Logout       - log out");

        string teacherInput = Console.ReadLine() ?? string.Empty;
        switch (teacherInput)
        {
          case "SHOWTEACHERS":
            foreach (var t in users.Where(u => u.GetRole() == Role.Teacher)) t.Info();
            Console.ReadLine();
            break;

          case "SHOWSTUDENTS":
            foreach (var s in users.Where(u => u.GetRole() == Role.Student)) s.Info();
            Console.ReadLine();
            break;

          case "SHOWSCHEDULE":
            foreach (var sch in schedules.Where(s => s.Teacher == (active_user?.GetUsername() ?? string.Empty)))
              sch.Show();
            Console.ReadLine();
            break;

          case "ASSIGNMENTS":
            Console.WriteLine("Submissions:");
            foreach (var sub in submissions) sub.Show();
            Console.Write("Enter student username to grade: ");
            string stuU = Console.ReadLine() ?? string.Empty;
            var targetSub = submissions.FirstOrDefault(s => s.StudentUsername == stuU);
            if (targetSub != null)
            {
              Console.Write("Enter grade (A-F): ");
              string g = Console.ReadLine() ?? string.Empty;
              if (Enum.TryParse<Grade>(g, out Grade gr))
              {
                targetSub.Grade = gr;
                Console.WriteLine("Graded!");
              }
            }
            Console.ReadLine();
            break;

          case "MESSAGE":
            Console.Write("Message to (Teacher/Student username): ");
            string toMsg = Console.ReadLine() ?? string.Empty;
            Console.Write("Message: ");
            string msgContent = Console.ReadLine() ?? string.Empty;
            if (active_user != null) messages.Add(new Message(active_user.GetUsername(), toMsg, msgContent));
            Console.WriteLine("Message sent!");
            Console.ReadLine();
            break;

          case "INBOX":
            foreach (var m in messages.Where(m => m.To == (active_user?.GetUsername() ?? string.Empty)))
              m.Show();
            Console.ReadLine();
            break;

          case "Logout":
            active_user = null;
            Console.WriteLine("You have logged out.");
            Console.ReadLine();
            break;
        }
        break;

      case Role.Student:
        Console.WriteLine("Welcome Student");
        Console.WriteLine("Options: ");
        Console.WriteLine("SUBMIT       - submit assignment");
        Console.WriteLine("MYSUBS       - see your submissions & grades");
        Console.WriteLine("SHOWSCHEDULE - see your schedule");
        Console.WriteLine("SHOWTEACHERS - see all teachers");
        Console.WriteLine("MESSAGE      - send messages");
        Console.WriteLine("INBOX        - read your messages");
        Console.WriteLine("Logout       - log out");

        string stuInput = Console.ReadLine() ?? string.Empty;
        switch (stuInput)
        {
          case "SUBMIT":
            Console.Write("Assignment Title: ");
            string aTitle = Console.ReadLine() ?? string.Empty;
            Console.Write("Content: ");
            string aContent = Console.ReadLine() ?? string.Empty;
            if (active_user != null) submissions.Add(new Submission(active_user.GetUsername(), aTitle, aContent));
            Console.WriteLine("Submitted!");
            Console.ReadLine();
            break;

          case "MYSUBS":
            foreach (var sub in submissions.Where(s => s.StudentUsername == (active_user?.GetUsername() ?? string.Empty)))
              sub.Show();
            Console.ReadLine();
            break;

          case "SHOWSCHEDULE":
            foreach (var sch in schedules) sch.Show();
            Console.ReadLine();
            break;

          case "SHOWTEACHERS":
            foreach (var t in users.Where(u => u.GetRole() == Role.Teacher)) t.Info();
            Console.ReadLine();
            break;

          case "MESSAGE":
            Console.Write("Message to Teacher: ");
            string toT = Console.ReadLine() ?? string.Empty;
            Console.Write("Message: ");
            string msgT = Console.ReadLine() ?? string.Empty;
            if (active_user != null) messages.Add(new Message(active_user.GetUsername(), toT, msgT));
            Console.WriteLine("Message sent!");
            Console.ReadLine();
            break;

          case "INBOX":
            foreach (var m in messages.Where(m => m.To == (active_user?.GetUsername() ?? string.Empty)))
              m.Show();
            Console.ReadLine();
            break;

          case "Logout":
            active_user = null;
            Console.WriteLine("You have logged out.");
            Console.ReadLine();
            break;
        }
        break;
    }
  }
}

// --- BACKUP & RESTORE ---
void BackupSystem(List<IUSer> users, List<Schedule> schedules, List<Assignment> assignments, List<Submission> submissions, List<Message> messages, List<string> logs)
{
  using (StreamWriter sw = new StreamWriter("backup.txt"))
  {
    foreach (var u in users) sw.WriteLine($"USER|{u.GetRole()}|{u.GetUsername()}|{u.GetName()}");
    foreach (var s in schedules) sw.WriteLine($"SCHEDULE|{s.Course}|{s.Teacher}|{s.Day}|{s.Time}");
    foreach (var a in assignments) sw.WriteLine($"ASSIGN|{a.Title}|{a.Grade}");
    foreach (var sub in submissions) sw.WriteLine($"SUB|{sub.StudentUsername}|{sub.AssignmentTitle}|{sub.Content}|{sub.Grade}");
    foreach (var m in messages) sw.WriteLine($"MSG|{m.From}|{m.To}|{m.Content}");
    foreach (var l in logs) sw.WriteLine($"LOG|{l}");
  }
}

void RestoreSystem(out List<IUSer> users, out List<Schedule> schedules, out List<Assignment> assignments, out List<Submission> submissions, out List<Message> messages, out List<string> logs)
{
  users = new List<IUSer>();
  schedules = new List<Schedule>();
  assignments = new List<Assignment>();
  submissions = new List<Submission>();
  messages = new List<Message>();
  logs = new List<string>();

  if (!File.Exists("backup.txt")) return;

  foreach (var line in File.ReadAllLines("backup.txt"))
  {
    var parts = line.Split('|');
    switch (parts[0])
    {
      case "USER":
        Role r = Enum.Parse<Role>(parts[1]);
        string username = parts[2];
        string name = parts[3];
        string defaultPass = "changeme";
        if (r == Role.Admin) users.Add(new Admin(username, name, defaultPass));
        if (r == Role.Teacher) users.Add(new Teacher(username, name, defaultPass));
        if (r == Role.Student) users.Add(new Student(username, name, defaultPass));
        break;
      case "SCHEDULE":
        schedules.Add(new Schedule(parts[1], parts[2], parts[3], parts[4]));
        break;
      case "ASSIGN":
        assignments.Add(new Assignment(parts[1]) { Grade = Enum.Parse<Grade>(parts[2]) });
        break;
      case "SUB":
        submissions.Add(new Submission(parts[1], parts[2], parts[3]) { Grade = Enum.Parse<Grade>(parts[4]) });
        break;
      case "MSG":
        messages.Add(new Message(parts[1], parts[2], parts[3]));
        break;
      case "LOG":
        logs.Add(parts[1]);
        break;
    }
  }
}
