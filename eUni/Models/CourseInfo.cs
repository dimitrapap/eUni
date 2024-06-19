namespace eUni.Models
{
    public class CourseInfo
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; }
        public Guid TeachersId { get; set; }
        public int Semester { get; set; }
        public int Base{ get; set; }
        public string CodeCourse { get; set; }
        public string CourseType { get; set; }
        public string Department { get; set; }
        public int Credits { get; set; }
        public string CourseDetails { get; set; }
        public bool Checked { get; set; }
    }
}
