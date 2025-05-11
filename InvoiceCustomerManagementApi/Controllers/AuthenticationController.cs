using DataAccessLayer.Model;
using DataAccessLayer.ViewModel;
using InvoiceCustomerManagementApi.CommonModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InvoiceCustomerManagementApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        //smtp

        static string smtpAddress = "smtp.gmail.com";
        static int portNumber = 587;
        static bool enableSSL = true;
        static string emailFromAddress = "vivek1.satva@gmail.com"; //Sender Email Address  
        static string password = "xiqo obzt aeqk oiai"; //Sender Password  
        static string emailToAddress; //Receiver Email Address  
        static string subject = "Successful registration!";
        static string body = "Congratulations, you have registered successfully.\n Click on this link to verify: ";

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //[HttpPost]
        //[Route("createRole")]
        //public async Task<ActionResult> CreateRole()
        //{
        //    await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
        //    await _roleManager.CreateAsync(new ApplicationRole { Name = "Employee" });
        //    return Ok();
        //}
        [HttpPost]
        [Route("createUser")]
        public async Task<IActionResult> Register(RegisterViewModel request = null)
        {
            var result = await RegisterAsync(request);
            return Ok(result);
        }
        [HttpPost]
        [Route("loginUser")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await LoginAsync(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("resetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordModel request)
        {
            var result = await ResetPassword(request);
            return Ok(result);
        }

        private async Task<CommonJsonResponse> ResetPassword(ResetPasswordModel request)
        {
            string emailId = User.FindFirst(ClaimTypes.Name)?.Value;
            request.Email = emailId;

            if (!ModelState.IsValid)
                return ValidateModel();

            var user = await _userManager.FindByEmailAsync(emailId);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (user == null)
                return new CommonJsonResponse { message = "Email not exist.", responseStatus = 0 };

            var resetPassResult = await _userManager.ResetPasswordAsync(user, token, request.Password);
            if (!resetPassResult.Succeeded)
            {
                foreach (var error in resetPassResult.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return new CommonJsonResponse { message = "Password not updated", responseStatus = 0 };
            }
            return new CommonJsonResponse
            {
                message = "Password updated successfully",
                responseStatus = 1,
            };
        }

        private async Task<CommonJsonResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null) return new CommonJsonResponse { message = "Invalid email/password", responseStatus = 0 };
            if (!user.EmailConfirmed) return new CommonJsonResponse { message = "Email is not verfied. Please verify it.", responseStatus = 0 };
            //after validating email checking password

            var checkPassword = await _userManager.CheckPasswordAsync(user,request.Password);
            if (!checkPassword) return new CommonJsonResponse { message = "Incorrect Password", responseStatus = 0 };
            //after validating password adding claims

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaim = roles.Select(x => new Claim(ClaimTypes.Role, x));
            claims.AddRange(roleClaim);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1swek3u4uo2u4a6e123456789012345678901234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(30);

            var token = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds
                    );

            return new CommonJsonResponse
            {

                message = "Login Successful",
                result = new
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Email = user?.Email,
                    UserId = user?.Id.ToString()
                },
                responseStatus = 1,
            };
        }

        private async Task<CommonJsonResponse> RegisterAsync(RegisterViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidateModel();
            }
            var objUserInformation = await _userManager.FindByEmailAsync(request.Email);
            if (objUserInformation != null) return new CommonJsonResponse { message = "User already exists", responseStatus = 0 };

            //same email not found

            emailToAddress = request.Email;

            objUserInformation = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                UserName = request.Email,
            };
            var createUserResult = await _userManager.CreateAsync(objUserInformation, request.Password);
            if (!createUserResult.Succeeded) return new CommonJsonResponse { message = $"Create user failed {createUserResult?.Errors?.First()?.Description}", responseStatus = 0 };

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(objUserInformation);
            body += Url.Action("ConfirmEmail", "Authentication", new { token, email = objUserInformation.Email }, Request.Scheme);
            SendEmail();


            //user created and role created
            var addUserToRoleResult = await _userManager.AddToRoleAsync(objUserInformation, "EMPLOYEE");
            if (!addUserToRoleResult.Succeeded) return new CommonJsonResponse { message = $"Create user succeeded but could not add user to role {addUserToRoleResult?.Errors?.First()?.Description}", responseStatus = 2 };

            //no error found
            return new CommonJsonResponse
            {
                responseStatus = 1,
                message = "User registered successfully",
                result = objUserInformation
            };
        }

        private CommonJsonResponse ValidateModel()
        {
            var errors = ModelState
                           .Where(x => x.Value.Errors.Any())
                           .Select(x => new
                           {
                               Field = x.Key,
                               Error = x.Value.Errors.First().ErrorMessage
                           })
                           .ToList();

            return new CommonJsonResponse
            {
                responseStatus = 0,
                message = "Validation failed",
                result = errors
            };
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Ok("Email Not Exist");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return Ok(result.Succeeded ? nameof(ConfirmEmail) : "Error");
        }

        public static void SendEmail()
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFromAddress);
                mail.To.Add(emailToAddress);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFromAddress, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }
    }
}

