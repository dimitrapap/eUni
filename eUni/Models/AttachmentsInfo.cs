namespace eUni.Models
{
    public class AttachmentsInfo
    {
        public Guid AttachmentId { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public Guid CourseId { get; set; }
    }
}
