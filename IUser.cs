namespace Learnpoint;

interface IUSer
{
  public bool TryLogin(string username, string password);
  public void Info();
  public Role GetRole();

}
enum Role
{
  None,
  Admin,
  Teacher,
  Student,
}

