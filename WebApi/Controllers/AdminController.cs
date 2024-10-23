using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        public AdminController(DataContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/leave")]
        public async Task<IActionResult> GetAllLeaveApplications()
        {
            var applications = await _context.LeaveApplications
                .Include(l => l.User)
                .ToListAsync();
            return Ok(applications);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/leave/approve")]
        public async Task<IActionResult> ApproveLeave(ApproveLeaveDto approveLeaveDto)
        {
            //FindAsync :  Retrieves an entity by its primary key.
            var leave = await _context.LeaveApplications.FindAsync(approveLeaveDto.LeaveId);

            if (leave == null)
                return BadRequest("Leave application not found");

            leave.Status = approveLeaveDto.Status;
             _context.SaveChanges();

            return Ok("Leave application approved");
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("/employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Users
                .Where(u => u.Role == UserRole.Employee)
                .ToListAsync();
            return Ok(employees);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("/application")]
        public async Task<IActionResult> GetLeaveApplications()
        {
            //Single Database Call: By using .Include, you minimize the number of database calls.
            //Instead of fetching leave applications and then fetching user details in a separate call,
            //both sets of data are retrieved in one go.
            var applications = await _context.LeaveApplications
                .Include(l => l.User)
                .ToListAsync();
            //This ensures that the data sent to the client meets the expected format.
            //Leave enum 1 2 3 ma thiyo soo string ma pathauna yoo gareyko
            var applicationsDto = applications.Select(a => new LeaveApplicationDto
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

