using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NorbitApi.Model;

namespace NorbitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly NorbitBaseContext _context;

        public UsersController(NorbitBaseContext context)
        {
            _context = context;
        }
         
        [HttpGet("login")]
        public async Task<ActionResult<string>> GenerateTocken(string login, string passwordBase64)
        {
            try
            {
                // Convert Base64 string back to byte array
                byte[] password = Convert.FromBase64String(passwordBase64);
        
                var user = await _context.Users.FirstOrDefaultAsync(p => p.Login == login && p.Password == password);
                if (user != null)
                {
                    if (user.IsBlocked)
                    {
                        _context.Logs.Add(new Log() { StatusId = 5, TypeId = 2, User = user, Time = DateTime.Now });
                        await _context.SaveChangesAsync();
                        return Forbid();
                    }
                    string uuidString = user.Uuid.ToString();
                    var claims = new List<Claim> { new Claim(ClaimTypes.Sid, uuidString) };
                    var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        claims: claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)), // время действия 2 минуты
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                    _context.Logs.Add(new Log() { StatusId = 1, TypeId = 2, User = user, Time = DateTime.Now });
                    await _context.SaveChangesAsync();
        
                    return new JwtSecurityTokenHandler().WriteToken(jwt);
                }
                _context.Logs.Add(new Log() { StatusId = 2, TypeId = 2, Time = DateTime.Now });
                await _context.SaveChangesAsync();
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid request: {ex.Message}");
            }
        }
        // GET: api/Users
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.Uuid)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpPut("block/{id}")]
        public async Task<IActionResult> BlockUser(Guid id)
        {
            User currUser = await _context.Users.FindAsync(id);
            if (currUser == null)
                return BadRequest();

            currUser.IsBlocked = true;
            _context.Entry(currUser).State = EntityState.Modified;
            _context.Logs.Add(new Log() { StatusId = 1, TypeId = 4, User = currUser, Time = DateTime.Now });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            _context.Logs.Add(new Log() { StatusId = 1, TypeId = 1, User = user, Time = DateTime.Now });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Uuid }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Logout endpoint - invalidates the user's session/token
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Extract username from JWT claims
                var sidClaim = User.FindFirst(ClaimTypes.Sid)?.Value;

                if (string.IsNullOrEmpty(sidClaim))
                {
                    _context.Logs.Add(new Log() { StatusId = 3, TypeId = 5, Time = DateTime.Now });
                    await _context.SaveChangesAsync();
                    return BadRequest("Unable to identify user from token");
                }
                // Return success response
                Guid id = new Guid(sidClaim);
                Model.User currUser = _context.Users.FirstOrDefault(p => p.Uuid == id);
                _context.Logs.Add(new Log() { StatusId = 1, TypeId = 5, Time = DateTime.Now });
                await _context.SaveChangesAsync();
                return Ok(new { message = "Logout successful", login = sidClaim });
            }
            catch (Exception ex)
            {
                return BadRequest($"Logout failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Validates if the current token is still valid
        /// Used by client to check token status before making API calls
        /// </summary>
        [Authorize]
        [HttpGet("validate-token")]
        public IActionResult ValidateToken()
        {
            try
            {
                var sidClaim = User.FindFirst(ClaimTypes.Sid)?.Value;

                if (string.IsNullOrEmpty(sidClaim))
                {
                    _context.Logs.Add(new Log() { StatusId = 3, TypeId = 3, Time = DateTime.Now });
                    _context.SaveChanges();
                    return Unauthorized(new { message = "Invalid token", isValid = false });
                }

                Guid id = new Guid(sidClaim);
                Model.User currUser = _context.Users.FirstOrDefault(p => p.Uuid == id);
                _context.Logs.Add(new Log() { StatusId = 1, TypeId = 3, User = currUser, Time = DateTime.Now });
                _context.SaveChanges();
                return Ok(new { message = "Token is valid", login = sidClaim, isValid = true });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message, isValid = false });
            }
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Uuid == id);
        }
    }
}
