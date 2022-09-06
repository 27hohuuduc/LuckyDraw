using LuckyDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LuckyDraw.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly LuckyDrawContext _context;

        public CustomerController(LuckyDrawContext context)
        {

            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var name = await Task.Run(() => User.Claims.FirstOrDefault(i => i.Type == "name"));
            if (name != null)
                return Ok(name.Value);
            return BadRequest("Tham số không hợp lệ");
        }
    }
}
