///Användare
/// Resurser, Dolda baserade på rollen
/// Logga in
/// Logga ut 
/// Skapa en användare om man är en Admin
///  Se schema
///Lämna in uppgifter
// Få uppgifter rättade med betyg (betyg kan vara en ENUM) 

using Learnpoint;

List<IUSer> users = new List<IUSer>();
users.Add(new Admin("Edvin@hotmail.se", "Admin", "password"));
users.Add(new Teacher("Alex@hotmail.se", "AlexT", "password"));
users.Add(new Student("Jarl@hotmail.se", "Jarls", "password"));
users.Add(new Student("Kiffle@hotmail.se", "Kiffle", "password"));
IUSer? active_user = null;

bool running = true;
while (running)
{
  if (active_user == null)
  {
    Console.Clear();
    Console.WriteLine("Please login in");
    Console.WriteLine("username: ");
    string username = Console.ReadLine();

    Console.WriteLine("");
    Console.WriteLine("password: ");
    string _password = Console.ReadLine();

    foreach (IUSer user in users)
    {
      if (user.TryLogin(username, _password))
      {
        active_user = user;
        break;
      }
    }
  }
  else
  {
    Console.Clear();
    switch (active_user.GetRole())
    {
      case Role.Admin:
        Console.WriteLine($"Welcome Admin");
        Console.WriteLine($"Write Admin to get to Admin");
        Console.WriteLine($"Write Teacher to get to Teacher");
        Console.WriteLine($"Write Student to get to Student");
        Console.WriteLine("Logout:");
        string input = Console.ReadLine();
        switch (input)
        {
          case "Admin":
            Console.WriteLine("ADD, to add a new admin");
            switch (Console.ReadLine())
            {
              case "ADD":
                Console.WriteLine("Write username for the new admin!");
                string username = Console.ReadLine();
                Console.WriteLine("Write new user for the new admin!");
                string user = Console.ReadLine();
                Console.WriteLine("Create new password for the new admin!");
                string _password = Console.ReadLine();
                users.Add(new Admin(username, user, _password));

                break;
              case "SHOW":
                Console.WriteLine("Show everyone");
                foreach (IUSer User in users)
                {
                  User.Info();
                }
                Console.ReadLine();
                break;

            }
            break;
          case "Teacher":
            Console.WriteLine("ADD, to add a new teacher");
            switch (Console.ReadLine())
            {
              case "ADD":
                Console.WriteLine("Write username for the new teacher!");
                string username = Console.ReadLine();
                Console.WriteLine("Write new user for the new teacher!");
                string user = Console.ReadLine();
                Console.WriteLine("Create new password for the new teacher!");
                string _password = Console.ReadLine();
                users.Add(new Teacher(username, user, _password));
                break;
            }
            break;
          case "Student":
            Console.WriteLine("ADD, to add a new student");
            switch (Console.ReadLine())
            {
              case "ADD":
                Console.WriteLine("Write username for the new student");
                string username = Console.ReadLine();
                Console.WriteLine("Write new user for the new student!");
                string user = Console.ReadLine();
                Console.WriteLine("Create new passwrod for the new student!");
                string _password = Console.ReadLine();

                users.Add(new Student(username, user, _password));
                break;
            }
            break;
          case "logout":
            active_user = null;
            Console.WriteLine("See you later!");
            Thread.Sleep(2000);
            break;
        }

        break;
      case Role.Teacher:
        Console.WriteLine("Welcome Teacher!");
        break;
      case Role.Student:
        Console.WriteLine("Welcome Student!");
        break;
    }

  }

}

