using System;
using System.Collections.Generic;

namespace Student.Models
{
    public class Subgroup
    {
        public Guid Id { get; set; }
        public int Value { get; set; }
        public ICollection<Lecture> Lectures { get; set; }
        public Group Group { get; set; }

        public Subgroup()
        {
            Lectures = new List<Lecture>();
            Id = Guid.NewGuid();
        }
    }
}