namespace Learnpoint;

class Teacher : IUSer
{
  public string UserName;
  public string Name;
  string _password;

  public Teacher(string username, string name, string password)
  {
    UserName = username;
    Name = name;
    _password = password;
  }
  public void Info()
  {
    Console.WriteLine($"Name: {Name}. Role: {GetRole()}");
  }
  public bool TryLogin(string username, string password)
  {
    return username == UserName && password == _password;
  }
  public Role GetRole()
  {
    return Role.Teacher;
  }
}