using backend.Commons;
using backend.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountCrudController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IMailHelperService _mailHelperService;
        private IMailTemplateHelperService _mailTemplateHelperService;
        private IConfiguration _config;

        public AccountCrudController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMailHelperService mailHelperService,
            IMailTemplateHelperService mailTemplateHelperService,
            IConfiguration config
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mailHelperService = mailHelperService;
            _mailTemplateHelperService = mailTemplateHelperService;
            _config = config;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] UserCreateForm userForm)
        {
            if (userForm.Password != userForm.ConfirmPassword)
            {
                return new BadRequestObjectResult(new AppResponse { Message = "Konfirmasi password tidak cocok." });
            }
            var user = new ApplicationUser
            {
                UserName = userForm.Email,
                Name = userForm.Username,
                Email = userForm.Email,
                NormalizedEmail = userForm.Email.ToUpper(),
                NormalizedUserName = userForm.Username.ToUpper(),
            };
            var result = await _userManager.CreateAsync(user,userForm.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var url = Url.Action("Activate", "Account", new { Token = code, Email=user.Email }, Request.Scheme);
                var model = new NewUserEmailModel 
                { 
                    Url= url,
                    Email=user.Email,
                    Name=user.Name
                };
                var htmlTemplate = await _mailTemplateHelperService.GetTemplateHtmlAsStringAsync<NewUserEmailModel>("UserRegistrationSuccess.html", model);
                _mailHelperService.SendMail(model.Email, "Pendaftaran berhasil!", htmlTemplate);

                return new OkObjectResult(new AppResponse { Message = "Pendaftaran berhasil. Silahkan cek email anda." });
            }
            return new BadRequestObjectResult(new AppResponse { Message="Email sudah digunakan."});
        }
        [HttpGet]
        public async Task<IActionResult> Activate([FromQuery] string Token, [FromQuery] string Email)
        {
            
            var user = await _userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                //TODO Create view no user found
                return new BadRequestObjectResult(new AppResponse { Message = "User tidak ditemukan." });
            }
            var result = await _userManager.ConfirmEmailAsync(user, Token);
            if (result.Succeeded)
            {
                var location = new Uri($"{Request.Scheme}://{Request.Host}");

                var url = location.AbsoluteUri;
                return RedirectPermanent(url);
            }
            else
            {
                //TODO Create view error confirm
                return new BadRequestObjectResult(new AppResponse { Message = "User tidak ditemukan." });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UserUpdateForm userForm)
        {
            var user = await _userManager.FindByIdAsync(userForm.Id.ToString());
            var checkDuplicateEmailUser = await _userManager.FindByEmailAsync(userForm.Email);
            if (user != null && checkDuplicateEmailUser  ==null)
            {
                user.UserName = userForm.Username;
                
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (user.Email != userForm.Email)
                    {
                        var token = await _userManager.GenerateChangeEmailTokenAsync(user, userForm.Email);
                        var resultChangeEmail = await _userManager.ChangeEmailAsync(user, userForm.Email, token);
                        if (resultChangeEmail.Succeeded)
                        {
                            return new OkObjectResult(new AppResponse { Message = "Perubahan data berhasil." });
                        }
                        return new BadRequestObjectResult(new AppResponse { Message = "Perubahan email gagal." });
                    }
                    return new OkObjectResult(new AppResponse { Message = "Perubahan data berhasil." });
                }
            }
            return new BadRequestObjectResult(new AppResponse { Message = "Email sudah digunakan." });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> UpdatePassword([FromForm] UserUpdatePasswordForm userForm)
        {
            var user = await _userManager.FindByIdAsync(userForm.Id.ToString());
            if (user != null)
            {
               
                var result = await _userManager.ChangePasswordAsync(user, userForm.OldPassword,userForm.NewPassword);

                if (result.Succeeded)
                {
                    return new OkObjectResult(new AppResponse { Message = "Perubahan data berhasil." });
                }
                return new BadRequestObjectResult(new AppResponse { Message = "Password salah." });
            }
            return new BadRequestObjectResult(new AppResponse { Message = "Pengguna tidak ada." });
        }
        
        [HttpPost]
        public async Task<IActionResult> OTPForgetPassword([FromForm] UserOTPPassword userForm)
        {
            var user = await _userManager.FindByEmailAsync(userForm.Email);
            if (user != null)
            {
                int otp = new Random().Next(100000, 999999);
                user.Otp = otp.ToString();
                user.OtpExpired = DateTime.Now.AddMinutes(5);
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {

                    var model = new UserOtpModel
                    { 
                        Email=user.Email,
                        Name=user.Name,
                        Otp=otp.ToString()
                    };
                    var htmlTemplate = await _mailTemplateHelperService.GetTemplateHtmlAsStringAsync<UserOtpModel>("UserFogetPasswordOtp.html", model);
                    _mailHelperService.SendMail(model.Email, "Reset Password!", htmlTemplate);
                    return new OkObjectResult(new AppResponse { Message = "Kami telah mengirim kode ke email anda." });
                }
                return new BadRequestObjectResult(new AppResponse { Message = "Terdapat kesalahan dalam pembuatan kode OTP." });
            }
            return new BadRequestObjectResult(new AppResponse { Message = "Pengguna tidak ada." });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromForm] UserResetPassword userForm)
        {
            var user = await _userManager.FindByEmailAsync(userForm.Email);
            if (user != null && user.Otp==userForm.Otp)
            {
                if (user.OtpExpired < DateTime.Now)
                {
                    return new BadRequestObjectResult(new AppResponse { Message = "Kode OTP sudah kadaluwarsa." });

                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, userForm.Password);

                if (result.Succeeded)
                {
                    user.Otp = null;
                    user.OtpExpired = null;
                    await _userManager.UpdateAsync(user);
                    return new OkObjectResult(new AppResponse { Message = "Atur ulang password berhasil." });
                }
            }
            return new BadRequestObjectResult(new AppResponse { Message = "Email sudah digunakan." });
        }
        
        [HttpGet]
        public async Task<UserDto> Read([FromQuery] UserForm userForm)
        {
            var user = await _userManager.FindByNameAsync(userForm.Email);
            return new UserDto { Email = user.Email, Id = user.Id };
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserLoginForm userForm)
        {
            var user = await _userManager.FindByNameAsync(userForm.Email);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, userForm.Password, false);

                if (result.Succeeded)
                {
                    //var t = await _userManager.CreateSecurityTokenAsync(user);

                    var claims = new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(JwtRegisteredClaimNames.GivenName, user.Name),
                        //tambah role
                    };
                    var secret = _config["JwtSettings:SymKey"];
                    var secretByte = Encoding.UTF8.GetBytes(secret);
                    var key = new SymmetricSecurityKey(secretByte);
                    var algorithm = SecurityAlgorithms.HmacSha256;
                    var signinCredentials = new SigningCredentials(key, algorithm);
                    var token = new JwtSecurityToken(null,null,claims,DateTime.Now,DateTime.Now.AddHours(12),signinCredentials);
                    var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
                    return new OkObjectResult(new LoginResponse { Message = "Masuk.", AccessToken = tokenJson });
                }
                else if (result.IsNotAllowed)
                {
                    return new BadRequestObjectResult(new AppResponse { Message = "Akun belum aktif." });
                }
            }
            return new BadRequestObjectResult(new AppResponse { Message = "Pengguna tidak ditemukan." });
        }
        //TODO Delete User
    }

    public class UserForm
    {
        public string Email { get; set; }
        
    }
    public class UserCreateForm: UserForm
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
    public class UserUpdateForm : UserForm
    {
        public int Id { get; set; }
        public string Username { get; set; }

    }
    public class UserOTPPassword:UserForm
    {
    }
    public class UserResetPassword : UserForm
    {
        public string Password { get; set; }
        public string Otp { get; set; }
    }
    public class UserUpdatePasswordForm
    {
        public int Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }
    public class UserLoginForm : UserForm
    {
        public string Password { get; set; }
    }
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
    public class NewUserEmailModel
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class UserOtpModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Otp { get; set; }
    }
    public class LoginResponse : AppResponse
    {
        public string AccessToken { get; set; }
    }
}
