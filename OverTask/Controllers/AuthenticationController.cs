using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OverTask.Models;
using OverTask.Models.InputModels;
using OverTask.Services;
using System.Web;

namespace OverTask.Controllers
{
	[Route("authenticate")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{

		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AuthenticationController> _logger;
		private readonly ITokenService _tokenService;

		public AuthenticationController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AuthenticationController> logger
		, ITokenService tokenService)
		{
			_tokenService = tokenService;
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] AuthResource resource)
		{
			var user = new ApplicationUser
			{
				Id = 0,
				UserName = resource.UserName,
				Email = resource.Email,
				ProfilePictureURL = "/user_1.jpg"
			};

			var result = await _userManager.CreateAsync(user, resource.Password);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			var token = await _tokenService.GenerateToken(user);
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = DateTime.UtcNow.AddDays(2)
			};

			Response.Cookies.Append("token", token, cookieOptions);

			return new JsonResult(new { token });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] AuthResource resource)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var user = await _userManager.FindByEmailAsync(resource.Email);
			if (user == null)
			{
				return Unauthorized();
			}
			if (!await _userManager.CheckPasswordAsync(user, resource.Password))
			{
				return Unauthorized();
			}

			var token = await _tokenService.GenerateToken(user);

			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = DateTime.UtcNow.AddDays(2)
			};

			Response.Cookies.Append("token", token, cookieOptions);

			return new JsonResult(new { token });

		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			Response.Cookies.Delete("token");
			_logger.LogInformation("User logged out");

			return Ok();
		}
	}
}
