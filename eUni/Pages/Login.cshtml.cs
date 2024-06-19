using eUni.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;

namespace eUni.Pages
{
    [RequireNoAuth]
    [BindProperties]
    public class LoginModel : PageModel
    {
        [Required(ErrorMessage = "Το email είναι υποχρεωτικό"), EmailAddress]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Ο κωδικός είναι υποχρεωτικός")]
        public string Password { get; set; } = "";

        public string errorMessage = "";
        public string successMessage = "";

        private readonly string connectionString;

        public LoginModel(IConfiguration config)
        {
            connectionString = config.GetConnectionString("eUniDBConnection");
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                errorMessage = "Data validation failed";
                return;
            }

            //successfull data validation

            //connect to database and check the user credentials
            try
            {
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Users WHERE email=@email";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@email", Email);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Guid id = reader.GetGuid(reader.GetOrdinal("UserId"));
                                string firstname = reader.GetString(reader.GetOrdinal("Name"));
                                string lastname = reader.GetString(reader.GetOrdinal("LastName"));
                                string email = reader.GetString(reader.GetOrdinal("Email"));
                                string phone = reader.GetString(reader.GetOrdinal("PhoneNumber"));
                                string arithmosMitrwou = reader.GetString(reader.GetOrdinal("RegistrationNumber"));
                                string hashedPassword = reader.GetString(reader.GetOrdinal("Password"));
                                string role = reader.GetString(reader.GetOrdinal("Role"));
                                string createdOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")).ToString("yyyy-MM-dd");

                                // verify the password
                                var passwordHasher = new PasswordHasher<IdentityUser>();
                                var result = passwordHasher.VerifyHashedPassword(new IdentityUser(),
                                    hashedPassword, Password);


                                if (result == PasswordVerificationResult.Success
                                    || result == PasswordVerificationResult.SuccessRehashNeeded)
                                {
                                    // successful password verification => initialize the session
                                    HttpContext.Session.SetString("UserId", id.ToString());
                                    HttpContext.Session.SetString("firstname", firstname);
                                    HttpContext.Session.SetString("lastname", lastname);
                                    HttpContext.Session.SetString("email", email);
                                    HttpContext.Session.SetString("phone", phone);
                                    HttpContext.Session.SetString("registrationNumber", arithmosMitrwou);
                                    HttpContext.Session.SetString("role", role);
                                    HttpContext.Session.SetString("createdOn", createdOn);

                                    // the user is authenticated successfully => redirect to the home page
                                    if(role == "Kathigitis")
                                    {
                                        Response.Redirect("/Teacher/TeachersInformation?id=" + id.ToString());
                                    }
                                    else if (role == "Mathitis")
                                    {
                                        Response.Redirect("/Student/StudentsInformation?id=" + id.ToString());
                                    }
                                    else
                                    {
                                        Response.Redirect("/");
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            // Wrong Email or Password
            errorMessage = "Wrong Email or Password";
        }
    }

    public enum User
    {
        Grammateia,
        Kathigitis,
        Mathitis
    }
}
