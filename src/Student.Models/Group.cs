using System.Collections.Generic;

namespace Student.Models
{
    public class Group
    {
        public IEnumerable<Subgroup> Subgroups { get; set; }
        public int Value { get; set; }
        public Subject Subject { get; set; }
    }
}