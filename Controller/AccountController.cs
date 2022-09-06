using LuckyDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using LuckyDraw.Services;
using Microsoft.AspNetCore.Authorization;

namespace LuckyDraw.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly LuckyDrawContext _context;

        public AccountController(IConfiguration config, LuckyDrawContext context)
        {
            _configuration = config;
            _context = context;
        }

        public class Login
        {
            public string? Phone { set; get; }
            public string? Password { set; get; }
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Post(Login login)
        {
            if (login != null && login.Phone != null && login.Password != null)
            {
                byte[] bytes;
                using (SHA256 sha256 = SHA256.Create())
                {
                    bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(login.Password));
                }
                var user = await _context.Customers.FirstOrDefaultAsync(
                    i => i.Phone.Equals(login.Phone) && i.Password.SequenceEqual(bytes));
                if (user != null)
                {
                    user.AcceptanceTime = DateTime.UtcNow;
                    _context.Update(user);
                    var row = await _context.SaveChangesAsync();
                    if (row > 0)
                    {
                        return Ok(new JwtSecurityTokenHandler().WriteToken(CustomToken.Maketoken(user.Id.ToString(), user.Name, user.Phone, _configuration["Jwt:Key"])));
                    }
                    return BadRequest("Có gì đó sai sai");
                }
                else
                {
                    return BadRequest("Thông tin không chính xác");
                }
            }
            else
            {
                return BadRequest("Tham số không hợp lệ");
            }
        }

        public class Register
        {
            public string? fullName { set; get; }
            public string? Phone { set; get; }
            public string? Birth { set; get; }
            public string? Position { set; get; }
            public string? TypeBusiness { set; get; }
            public string? Password { set; get; }
        }

        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> Post(Register register)
        {
            #pragma warning disable CS8601
            if (register != null)
            {
                List<String> Msg = new List<string>();

                if (String.IsNullOrWhiteSpace(register.fullName))
                    Msg.Add("Tên không hợp lệ");

                if (!CheckStringFormat.IsPhoneNummber(register.Phone)
                    || await _context.Customers.AnyAsync(i => i.Phone == register.Phone))
                    Msg.Add("Số điện thoại không hợp lệ");

                DateTime birth;
                if (!DateTime.TryParse(register.Birth, out birth))
                    Msg.Add("Ngày sinh không hợp lệ");

                if (!CheckStringFormat.IsPassword(register.Password))
                    Msg.Add("Mật khẩu không hợp lệ");

                if (Msg.Count > 0)
                    return BadRequest(Msg);

                byte[] bytes;
                using (SHA256 sha256 = SHA256.Create())
                {
                    bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
                }
                await _context.Customers.AddAsync(new Customer()
                {
                    Name = register.fullName,
                    Password = bytes,
                    Phone = register.Phone,
                    Birth = birth,
                    Position = register.Position,
                    TypeBusiness = register.TypeBusiness,
                    AcceptanceTime = DateTime.UtcNow
                });
                int row = await _context.SaveChangesAsync();
                if (row > 0)
                    return Ok("Đăng ký thành công");
                else
                    return BadRequest("Có gì đó sai sai");
            }
            else
            {
                return BadRequest("Tham số không hợp lệ");
            }
            #pragma warning restore CS8601
        }

        public class ResetPassword
        {
            public string? Phone { set; get; }
            public string? OldPassword { set; get; }
            public string? NewPassword { set; get; }
        }

        [Route("/resetpassword")]
        [HttpPost]
        public async Task<IActionResult> Post(ResetPassword resetPassword)
        {
            if (resetPassword != null && resetPassword.Phone != null 
                && resetPassword.OldPassword != null && resetPassword.NewPassword != null)
            {
                byte[] bytes;
                using (SHA256 sha256 = SHA256.Create())
                {
                    bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(resetPassword.OldPassword));
                }
                var user = await _context.Customers.FirstOrDefaultAsync(
                    i => i.Phone.Equals(resetPassword.Phone) && i.Password.SequenceEqual(bytes));
                if (user != null && CheckStringFormat.IsPassword(resetPassword.NewPassword))
                {
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(resetPassword.NewPassword));
                    }
                    user.Password = bytes;
                    user.AcceptanceTime = DateTime.UtcNow;
                    _context.Customers.Update(user);
                    int row = await _context.SaveChangesAsync();
                    if (row > 0)
                    {
                        return Ok(new JwtSecurityTokenHandler().WriteToken(CustomToken.Maketoken(user.Id.ToString(), user.Name, user.Phone, _configuration["Jwt:Key"])));
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
            else
            {
                return BadRequest("Yêu cầu tài khoản và mật khẩu");
            }
        }

        [Authorize]
        [Route("/refreshtoken")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (User != null)
            {
                #pragma warning disable CS8602
                var id = await Task.Run(() => User.Claims.FirstOrDefault(i => i.Type == "id").Value);
                var name = await Task.Run(() => User.Claims.FirstOrDefault(i => i.Type == "name").Value);
                var phone = await Task.Run(() => User.Claims.FirstOrDefault(i => i.Type == "phone").Value);
                #pragma warning restore CS8602
                if (id != null && name != null && phone != null)
                {
                    return Ok(new JwtSecurityTokenHandler().WriteToken(CustomToken.Maketoken(id, name, phone, _configuration["Jwt:Key"])));
                }
            }
            return BadRequest("Có gì đó sai sai");
        }
    }
}