using System;

namespace Student.Models
{
    public class Lecture
    {
        public Guid Id { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string Location { get; set; }
        public LectureType LectureType { get; set; }
        public Subgroup Subgroup { get; set; }

        public Lecture()
        {
            Id = Guid.NewGuid();
        }
    }
}