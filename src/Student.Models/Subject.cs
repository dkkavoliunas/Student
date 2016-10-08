using System;
using System.Collections.Generic;

namespace Student.Models
{
    public class Subject
    {
        public Guid Id { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Group> Groups { get; set; }
        public string Name { get; set; }
        public SubjectType Type { get; set; }

        public Subject()
        {
            Comments = new List<Comment>();
            Groups = new List<Group>();
            Id = Guid.NewGuid();
        }
    }
}