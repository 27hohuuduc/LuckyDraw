using LuckyDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LuckyDraw.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Xml.Linq;

namespace LuckyDraw.Controller
{
    [ApiController]
    public class AdminController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly LuckyDrawContext _context;

        public AdminController(IConfiguration config, LuckyDrawContext context)
        {
            _configuration = config;
            _context = context;
        }

        public class Login
        {
            public string? Email { set; get; }
            public string? Password { set; get; }
        }

        [Route("/admin/login")]
        [HttpPost]
        public async Task<IActionResult> Post(Login login)
        {
            if (login != null && login.Email != null && login.Password != null)
            {
                var cms = await _context.Cms.FirstOrDefaultAsync(i => i.Username == login.Email && i.Password == login.Password);
                if (cms != null)
                {
                    cms.AcceptanceTime = DateTime.UtcNow;
                    _context.Update(cms);
                    var row = await _context.SaveChangesAsync();
                    if (row > 0)
                    {
                        var claims = new[] {
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss.fff tt")),
                            new Claim("username", cms.Username),
                            new Claim("localrole", cms.Role)
                           };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var exp = DateTime.UtcNow.AddMinutes(CustomToken.expires);
                        return Ok(new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(claims: claims, expires: exp, signingCredentials: signIn)));
                    }
                    else
                    {
                        return BadRequest("Có gì đó sai sai");
                    }
                }
                else
                {
                    return BadRequest("Thông tin không chính xác");
                }
            }
            return BadRequest("Tham số không hợp lệ");
        }

        [Authorize]
        [Route("/admin/refreshtoken")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (User != null)
            {
                #pragma warning disable CS8602
                var username = await Task.Run(() => User.Claims.FirstOrDefault(i => i.Type == "username").Value);
                var localrole = await Task.Run(() => User.Claims.FirstOrDefault(i => i.Type == "localrole").Value);
                #pragma warning restore CS8602
                if (username != null && localrole != null)
                {
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss.fff tt")),
                        new Claim("username", username),
                        new Claim("localrole", localrole)
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var exp = DateTime.UtcNow.AddMinutes(CustomToken.expires);
                    return Ok(new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(claims: claims, expires: exp, signingCredentials: signIn)));
                }
            }
            return BadRequest("Có gì đó sai sai");
        }
    }
}
