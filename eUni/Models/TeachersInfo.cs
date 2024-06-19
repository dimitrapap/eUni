namespace eUni.Models
{
    public class TeachersInfo
    {
        public Guid TeachersId { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public int YearOfAdmission { get; set; }
        public string Department { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public int NumberOfCourses { get; set; }
    }
}
