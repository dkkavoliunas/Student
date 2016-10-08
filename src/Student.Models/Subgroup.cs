using System.Collections.Generic;

namespace Student.Models
{
    public class Subgroup
    {
        public int Value { get; set; }
        public IEnumerable<Lecture> Lectures { get; set; }
        public Group Group { get; set; }
    }
}