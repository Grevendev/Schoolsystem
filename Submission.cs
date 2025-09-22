namespace Learnpoint
{
  public class Submission
  {
    public string StudentUsername { get; set; }
    public string AssignmentTitle { get; set; }
    public string Content { get; set; }
    public Grade Grade { get; set; }

    public Submission(string student, string title, string content)
    {
      StudentUsername = student;
      AssignmentTitle = title;
      Content = content;
      Grade = Grade.None;
    }

    public void Show()
    {
      Console.WriteLine($"Submission by {StudentUsername} for {AssignmentTitle}");
      Console.WriteLine($"Content: {Content}");
      Console.WriteLine($"Grade: {Grade}");
    }
  }
}
