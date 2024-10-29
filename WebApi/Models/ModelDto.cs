using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class UserRegistrationDto
    {
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9]*$", ErrorMessage = "Username must start with a letter and cannot start with a number or special symbol.")]
        public required string Username { get; set; }
        public required string Password { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }
        public required string Department { get; set; }
        public required string Role { get; set; }
    }

    public class UserLoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class ApplyLeaveDto
    {
        [Required(ErrorMessage = "StartDate is required.")]
        [RegularExpression(@"\d{4}/\d{2}/\d{2}", ErrorMessage = "StartDate must be in the format yyyy/MM/dd.")]
        public string StartDate { get; set; } // Accept as string

        [Required(ErrorMessage = "EndDate is required.")]
        [RegularExpression(@"\d{4}/\d{2}/\d{2}", ErrorMessage = "EndDate must be in the format yyyy/MM/dd.")]
        public string EndDate { get; set; } // Accept as string
        public string LeaveType { get; set; }
        public string Username { get; set; }
        public int LeaveApplicationId { get; set; }
    }

    public class LeaveApplicationDto
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Status { get; set; }  // Only used when fetching leaves
        public int LeaveApplicationId { get; set; }
    }




    public class ApproveLeaveDto
    {
        public int LeaveId { get; set; }
        public string Status { get; set; }
    }
}
