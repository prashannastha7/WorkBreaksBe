using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        public AdminController(DataContext context)
        {
            _context = context;
        }

        [HttpPut("leave/approve")]
        public async Task<IActionResult> ApproveLeave(ApproveLeaveDto approveLeaveDto)
        {
            //FindAsync :  Retrieves an entity by its primary key.
           
            var leave = await _context.LeaveApplications.FindAsync(approveLeaveDto.LeaveId);

            if (leave == null)
                return BadRequest("Leave application not found");

            if (approveLeaveDto.Status != "Approved" && approveLeaveDto.Status != "Declined")
                return BadRequest("Invalid status value");

            leave.Status = approveLeaveDto.Status;
             await _context.SaveChangesAsync();

            return Ok("Leave application updated succesfully");
        }

        [HttpPut("leave/decline")]
        public async Task<IActionResult> DeclineLeave(ApproveLeaveDto approveLeaveDto)
        {
            //FindAsync :  Retrieves an entity by its primary key.
           
            var leave = await _context.LeaveApplications.FindAsync(approveLeaveDto.LeaveId);

            if (leave == null)
                return BadRequest("Leave application not found");

            if (approveLeaveDto.Status != "Approved" && approveLeaveDto.Status != "Declined")
                return BadRequest("Invalid status value");

            leave.Status = approveLeaveDto.Status;
            await _context.SaveChangesAsync();

            return Ok("Leave application updated succesfully");
        }

        [HttpGet("/employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Users
                .Where(u => u.Role == UserRole.Employee)
                .ToListAsync();
            return Ok(employees);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return BadRequest("User not found");

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok("User deleted successfully");
        }

        [HttpGet("/application")]
        public async Task<IActionResult> GetLeaveApplications()
        {
            //Single Database Call: By using .Include, you minimize the number of database calls.
            //Instead of fetching leave applications and then fetching user details in a separate call, both sets of data are retrieved in one go.
           
            var applications = await _context.LeaveApplications
                .Where(u => u.Status == "Pending")
                .Include(l => l.User)
                .ToListAsync();
            
            //This ensures that the data sent to the client meets the expected format.
            //Leave enum 1 2 3 ma thiyo soo string ma pathauna yoo gareyko
            
            var applicationsDto = applications.Select(a => new ApplyLeaveDto
            {
                StartDate = a.StartDate.ToString("yyyy/MM/dd"),
                EndDate = a.EndDate.ToString("yyyy/MM/dd"),
                LeaveType = a.LeaveType.ToString(),
                Username = a.User.Username,
                LeaveApplicationId = a.LeaveApplicationId
            });
            return Ok(applicationsDto);
        }

        //[HttpGet("leave/approved")]
        //public async Task<IActionResult> GetApprovedLeave()
        //{
        //    var result = await _context.LeaveApplications
        //       .Where(l => l.Status == "Approved")
        //        .Include(l => l.User)
        //        .ToListAsync();
            
        //    var applicationsDto = result.Select(a => new ApplyLeaveDto
        //    {
        //        StartDate = a.StartDate.ToString("yyyy/MM/dd"),
        //        EndDate = a.EndDate.ToString("yyyy/MM/dd"),
        //        LeaveType = a.LeaveType.ToString(),
        //        Username = a.User.Username
        //    });
        //    return Ok(applicationsDto);
        //}

        [HttpGet("leave/approved/today")]
        public async Task<IActionResult> GetTodayApprovedLeave()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now); // Get today's date as DateOnly

            var approvedLeaves = await _context.LeaveApplications
                .Where(l => l.Status == "Approved"
                             && l.StartDate <= today && l.EndDate >= today)
                .Include(a => a.User)
                .ToListAsync();


            var applicationsDto = approvedLeaves.Select(a => new ApplyLeaveDto
            {
                StartDate = a.StartDate.ToString("yyyy/MM/dd"),
                EndDate = a.EndDate.ToString("yyyy/MM/dd"),
                LeaveType = a.LeaveType.ToString(),
                Username = a.User.Username
            });
            return Ok(applicationsDto);
        }

        //[HttpGet("leave/declined")]
        //public async Task<IActionResult> GetDeclinedLeave()
        //{             var result = await _context.LeaveApplications
        //       .Where(l => l.Status == "Declined")
        //        .Include(l => l.User)
        //        .ToListAsync();

        //    var applicationsDto = result.Select(a => new ApplyLeaveDto
        //    {
        //        StartDate = a.StartDate.ToString("yyyy/MM/dd"),
        //        EndDate = a.EndDate.ToString("yyyy/MM/dd"),
        //        LeaveType = a.LeaveType.ToString(),
        //        Username = a.User.Username
        //    });
        //    return Ok(applicationsDto);
        //}

        [HttpGet("leave/declined/today")]
        public async Task<IActionResult> GetDeclinedLeave()
        {
            var today = DateOnly.FromDateTime(DateTime.Now); // Get today's date as DateOnly
            var result = await _context.LeaveApplications
               .Where(l => l.Status == "Declined" &&
               l.StartDate <= today && l.EndDate >= today)
                .Include(l => l.User)
                .ToListAsync();

            var applicationsDto = result.Select(a => new ApplyLeaveDto
            {
                StartDate = a.StartDate.ToString("yyyy/MM/dd"),
                EndDate = a.EndDate.ToString("yyyy/MM/dd"),
                LeaveType = a.LeaveType.ToString(),
                Username = a.User.Username
            });
            return Ok(applicationsDto);
        }

        [HttpGet("leave/pending")]
        public async Task<IActionResult> GetPendingLeave()
        {
            var result = await _context.LeaveApplications
               .Where(l => l.Status == "Pending")
                .Include(l => l.User)
                .ToListAsync();
            
            var applicationsDto = result.Select(a => new ApplyLeaveDto
            {
                StartDate = a.StartDate.ToString("yyyy/MM/dd"),
                EndDate = a.EndDate.ToString("yyyy/MM/dd"),
                LeaveType = a.LeaveType.ToString(),
                Username = a.User.Username
            });
            return Ok(applicationsDto);
        }
    }
}
