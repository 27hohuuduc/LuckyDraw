using LuckyDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using LuckyDraw.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Xml.Linq;

namespace LuckyDraw.Controller
{
    [Authorize]
    [ApiController]
    public class AdminFunctionController : ControllerBase
    {
        private readonly LuckyDrawContext _context;

        public AdminFunctionController(IConfiguration config, LuckyDrawContext context)
        {
            _context = context;
        }

        // Kiểm tra quyền
        public static async Task<bool> IsAdmin(ClaimsPrincipal user, LuckyDrawContext context)
        {
            var role = (await Task.Run(() => user.Claims.FirstOrDefault(i => i.Type == "localrole"))).Value;
            return "Admin".Equals(role?.ToString() ?? "") ? true : false;
        }

        public class Campaign
        {
            public string? Name { set; get; }
            public string? Description { set; get; } = "";
            public int Limit { set; get; }

            // HIện tại chỉ sử dụng kiểu number
            public string? Charset { set; get; } = "Number";

            public int CodeLength { set; get; } = 10;

            public string? Prefix { set; get; }
            
            public DateTime StartTime {set;get;}

            public DateTime EndTime {set;get;}


        }

        [Route("/admin/campaign")]
        [HttpPost]
        public async Task<IActionResult> Post(Campaign campaign)
        {
            if (!await IsAdmin(User, _context))
            {
                return BadRequest("Không đủ quyền truy cập");
            }
            if (campaign != null && campaign.Name != null && campaign.Prefix != null)
            {
                
            }
            return BadRequest("Tham số không hợp lệ");
        }

        public class Product
        {
            public string? Name { set; get; }
        }

        [Route("/admin/product")]
        [HttpPost]
        public async Task<IActionResult> Post(Product product) 
        {
            if (!await IsAdmin(User, _context))
            {
                return BadRequest("Không đủ quyền truy cập");
            }
            if (product != null && product.Name != null)
            {
                await _context.AddAsync(new Models.Product { Name = product.Name });
                var row = await _context.SaveChangesAsync();
                if (row > 0)
                {
                    return Ok("Thêm sản phẩm thành công");
                }
                else
                {
                    return BadRequest("Có gì đó sai sai");
                }
            }
            return BadRequest("Tham số không hợp lệ");
        }

        [Route("/admin/products")]
        [HttpPost]
        public async Task<IActionResult> Post(List<Product> products)
        {
            if (!await IsAdmin(User, _context))
            {
                return BadRequest("Không đủ quyền truy cập");
            }
            if (products == null)
            {
                return BadRequest("Tham số không hợp lệ");
            }
            IList<Models.Product> newProducts = new List<Models.Product>();
            foreach(var product in products)
            {
                if (product.Name == null)
                {
                    return BadRequest("Tham số không hợp lệ");
                }
                newProducts.Add(new Models.Product { Name = product.Name });
            }
            await _context.AddRangeAsync(newProducts);
            var row = await _context.SaveChangesAsync();
            if (row > 0)
            {
                return Ok("Thêm sản phẩm thành công");
            }
            else
            {
                return BadRequest("Có gì đó sai sai");
            }
        }

        [Route("/admin/product")]
        [Route("/admin/product/{size}&{page}")]
        [HttpGet]
        public async Task<IActionResult> Get(int size = 10, int page = 1)
        {
            return Ok(JsonSerializer.Serialize(await _context.Products.OrderBy(i => i.Name).Skip(size * (page - 1)).Take(size).Select(i => i.Name).ToListAsync()));
        }
    }
}
