namespace Learnpoint
{
  public class Assignment
  {
    public string Title { get; set; }
    public Grade Grade { get; set; }

    public Assignment(string title)
    {
      Title = title;
      Grade = Grade.None;
    }

    public void Show()
    {
      Console.WriteLine($"Assignment: {Title} | Grade: {Grade}");
    }
  }
}
