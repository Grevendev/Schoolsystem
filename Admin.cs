namespace Learnpoint
{
  public class Admin : IUSer
  {
    public string Username { get; private set; }
    public string Name { get; set; }
    private string Password { get; set; }

    public Admin(string username, string name, string password)
    {
      Username = username;
      Name = name;
      Password = password;
    }

    public string GetUsername() => Username;
    public string GetName() => Name;
    public string GetPassword() => Password;
    public Role GetRole() => Role.Admin;

    public bool TryLogin(string username, string password) =>
        Username == username && Password == password;

    public void Info()
    {
      Console.WriteLine($"[Admin] {Name} ({Username})");
    }

    public void SetPassword(string newPassword) => Password = newPassword;
  }
}
