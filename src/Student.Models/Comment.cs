namespace Student.Models
{
    public class Comment
    {
        public long CreationDate { get; set; }
        public string Text { get; set; }
        public User User { get; set; }
    }
}