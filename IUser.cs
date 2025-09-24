namespace Learnpoint
{
  interface IUSer
  {
    bool TryLogin(string username, string password);
    void Info();
    Role GetRole();
    string GetUsername();
    string GetName();

    // Lösenordshantering (hash returneras av GetPassword)
    string GetPassword();
    void SetPassword(string newPassword);

    // Säkerhetsfält
    int FailedLogins { get; set; }
    bool MustChangePassword { get; set; }
    bool IsActive { get; set; }
  }

  public enum Role
  {
    None,
    Admin,
    Teacher,
    Student
  }

  public enum Grade
  {
    None,
    A,
    B,
    C,
    D,
    E,
    F
  }

  public enum WeekDay
  {
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday
  }
}
