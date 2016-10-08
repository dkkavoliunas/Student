using System.Collections.Generic;

namespace Student.Models
{
    public class Subject
    {
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Group> Group { get; set; }
        public string Name { get; set; }
    }
}