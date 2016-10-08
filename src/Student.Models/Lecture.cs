namespace Student.Models
{
    public class Lecture
    {
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public string Location { get; set; }
        public LectureType LectureType { get; set; }
        public Subgroup Subgroup { get; set; }
    }
}