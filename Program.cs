using LuckyDraw.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using LuckyDraw.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<LuckyDrawContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("Constr"));
});

builder.Services.AddControllers();
if (!Double.TryParse(builder.Configuration["Jwt:Expires"], out CustomToken.expires))
{
    throw new Exception("Giá trị hết hạn không phải là một số thực.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    // Thêm sự kiện trong quá trình xác thực
    o.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            if (context?.AuthenticateFailure is SecurityTokenExpiredException)
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync("Token đã hết hạn");
            }
            if (context?.AuthenticateFailure?.Message is "Tokens are no longer in use")
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync("Token đã không còn được sử dụng.");
            }
            if (context?.AuthenticateFailure?.Message is "Error")
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync("Có gì đó sai sai");
            }
        },
        OnTokenValidated = async context =>
        {
            if (context != null && context.Principal != null)
            {
                var _context = new LuckyDrawContext(
                    new DbContextOptionsBuilder<LuckyDrawContext>()
                        .UseSqlServer(builder.Configuration.GetConnectionString("Constr")).Options
                );
                #pragma warning disable CS8602
                var iat = await Task.Run(() => context.Principal.Claims.FirstOrDefault(i => i.Type == JwtRegisteredClaimNames.Iat).Value);
                DateTime? datetime;
                if (context.HttpContext.Request.Path.Value.Contains("admin"))
                {
                    var username = await Task.Run(() => context.Principal.Claims.FirstOrDefault(i => i.Type == "username").Value);
                    datetime = (await _context.Cms.FirstOrDefaultAsync(i => i.Username.Equals(username))).AcceptanceTime;
                }
                else
                {
                    var id = await Task.Run(() => context.Principal.Claims.FirstOrDefault(i => i.Type == "id").Value);
                    datetime = (await _context.Customers.FirstOrDefaultAsync(i => i.Id.Equals(int.Parse(id)))).AcceptanceTime;
                }
                #pragma warning restore CS8602
                if (datetime == null)
                {
                    context.Fail("Error");
                }
                else
                {
                    DateTime d1 = datetime ?? DateTime.Now, d2;
                    if (!DateTime.TryParse(iat, out d2))
                    {
                        context.Fail("Error");
                    }
                    else
                    {
                        if (DateTime.Compare(d1, d2) > 0)
                        {
                            context.Fail("Tokens are no longer in use");
                        }
                    }
                }
            }
        }
    };
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();