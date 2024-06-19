using eUni.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace eUni.Pages.Student
{
    [RequireAuth(RequiredRole = "Mathitis")]
    public class StudentsInformationModel : PageModel
    {
        [BindProperty]
        public Guid StudentsId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Fathers name is required")]
        public string FathersName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Mothers name is required")]
        public string MothersName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Current semester is required")]
        public int CurrentSemester {  get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Year of admission is required")]
        public int YearOfAdmission { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; }

        [BindProperty]
        public string? Specialization { get; set; }

        [BindProperty]
        public Guid UserId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        [BindProperty]
        public string RegistrationNumber { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private readonly string connectionString;

        public StudentsInformationModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }

        public void OnGet()
        {
            string requestId = Request.Query["id"];
            if (string.IsNullOrEmpty(requestId))
            {
                requestId = HttpContext.Session.GetString("UserId");
            }
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Students WHERE UserId=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserId = reader.GetGuid(reader.GetOrdinal("UserId"));
                                StudentsId = reader.GetGuid(reader.GetOrdinal("StudentsId"));
                                Name = reader.GetString(reader.GetOrdinal("Name"));
                                LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                Email = reader.GetString(reader.GetOrdinal("Email"));
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                                RegistrationNumber = reader.GetString(reader.GetOrdinal("RegistrationNumber"));
                                FathersName = reader.GetString(reader.GetOrdinal("FathersName"));
                                MothersName = reader.GetString(reader.GetOrdinal("MothersName"));
                                Department = reader.GetString(reader.GetOrdinal("Department"));
                                YearOfAdmission = reader.GetInt32(reader.GetOrdinal("YearOfAdmission"));
                            }
                            //else
                            //{
                            //    Response.Redirect("/Admin/Teachers");
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Response.Redirect("/Admin/Teachers");
            }
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }
            string requestId = Request.Query["id"];
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Students SET Name = @Name,LastName = @LastName" +
      ",Email = @Email,PhoneNumber = @PhoneNumber,RegistrationNumber = @RegistrationNumber,FathersName = @FathersName" +
      ",MothersName = @MothersName,YearOfAdmission = @YearOfAdmission, CurrentSemester = @CurrentSemester" +
      ",Department = @Department, Specialization = @Specialization WHERE StudentsId=@id;";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@RegistrationNumber", RegistrationNumber);
                        command.Parameters.AddWithValue("@FathersName", FathersName);
                        command.Parameters.AddWithValue("@MothersName", MothersName);
                        command.Parameters.AddWithValue("@YearOfAdmission", YearOfAdmission);
                        command.Parameters.AddWithValue("@CurrentSemester", CurrentSemester);
                        command.Parameters.AddWithValue("@Department", Department);
                        command.Parameters.AddWithValue("@Specialization", !string.IsNullOrEmpty(Specialization) ? Specialization : "");
                        command.Parameters.AddWithValue("@id", StudentsId);

                        command.ExecuteNonQuery();
                    }

                    string sqlUser = "UPDATE Users SET Name = @Name,LastName = @LastName,Email = @Email,PhoneNumber = @PhoneNumber WHERE UserId=@id";
                    using (SqlCommand command = new SqlCommand(sqlUser, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@id", requestId);

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }


            successMessage = "Students information saved correctly";
            //Response.Redirect("/Admin/Teachers");
        }
    }
}
