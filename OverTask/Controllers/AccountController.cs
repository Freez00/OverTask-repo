using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OverTask.Data;
using OverTask.Models;

namespace OverTask.Controllers
{

	[Route("account")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger<AccountController> _logger;
		private readonly ApplicationDbContext _db;


		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger, ApplicationDbContext db)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			_db = db;
		}
		

		[HttpGet("getUserId")]
		public async Task<IActionResult> GetUserId()
		{
			var user = await _userManager.GetUserAsync(User);
			
			if (user == null) return Unauthorized();

			int userID = user.Id;
			return new JsonResult(new { userID });
		}

		[HttpGet("getUsername")]

		public async Task<IActionResult> GetUsername()
		{
			if (User?.Identity?.IsAuthenticated == true)
			{
				foreach(var claim in User.Claims)
				{
					_logger.LogInformation(claim.Type + " " + claim.Value);
				}
			}
			
			var username = HttpContext?.User?.Identity?.Name?.ToString();

			if(username == null)
			{
				return BadRequest("No user is logged in");
			}

			return new JsonResult(new { username });
		}

		[HttpGet("getGroups")]
		public async Task<IActionResult> GetGroups()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			var groups = await _db.Groups.Where(x => x.UserIDs.Contains(user.Id)).ToListAsync();

			var result = new List<object>();

			foreach (var group in groups)
			{
				var usersInGroup = await _db.ApplicationUsers.Where(x => group.UserIDs.Contains(x.Id))
					.Select(x => new { x.Id, x.UserName, x.ProfilePictureURL})
					.ToListAsync();
				result.Add(new { Group = group, Users = usersInGroup });
			}

			return new JsonResult(result);
		}

		[HttpGet("serverStatus")]
		public IActionResult ServerStatus()
		{
			return Ok("Server is running");
		}
	}
}
