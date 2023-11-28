
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Laundry.Models.DTO;
using Laundry.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Laundry.Controllers
{
    //don't forget to authorize cause i commented that part
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private IConfiguration config;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration _config)
        {
            this.userManager = userManager;
            this.config = _config;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser
                {
                    Email = registerDTO.Email,
                    Firstname = registerDTO.FirstName,
                    Lastname = registerDTO.LastName,
                    UserName = registerDTO.Username,
                    PhoneNumber = registerDTO.PhoneNumber,
                    City = registerDTO.City,
                    Country = registerDTO.Country,
                    Address = registerDTO.Address
                };

                IdentityResult result = await userManager.CreateAsync(applicationUser, registerDTO.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicationUser, "User");

                    return Ok(new { message = "Account Add Success" });
                }
                else
                {
                    return BadRequest(new { errors = result.Errors });
                }
            }

            return BadRequest(ModelState);
        }



        [HttpPost("LogIn")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = await userManager.FindByEmailAsync(loginDTO.Email);
                if (applicationUser != null)
                {
                    bool result = await userManager.CheckPasswordAsync(applicationUser, loginDTO.Password);
                    if (result)
                    {
                        var userRoles = await userManager.GetRolesAsync(applicationUser);

                        var claims = new List<Claim>
                {
                    new Claim("email", applicationUser.Email),
                    new Claim("username", applicationUser.UserName)
                };

                        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]));
                        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                        var Sectoken = new JwtSecurityToken(
                            issuer: config["Jwt:ValidIssuer"],
                            audience: config["Jwt:ValidAudience"],
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(120),
                            signingCredentials: credentials
                        );

                        var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

                        return Ok(new { token = token, expires = Sectoken.ValidTo, email = applicationUser.Email, username = applicationUser.UserName, roles = userRoles });
                    }
                    else
                    {
                        return BadRequest("Invalid password");
                    }
                }
                else
                {
                    return BadRequest("Invalid email");
                }
            }

            return StatusCode(401);
        }

        [HttpGet("authenticateddetails")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetAuthenticatedDetails()
        {
            var claims = HttpContext.User.Claims;

            var email = claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            var username = claims.FirstOrDefault(x => x.Type == "username")?.Value;
            var roles = claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value)?.ToList();

            var userDetails = new AuthenticatedUserDetailsDTO
            {
                Email = email,
                Username = username,
                Roles = roles
            };

            return Ok(userDetails);
        }


    }
}
