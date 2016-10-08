using System.Collections.Generic;

namespace Student.Models
{
    public class Course
    {
        public IEnumerable<Subject> Subjects { get; set; }
        public int GroupCount { get; set; }
    }
}