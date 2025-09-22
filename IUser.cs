namespace Learnpoint
{
  public enum Role { Admin, Teacher, Student }
  public enum Grade { None, A, B, C, D, E, F }



  public interface IUSer
  {
    bool TryLogin(string username, string password);
    void Info();
    public bool IsActive { get; set; }
    Role GetRole();
    string GetUsername();
    string GetName();
    string GetPassword();
  }
}
