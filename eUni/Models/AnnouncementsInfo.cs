namespace eUni.Models
{
    public class AnnouncementsInfo
    {
        public Guid AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public AuthorEnum Author { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public enum AuthorEnum
    {
        Grammateia = 1,
        Kathigitis = 2
    }
}
