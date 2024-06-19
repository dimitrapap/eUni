using eUni.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace eUni.Pages.Admin
{
    [RequireAuth(RequiredRole = "Grammateia")]
    public class EditStudentsModel : PageModel
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
        [Required(ErrorMessage = "Year of admission is required")]
        public int YearOfAdmission { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Current semester is required")]
        public int CurrentSemester { get; set; }

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
        [Required(ErrorMessage = "Registration number is required")]
        public string RegistrationNumber { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private readonly string connectionString;

        public EditStudentsModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }

        public void OnGet()
        {
            string requestId = Request.Query["id"];
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Students WHERE StudentsId=@id";
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
                                Specialization = !reader.IsDBNull(reader.GetOrdinal("Specialization")) ? reader.GetString(reader.GetOrdinal("Specialization")) : string.Empty;
                                YearOfAdmission = reader.GetInt32(reader.GetOrdinal("YearOfAdmission"));
                                CurrentSemester = reader.GetInt32(reader.GetOrdinal("CurrentSemester"));
                            }
                            else
                            {
                                Response.Redirect("/Admin/Students");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Students");
            }
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            try
            {
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Students SET Name = @Name,LastName = @LastName"+
      ",Email = @Email,PhoneNumber = @PhoneNumber,RegistrationNumber = @RegistrationNumber,FathersName = @FathersName"+
      ",MothersName = @MothersName,YearOfAdmission = @YearOfAdmission,CurrentSemester = @CurrentSemester"+
      ",Department = @Department,Specialization = @Specialization WHERE StudentsId=@id;";

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
                        command.Parameters.AddWithValue("@Specialization", (Specialization != null) ? Specialization : string.Empty);
                        command.Parameters.AddWithValue("@id", StudentsId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }


            successMessage = "Students information saved correctly";
            Response.Redirect("/Admin/Students");
        }
    }
}
