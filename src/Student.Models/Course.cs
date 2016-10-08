using System;
using System.Collections.Generic;

namespace Student.Models
{
    public class Course
    {
        public Guid Id { get; set; }
        public ICollection<Subject> Subjects { get; set; }
        public int GroupCount { get; set; }

        public Course()
        {
            Subjects = new List<Subject>();
            Id = Guid.NewGuid();
        }
    }
}