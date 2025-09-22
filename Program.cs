///Användare
/// Resurser, Dolda baserade på rollen
/// Logga in
/// Logga ut 
/// Skapa en användare om man är en Admin
///  Se schema
///Lämna in uppgifter
// Få uppgifter rättade med betyg (betyg kan vara en ENUM) 

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Learnpoint;

//===LISTOR FÖR ANVÄNDARE OCH MEDDELANDE===

List<IUSer> users = new List<IUSer>();
users.Add(new Admin("Edvin@hotmail.se", "Edvin", "password"));
users.Add(new Teacher("Alex@hotmail.se", "Alex", "password"));
users.Add(new Student("Jarl@hotmail.se", "Jarl", "password"));
users.Add(new Student("Kiffle@hotmail.se", "Kiffle", "password"));

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
    Console.WriteLine("==LOGIN==");
    Console.WriteLine("username: ");
    string username = Console.ReadLine();

    Console.WriteLine("");
    Console.WriteLine("password: ");
    string _password = Console.ReadLine();

    foreach (IUSer user in users)
    {
      if (user.TryLogin(username, _password) && user.IsActive)
      {
        active_user = user;

        // Logga inräkning
        if (!loginCounts.ContainsKey(user.GetUsername()))
          loginCounts[user.GetUsername()] = 0;
        loginCounts[user.GetUsername()]++;
        activeSessions.Add(user.GetUsername());

        break;
      }
    }
    if (active_user == null)
    {
      Console.WriteLine("Login failed. Try again...");
      Thread.Sleep(1500);
    }
  }
  else
  {
    Console.Clear();
    switch (active_user.GetRole())
    {
      // ---------------- ADMIN MENU ----------------
      case Role.Admin:
        Console.WriteLine("Welcome Admin");
        Console.WriteLine("Options: ");
        Console.WriteLine("Admin          - go to Admin menu");
        Console.WriteLine("Teacher        - go to Teacher menu");
        Console.WriteLine("Student        - go to Student menu");
        Console.WriteLine("REMOVE         - remove a Teacher or Student directly");
        Console.WriteLine("SHOWALL        - show all users (Admins, Teachers, Students)");
        Console.WriteLine("CREATESCHEDULE - create a new schedule entry");
        Console.WriteLine("SCHEDULE       - show all schedules");
        Console.WriteLine("UPDATESCHEDULE - update a schedule entry");
        Console.WriteLine("REMOVESCHEDULE - remove a schedule entry");
        Console.WriteLine("ASSIGNTEACHER  - assign a teacher to a course");
        Console.WriteLine("ASSIGNSTUDENT  - assign a student to a course");
        Console.WriteLine("SEARCHUSER     - search for a user");
        Console.WriteLine("EDITUSER       - edit user info");
        Console.WriteLine("RESET          - reset a user's password");
        Console.WriteLine("CHANGEPASSWORD - change password for a user");
        Console.WriteLine("DEACTIVATE     - deactivate a user");
        Console.WriteLine("ACTIVATE       - activate a user");
        Console.WriteLine("STATS          - show basic stats");
        Console.WriteLine("GRADESTATS     - show grade distribution");
        Console.WriteLine("COURSESTATS    - show course stats");
        Console.WriteLine("ACTIVITYSTATS  - show user activity");
        Console.WriteLine("MESSAGE        - send notifications");
        Console.WriteLine("INBOX          - read your messages");
        Console.WriteLine("LOGS           - show system logs");
        Console.WriteLine("SAVELOGS       - save logs to file");
        Console.WriteLine("CLEARLOGS      - clear system logs");
        Console.WriteLine("VIEWMESSAGES   - view all messages in system");
        Console.WriteLine("FORCELOGOUT    - force logout a user");
        Console.WriteLine("BACKUP         - save backup of all data");
        Console.WriteLine("RESTORE        - restore backup");
        Console.WriteLine("Logout         - log out");

        string adminInput = Console.ReadLine();

        switch (adminInput)
        {
          case "ADD":
            Console.Write("Username: ");
            string aUser = Console.ReadLine();

            // ==VALIDATE==
            if (users.Any(u => u.GetUsername().Equals(aUser, StringComparison.OrdinalIgnoreCase)))
            {
              Console.WriteLine("Error: Username already exist!");
              Console.ReadLine();
              break;
            }
            Console.Write("Name: ");
            string aName = Console.ReadLine();
            Console.Write("Password: ");
            string aPass = Console.ReadLine();
            users.Add(new Admin(aUser, aName, aPass));
            Console.WriteLine("Admin added!");
            logs.Add($"Admin {active_user.GetUsername()} created new Admin {aUser}");
            Console.ReadLine();
            break;

          case "SHOW":
            Console.WriteLine("Admins:");
            foreach (IUSer u in users.Where(u => u.GetRole() == Role.Admin))
              u.Info();
            Console.ReadLine();
            break;

          case "SHOWALL":
            Console.WriteLine("All Users:");
            foreach (IUSer u in users) u.Info();
            Console.ReadLine();
            break;

          case "REMOVE":
            Console.Write("Enter username to remove: ");
            string rUser = Console.ReadLine();
            var rem = users.FirstOrDefault(u => u.GetUsername() == rUser && u.GetRole() != Role.Admin);
            if (rem != null)
            {
              users.Remove(rem);
              Console.WriteLine("User removed.");
              logs.Add($"Admin {active_user.GetUsername()} removed {rUser}");
            }
            else Console.WriteLine("User not found or cannot remove Admins.");
            Console.ReadLine();
            break;

          case "SEARCHUSER":
            Console.Write("Search username or name: ");
            string search = Console.ReadLine();
            var found = users.Where(u => u.GetUsername().Contains(search));
            foreach (var u in found) u.Info();
            Console.ReadLine();
            break;

          case "EDITUSER":
            Console.Write("Enter username to edit: ");
            string eUser = Console.ReadLine();
            var editTarget = users.FirstOrDefault(u => u.GetUsername() == eUser);
            if (editTarget != null)
            {
              Console.Write("New name: ");
              string newName = Console.ReadLine();
              if (editTarget is Admin adm) adm.Name = newName;
              if (editTarget is Teacher t) t.Name = newName;
              if (editTarget is Student s) s.Name = newName;
              Console.WriteLine("User updated!");
              logs.Add($"Admin {active_user.GetUsername()} updated {eUser}");
            }
            else Console.WriteLine("User not found.");
            Console.ReadLine();
            break;

          case "RESET":
            Console.Write("Enter username to reset password: ");
            string resetUser = Console.ReadLine();
            var resetTarget = users.FirstOrDefault(u => u.GetUsername() == resetUser);
            if (resetTarget != null)
            {
              Console.Write("New password: ");
              string newPass = Console.ReadLine();
              if (resetTarget is Admin ad) ad.SetPassword(newPass);
              if (resetTarget is Teacher te) te.SetPassword(newPass);
              if (resetTarget is Student st) st.SetPassword(newPass);
              Console.WriteLine("Password reset!");
              logs.Add($"Admin {active_user.GetUsername()} reset password for {resetUser}");
            }
            else Console.WriteLine("User not found.");
            Console.ReadLine();
            break;

          case "CHANGEPASSWORD":
            Console.Write("Enter username to change password: ");
            string cpUser = Console.ReadLine();
            var changeTarget = users.FirstOrDefault(u => u.GetUsername() == cpUser);

            if (changeTarget != null)
            {
              Console.Write("Enter new password: ");
              string newPass = Console.ReadLine();

              if (changeTarget is Admin ad)
              {
                ad.SetPassword(newPass);
              }
              else if (changeTarget is Teacher te)
              {
                te.SetPassword(newPass);
              }
              else if (changeTarget is Student st)
              {
                st.SetPassword(newPass);
              }

              Console.WriteLine("Password updated successfully!");
              logs.Add($"Admin {active_user.GetUsername()} changed password for {cpUser}");
            }
            else
            {
              Console.WriteLine("User not found.");
            }
            Console.ReadLine();
            break;

          case "DEACTIVATE":

            Console.Write("Enter username to deactivate:");
            string dUser = Console.ReadLine();
            var deactTarget = users.FirstOrDefault(u => u.GetUsername() == dUser && u.GetRole() != Role.Admin);
            if (deactTarget != null)
            {
              deactTarget.IsActive = false;
              Console.WriteLine($"{dUser} has been deactivated.");
              logs.Add($"Admin {active_user.GetUsername()} deactivated {dUser}");
            }
            else Console.WriteLine("User not found or cannot deactivate Admins");
            Console.ReadLine();
            break;

          case "ACTIVATE":
            Console.Write("Enter username to activate: ");
            string aUser2 = Console.ReadLine();
            var actTarget = users.FirstOrDefault(u => u.GetUsername() == aUser2);
            if (actTarget != null)
            {
              actTarget.IsActive = true;
              Console.WriteLine($"{aUser2} has been activated.");
              logs.Add($"Admin {active_user.GetUsername()} activated {aUser2}");
            }
            else Console.WriteLine("User not found.");
            Console.ReadLine();
            break;




          case "CREATESCHEDULE":
            Console.Write("Course: ");
            string cCourse = Console.ReadLine();
            Console.Write("Teacher: ");
            string cTeacher = Console.ReadLine();
            Console.Write("Day: ");
            string cDay = Console.ReadLine();
            Console.Write("Time: ");
            string cTime = Console.ReadLine();
            schedules.Add(new Schedule(cCourse, cTeacher, cDay, cTime));
            Console.WriteLine("Schedule entry created!");
            logs.Add($"Admin {active_user.GetUsername()} created schedule {cCourse} {cDay} {cTime}");
            Console.ReadLine();
            break;

          case "SCHEDULE":
            Console.WriteLine("Schedules:");
            foreach (var sch in schedules) sch.Show();
            Console.ReadLine();
            break;

          case "UPDATESCHEDULE":
            Console.Write("Enter course to update: ");
            string uCourse = Console.ReadLine();
            var targetSch = schedules.FirstOrDefault(sch => sch.Course == uCourse);
            if (targetSch != null)
            {
              Console.Write("New Day: ");
              targetSch.Day = Console.ReadLine();
              Console.Write("New Time: ");
              targetSch.Time = Console.ReadLine();
              Console.WriteLine("Schedule updated!");
              logs.Add($"Admin {active_user.GetUsername()} updated schedule {uCourse}");
            }
            else Console.WriteLine("Schedule not found.");
            Console.ReadLine();
            break;

          case "REMOVESCHEDULE":
            Console.Write("Enter course to remove schedule: ");
            string dCourse = Console.ReadLine();
            var remSch = schedules.FirstOrDefault(sch => sch.Course == dCourse);
            if (remSch != null)
            {
              schedules.Remove(remSch);
              Console.WriteLine("Schedule removed!");
              logs.Add($"Admin {active_user.GetUsername()} removed schedule {dCourse}");
            }
            else Console.WriteLine("Schedule not found.");
            Console.ReadLine();
            break;

          case "STATS":
            Console.WriteLine($"Admins: {users.Count(u => u.GetRole() == Role.Admin)}");
            Console.WriteLine($"Teachers: {users.Count(u => u.GetRole() == Role.Teacher)}");
            Console.WriteLine($"Students: {users.Count(u => u.GetRole() == Role.Student)}");
            Console.WriteLine($"Schedules: {schedules.Count}");
            Console.ReadLine();
            break;

          case "GRADESTATS":
            Console.WriteLine("Grade distribution:");
            foreach (Grade g in Enum.GetValues(typeof(Grade)))
              Console.WriteLine($"{g}: {assignments.Count(a => a.Grade == g)}");
            Console.ReadLine();
            break;

          case "COURSESTATS":
            Console.WriteLine("Courses:");
            foreach (var group in schedules.GroupBy(s => s.Course))
              Console.WriteLine($"{group.Key}: {group.Count()} sessions, Teacher: {group.First().Teacher}");
            Console.ReadLine();
            break;

          case "ACTIVITYSTATS":
            Console.WriteLine("Logins per user:");
            foreach (var u in loginCounts)
              Console.WriteLine($"{u.Key}: {u.Value} logins");
            Console.ReadLine();
            break;

          case "LOGS":
            Console.WriteLine("System Logs:");
            foreach (var log in logs) Console.WriteLine(log);
            Console.ReadLine();
            break;

          case "SAVELOGS":
            File.WriteAllLines("system_logs.txt", logs);
            Console.WriteLine("Logs saved to system_logs.txt");
            Console.ReadLine();
            break;

          case "CLEARLOGS":
            logs.Clear();
            Console.WriteLine("Logs cleared.");
            Console.ReadLine();
            break;

          case "VIEWMESSAGES":
            Console.WriteLine("All Messages in system:");
            foreach (var msg in messages) msg.Show();
            Console.ReadLine();
            break;

          case "FORCELOGOUT":
            Console.Write("Enter username to force logout: ");
            string flUser = Console.ReadLine();
            if (activeSessions.Contains(flUser))
            {
              activeSessions.Remove(flUser);
              Console.WriteLine($"{flUser} has been logged out.");
              logs.Add($"Admin {active_user.GetUsername()} forced logout of {flUser}");
            }
            else Console.WriteLine("User not active.");
            Console.ReadLine();
            break;

          case "BACKUP":
            BackupSystem(users, schedules, assignments, submissions, messages, logs);
            Console.WriteLine("Backup saved.");
            Console.ReadLine();
            break;

          case "RESTORE":
            RestoreSystem(out users, out schedules, out assignments, out submissions, out messages, out logs);
            Console.WriteLine("System restored.");
            Console.ReadLine();
            break;

          case "MESSAGE":
            Console.WriteLine("Who do you want to notify?");
            Console.WriteLine("1. Specific Admin");
            Console.WriteLine("2. All Teachers");
            Console.WriteLine("3. All Students");
            string msgChoice = Console.ReadLine();

            switch (msgChoice)
            {
              case "1":
                Console.Write("Enter Admin username: ");
                string adminTarget = Console.ReadLine();
                Console.Write("Message: ");
                string adminMsg = Console.ReadLine();
                var targetAdmin = users.FirstOrDefault(u => u.GetUsername() == adminTarget && u.GetRole() == Role.Admin);
                if (targetAdmin != null)
                {
                  messages.Add(new Message(active_user.GetUsername(), adminTarget, adminMsg));
                  Console.WriteLine("Message sent to Admin!");
                }
                else Console.WriteLine("Admin not found.");
                break;

              case "2":
                Console.Write("Message for all Teachers: ");
                string teacherMsg = Console.ReadLine();
                foreach (var teacher in users.Where(u => u.GetRole() == Role.Teacher))
                  messages.Add(new Message(active_user.GetUsername(), teacher.GetUsername(), teacherMsg));
                Console.WriteLine("Message sent to all Teachers!");
                break;

              case "3":
                Console.Write("Message for all Students: ");
                string studentMsg = Console.ReadLine();
                foreach (var student in users.Where(u => u.GetRole() == Role.Student))
                  messages.Add(new Message(active_user.GetUsername(), student.GetUsername(), studentMsg));
                Console.WriteLine("Message sent to all Students!");
                break;
            }
            Console.ReadLine();
            break;

          case "INBOX":
            Console.WriteLine("Your messages: ");
            foreach (var m in messages.Where(m => m.To == active_user.GetUsername()))
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

      // ---------------- TEACHER MENU ----------------
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

        string teacherInput = Console.ReadLine();
        switch (teacherInput)
        {
          case "SHOWTEACHERS":
            foreach (var t in users.Where(u => u.GetRole() == Role.Teacher))
              t.Info();
            Console.ReadLine();
            break;

          case "SHOWSTUDENTS":
            foreach (var s in users.Where(u => u.GetRole() == Role.Student))
              s.Info();
            Console.ReadLine();
            break;

          case "SHOWSCHEDULE":
            foreach (var sch in schedules.Where(s => s.Teacher == active_user.GetUsername()))
              sch.Show();
            Console.ReadLine();
            break;

          case "ASSIGNMENTS":
            Console.WriteLine("Submissions:");
            foreach (var sub in submissions)
              sub.Show();

            Console.Write("Enter student username to grade: ");
            string stuU = Console.ReadLine();
            var targetSub = submissions.FirstOrDefault(s => s.StudentUsername == stuU);
            if (targetSub != null)
            {
              Console.Write("Enter grade (A-F): ");
              string g = Console.ReadLine();
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
            string to = Console.ReadLine();
            Console.Write("Message: ");
            string msg = Console.ReadLine();
            messages.Add(new Message(active_user.GetUsername(), to, msg));
            Console.WriteLine("Message sent!");
            Console.ReadLine();
            break;

          case "INBOX":
            foreach (var m in messages.Where(m => m.To == active_user.GetUsername()))
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

      // ---------------- STUDENT MENU ----------------
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

        string stuInput = Console.ReadLine();
        switch (stuInput)
        {
          case "SUBMIT":
            Console.Write("Assignment Title: ");
            string aTitle = Console.ReadLine();
            Console.Write("Content: ");
            string aContent = Console.ReadLine();
            submissions.Add(new Submission(active_user.GetUsername(), aTitle, aContent));
            Console.WriteLine("Submitted!");
            Console.ReadLine();
            break;

          case "MYSUBS":
            foreach (var sub in submissions.Where(s => s.StudentUsername == active_user.GetUsername()))
              sub.Show();
            Console.ReadLine();
            break;

          case "SHOWSCHEDULE":
            foreach (var sch in schedules)
              sch.Show();
            Console.ReadLine();
            break;

          case "SHOWTEACHERS":
            foreach (var t in users.Where(u => u.GetRole() == Role.Teacher))
              t.Info();
            Console.ReadLine();
            break;

          case "MESSAGE":
            Console.Write("Message to Teacher: ");
            string toT = Console.ReadLine();
            Console.Write("Message: ");
            string msgT = Console.ReadLine();
            messages.Add(new Message(active_user.GetUsername(), toT, msgT));
            Console.WriteLine("Message sent!");
            Console.ReadLine();
            break;

          case "INBOX":
            foreach (var m in messages.Where(m => m.To == active_user.GetUsername()))
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
    foreach (var u in users) sw.WriteLine($"USER|{u.GetRole()}|{u.GetUsername()}|{u.GetName()}|{u.GetPassword()}");
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
        if (r == Role.Admin) users.Add(new Admin(parts[2], parts[3], parts[4]));
        if (r == Role.Teacher) users.Add(new Teacher(parts[2], parts[3], parts[4]));
        if (r == Role.Student) users.Add(new Student(parts[2], parts[3], parts[4]));
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
