using eUni.Helpers;
using eUni.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace eUni.Pages.Admin
{
    [RequireAuth]
    [BindProperties]
    public class AddNewCourseModel : PageModel
    {
        [Required(ErrorMessage = "The title is required")]
        public string Title { get; set; } = "";
		[Required(ErrorMessage = "The Teacher is required")]
		public Guid TeachersId { get; set; }
        [Required(ErrorMessage = "The semester is required")]
        public int Semester { get; set; }
        [Required(ErrorMessage = "The base is required")]
        public int Base { get; set; }
        [Required(ErrorMessage = "The Course Code is required")]
        public string CodeCourse { get; set; }
        [Required(ErrorMessage = "The Course Type is required")]
        public string CourseType { get; set; }
        [Required(ErrorMessage = "The Department is required")]
        public string Department { get; set; }
        [Required(ErrorMessage = "The Credits are required")]
        public int Credits { get; set; }
        [Required(ErrorMessage = "The course details is required")]
        public string CourseDetails { get; set; }
		
		public List<TeachersInfo> TeachersInfoList = new List<TeachersInfo>();

		public string errorMessage = "";
        public string successMessage = "";

        private readonly string connectionString;

        public AddNewCourseModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }

        public void OnGet()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Teachers";

                sql += " ORDER BY LastName ASC";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            TeachersInfo teacher = new TeachersInfo();
                            teacher.UserId = reader.GetGuid(reader.GetOrdinal("UserId"));
                            teacher.TeachersId = reader.GetGuid(reader.GetOrdinal("TeachersId"));
                            teacher.Name = reader.GetString(reader.GetOrdinal("Name"));
                            teacher.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                            teacher.Email = reader.GetString(reader.GetOrdinal("Email"));
                            teacher.PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                            teacher.RegistrationNumber = reader.GetString(reader.GetOrdinal("RegistrationNumber"));
                            teacher.FathersName = reader.GetString(reader.GetOrdinal("FathersName"));
                            teacher.MothersName = reader.GetString(reader.GetOrdinal("MothersName"));
                            teacher.Department = reader.GetString(reader.GetOrdinal("Department"));
                            teacher.NumberOfCourses = reader.GetInt32(reader.GetOrdinal("NumberOfCourses"));
                            teacher.YearOfAdmission = reader.GetInt32(reader.GetOrdinal("YearOfAdmission"));

                            TeachersInfoList.Add(teacher);
                        }
                    }
                }
            }
		}
	

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            Guid UserId = Guid.NewGuid();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqluser = "INSERT INTO Courses " +
                    "VALUES " +
                    "(@CourseId, @Title, @TeachersId, @Semester, @Base, @CodeCourse, @CourseType, @Department, @Credits, @CourseDetails)";

                    using (SqlCommand command = new SqlCommand(sqluser, connection))
                    {
                        command.Parameters.AddWithValue("@CourseId", Guid.NewGuid());
                        command.Parameters.AddWithValue("@Title", Title);
                        command.Parameters.AddWithValue("@TeachersId", TeachersId);
                        command.Parameters.AddWithValue("@Semester", Semester);
                        command.Parameters.AddWithValue("@Base", Base);
                        command.Parameters.AddWithValue("@CodeCourse", CodeCourse);
                        command.Parameters.AddWithValue("@CourseType", CourseType);
                        command.Parameters.AddWithValue("@Department", Department);
                        command.Parameters.AddWithValue("@Credits", Credits);
                        command.Parameters.AddWithValue("@CourseDetails", CourseDetails);

                        command.ExecuteNonQuery();
                    }


                }
            }
            catch (Exception ex)
            {
                    errorMessage = ex.Message;

                return;
            }


            successMessage = "New Course created successfully";

            // redirect to the home page
            Response.Redirect("/");
        }
    }
}
