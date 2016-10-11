using System;

namespace Student.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public long CreationDate { get; set; }
        public string Text { get; set; }
        public User User { get; set; }

        public Comment()
        {
            Id = Guid.NewGuid();
        }
    }
}