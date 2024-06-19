using eUni.Pages;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Xml.Linq;

namespace eUni.Models
{
    public class StudentsInfo
    {
        public Guid StudentsId { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public int YearOfAdmission { get; set; }
        public int CurrentSemester { get; set; }
        public string Department { get; set; }
        public string? Specialization { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RegistrationNumber { get; set; }
    }
}
