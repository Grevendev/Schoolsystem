namespace Learnpoint
{
  public enum Role { Admin, Teacher, Student }
  public enum Grade { None, A, B, C, D, E, F }

  public interface IUSer
  {
    string GetUsername();
    string GetName();
    string GetPassword();
    Role GetRole();
    bool TryLogin(string username, string password);
    void Info();
    void SetPassword(string newPassword);
  }
}
