namespace Learnpoint
{
  class Student : IUSer
  {
    public string UserName;
    public string Name;
    private string _passwordHash;

    // Interface properties
    public int FailedLogins { get; set; } = 0;
    public bool MustChangePassword { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public Student(string username, string name, string password)
    {
      UserName = username;
      Name = name;
      _passwordHash = PasswordHelper.HashPassword(password);
    }

    public void SetPassword(string newPassword)
    {
      _passwordHash = PasswordHelper.HashPassword(newPassword);
    }

    public string GetPassword()
    {
      return _passwordHash;
    }

    public void Info()
    {
      Console.WriteLine($"Name: {Name}. Role: {GetRole()}");
    }

    public bool TryLogin(string username, string password)
    {
      return username == UserName && PasswordHelper.VerifyPassword(password, _passwordHash);
    }

    public Role GetRole()
    {
      return Role.Student;
    }

    public string GetUsername()
    {
      return UserName;
    }

    public string GetName()
    {
      return Name;
    }
  }
}
