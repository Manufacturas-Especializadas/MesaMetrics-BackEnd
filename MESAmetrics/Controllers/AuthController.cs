using MESAmetrics.Dtos;
using MESAmetrics.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MESAmetrics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<Users>> Register(UserRegisterDto request)
        {
            if(await _context.Users.AnyAsync(u => u.UserName == request.Username))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "El usuario ya existe"
                });
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new Users
            {
                UserName = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash
            };

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == request.Role);
            if(role == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "El rol especificado no existe"
                });
            }

            var userRole = new UserRoles { RoleId = role.Id, User = user };
            _context.UserRoles.Add(userRole);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Usuario registrado exitosamente"
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var user = await _context.Users
                        .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.Email == request.Email);

            if(user == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Usuario no encontrado"
                });
            }

            if(!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Contraseña incorrecta"
                });
            }

            string token = CreateToken(user);

            return Ok(new { token = token });
        }

        private string CreateToken(Users user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach(var role in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Role.RoleName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _configuration.GetSection("Jwt:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}