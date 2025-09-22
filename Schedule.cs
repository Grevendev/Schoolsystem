namespace Learnpoint
{
  public class Schedule
  {
    public string Course { get; set; }
    public string Teacher { get; set; }
    public string Day { get; set; }
    public string Time { get; set; }

    public Schedule(string course, string teacher, string day, string time)
    {
      Course = course;
      Teacher = teacher;
      Day = day;
      Time = time;
    }

    public void Show()
    {
      Console.WriteLine($"{Course} with {Teacher} on {Day} at {Time}");
    }
  }
}
