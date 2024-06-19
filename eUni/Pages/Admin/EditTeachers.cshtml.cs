using eUni.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace eUni.Pages.Admin
{
    [RequireAuth(RequiredRole = "Grammateia")]
    public class EditTeachersModel : PageModel
    {
        [BindProperty]
        public Guid TeachersId { get; set; }

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
        [Required(ErrorMessage = "Department is required")]
        public string Department { get; set; }
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

        [BindProperty]
        [Required(ErrorMessage = "Number of courses is required")]
        public int NumberOfCourses { get; set; }

        public string errorMessage = "";
        public string successMessage = "";

        private readonly string connectionString;

        public EditTeachersModel(IConfiguration config)
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

                    string sql = "SELECT * FROM Teachers WHERE TeachersId=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", requestId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserId = reader.GetGuid(reader.GetOrdinal("UserId"));
                                TeachersId = reader.GetGuid(reader.GetOrdinal("TeachersId"));
                                Name = reader.GetString(reader.GetOrdinal("Name"));
                                LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                Email = reader.GetString(reader.GetOrdinal("Email"));
                                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                                RegistrationNumber = reader.GetString(reader.GetOrdinal("RegistrationNumber"));
                                FathersName = reader.GetString(reader.GetOrdinal("FathersName"));
                                MothersName = reader.GetString(reader.GetOrdinal("MothersName"));
                                Department = reader.GetString(reader.GetOrdinal("Department"));
                                YearOfAdmission = reader.GetInt32(reader.GetOrdinal("YearOfAdmission"));
                                NumberOfCourses = reader.GetInt32(reader.GetOrdinal("NumberOfCourses"));
                            }
                            else
                            {
                                Response.Redirect("/Admin/Teachers");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.Redirect("/Admin/Teachers");
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
                    string sql = "UPDATE Teachers SET Name = @Name,LastName = @LastName" +
      ",Email = @Email,PhoneNumber = @PhoneNumber,RegistrationNumber = @RegistrationNumber,FathersName = @FathersName" +
      ",MothersName = @MothersName,YearOfAdmission = @YearOfAdmission" +
      ",Department = @Department, NumberOfCourses = @NumberOfCourses WHERE TeachersId=@id;";

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
                        command.Parameters.AddWithValue("@Department", Department);
                        command.Parameters.AddWithValue("@NumberOfCourses", NumberOfCourses);
                        command.Parameters.AddWithValue("@id", TeachersId);

                        command.ExecuteNonQuery();
                    }

                    string sqlSelect = "SELECT UserId FROM Teachers WHERE TeachersId=@id";
                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        command.Parameters.AddWithValue("@id", TeachersId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserId = reader.GetGuid(reader.GetOrdinal("UserId"));
                            }
                        }
                    }

                    string sqlUser = "UPDATE Users SET Name = @Name,LastName = @LastName,Email = @Email,PhoneNumber = @PhoneNumber WHERE UserId=@id";
                    using (SqlCommand command = new SqlCommand(sqlUser, connection))
                    {
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
                        command.Parameters.AddWithValue("@id", UserId);

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


            successMessage = "Teachers information saved correctly";
            Response.Redirect("/Admin/Teachers");
        }
    }
}
