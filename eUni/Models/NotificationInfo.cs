namespace eUni.Models
{
    public class NotificationInfo
    {
        public int NotificationId { get; set; }
        public ReportsEnum Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ApplicantsRN { get; set; }
        public bool Completed { get; set; }
    }

    public enum ReportsEnum
    {
        BebaiwsiPeratwsisSpoudwn,
        BebaiwsiSpoudwn,
        AnalytikiBathmologia
    }
}
