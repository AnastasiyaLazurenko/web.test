using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebSite.DB;
using WebSite.Models;

namespace WebSite.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        [HttpPost("remind")]
        public IActionResult Remind(string email)
        {
            email = email?.ToLower() ?? string.Empty;

            if (!email.IsValidEmail())
            {
                return Conflict("Invalid email.");
            }

            if (!Users.Emails.Contains(email))
            {
                return Conflict("No user was found.");
            }

            return Json($"Email with instructions was sent to {email}");
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] UserDto dto)
        {
            if (Users.IsValid(dto.Login, dto.Password))
            {
                return Ok();
            }

            if (string.IsNullOrEmpty(dto.Login?.Trim()) && string.IsNullOrEmpty(dto.Password))
            {
                return NotFound("Please enter your user name and password");
            }

            if (string.IsNullOrEmpty(dto.Login?.Trim()))
            {
                return NotFound("Please enter your user name");
            }

            if (string.IsNullOrEmpty(dto.Password))
            {
                return NotFound("Please enter your password");
            }

            return NotFound("Your username or password is incorrect");
        }
    }
}
