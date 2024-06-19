using eUni.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Net;

namespace eUni.Pages.Admin
{
    [RequireAuth]
    [BindProperties]
    public class RegisterModel : PageModel
    {
        [Required(ErrorMessage = "The First Name is required")]
        public string Firstname { get; set; } = "";

        [Required(ErrorMessage = "The Last Name is required")]
        public string Lastname { get; set; } = "";

        [Required(ErrorMessage = "The Email is required"), EmailAddress]
        public string Email { get; set; } = "";

		[Required(ErrorMessage = "The Phone Number is required")]
		public string? Phone { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must be between 5 and 50 characters", MinimumLength = 5)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = "";

		[Required(ErrorMessage = "Fathers name is required")]
		public string FathersName { get; set; }

		[Required(ErrorMessage = "Mothers name is required")]
		public string MothersName { get; set; }

		[Required(ErrorMessage = "Year of admission is required")]
		public int YearOfAdmission { get; set; }
		public int CurrentSemester { get; set; }
		[Required(ErrorMessage = "Department is required")]
		public string Department { get; set; }
		
		public string? Specialization { get; set; }
		
		[Required(ErrorMessage = "Registration number is required")]
		public string RegistrationNumber { get; set; }

		public int NumberOfCourses { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }


		public string errorMessage = "";
        public string successMessage = "";

		private readonly string connectionString;

		public RegisterModel(IConfiguration config)
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
            
            Guid UserId = Guid.NewGuid();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

					string sqluser = "INSERT INTO users " +
					"VALUES " +
					"(@UserId,@Name,@LastName,@Email,@PhoneNumber,@RegistrationNumber,@Password,@Role,@CreatedOn)";

					var passwordHasher = new PasswordHasher<IdentityUser>();
					string hashedPassword = passwordHasher.HashPassword(new IdentityUser(), Password);

					using (SqlCommand command = new SqlCommand(sqluser, connection))
					{
						command.Parameters.AddWithValue("@UserId", UserId);
						command.Parameters.AddWithValue("@Name", Firstname);
						command.Parameters.AddWithValue("@LastName", Lastname);
						command.Parameters.AddWithValue("@Email", Email);
						command.Parameters.AddWithValue("@PhoneNumber", Phone);
                        command.Parameters.AddWithValue("@RegistrationNumber", RegistrationNumber);
						command.Parameters.AddWithValue("@Password", hashedPassword);
						command.Parameters.AddWithValue("@Role", Role);
						command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

						command.ExecuteNonQuery();
					}
					string sql;
                    if(Role == "Kathigitis")
                    { 
						sql = "INSERT INTO Teachers " +
					    "VALUES" +
					    "(@TeachersId,@UserId,@Name,@LastName,@Email,@PhoneNumber,@RegistrationNumber,@FathersName,@MothersName,@YearOfAdmission,@Department,@NumberOfCourses);";
					}
                    else
                    {
						sql = "INSERT INTO Students " +
					    "VALUES" +
						"(@StudentsId,@UserId,@Name,@LastName,@Email,@PhoneNumber,@RegistrationNumber,@FathersName,@MothersName,@YearOfAdmission,@CurrentSemester,@Department,@Specialization);";
					}

					using (SqlCommand command = new SqlCommand(sql, connection))
					{
						
						command.Parameters.AddWithValue("@UserId", UserId);
                        command.Parameters.AddWithValue("@Name", Firstname);
                        command.Parameters.AddWithValue("@LastName", Lastname);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@PhoneNumber", Phone);
                        command.Parameters.AddWithValue("@RegistrationNumber", RegistrationNumber);
                        command.Parameters.AddWithValue("@YearOfAdmission", YearOfAdmission);
                        
                        command.Parameters.AddWithValue("@Department", Department);
                        
                        command.Parameters.AddWithValue("@MothersName", MothersName);
                        command.Parameters.AddWithValue("@FathersName", FathersName);
                        
                        if(Role == "Kathigitis")
                        {
							command.Parameters.AddWithValue("@TeachersId", Guid.NewGuid());
							command.Parameters.AddWithValue("@NumberOfCourses", NumberOfCourses);
						}
                        else
                        {
							command.Parameters.AddWithValue("@StudentsId", Guid.NewGuid());
							command.Parameters.AddWithValue("@CurrentSemester", CurrentSemester);
							command.Parameters.AddWithValue("@Specialization", (Specialization != null) ? Specialization : string.Empty);
						}
                        
						

						command.ExecuteNonQuery();
					}
					
                    
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(Email))
                {
                    errorMessage = "Email address already used";
                }
                else
                {
                    errorMessage = ex.Message;
                }

                return;
            }


            successMessage = "Account created successfully";

            // redirect to the home page
            Response.Redirect("/");
        }
    }
}

