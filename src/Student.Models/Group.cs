using System;
using System.Collections.Generic;

namespace Student.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public ICollection<Subgroup> Subgroups { get; set; }
        public int Value { get; set; }
        public Subject Subject { get; set; }

        public Group()
        {
            Subgroups = new List<Subgroup>();
            Id = Guid.NewGuid();
        }
    }
}