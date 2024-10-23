using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Department { get; set; }
        public required UserRole Role { get; set; }
        //public ICollection<LeaveApplication> LeaveApplications { get; set; }
    }

    public class LeaveApplication
    {
        [Key]
        public int LeaveApplicationId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public string? Status { get; set; }
        public LeaveType LeaveType { get; set; }

        public User User { get; set; }
}

public enum LeaveType
    {
        Paid,
        Sick,   
        Maternity,
        Covid ,
        Vacation
    }

    public enum UserRole
    {
        Admin,
        Employee
    }
}
