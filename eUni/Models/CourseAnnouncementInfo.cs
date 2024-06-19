namespace eUni.Models
{
    public class CourseAnnouncementInfo
    {
        public Guid CourseAnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CourseId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
