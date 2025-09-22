namespace Learnpoint
{
  public class Message
  {
    public string From { get; set; }
    public string To { get; set; }
    public string Content { get; set; }

    public Message(string from, string to, string content)
    {
      From = from;
      To = to;
      Content = content;
    }

    public void Show()
    {
      Console.WriteLine($"From: {From} -> To: {To} | {Content}");
    }
  }
}
