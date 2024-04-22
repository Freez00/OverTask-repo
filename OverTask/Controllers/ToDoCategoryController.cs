using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OverTask.Data;
using OverTask.Models;
using OverTask.Models.InputModels;

namespace OverTask.Controllers
{
	[ApiController]
	[Route("todo/category")]
	public class ToDoCategoryController : ControllerBase
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AuthenticationController> _logger;
		private readonly ApplicationDbContext _db;

		public ToDoCategoryController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AuthenticationController> logger, ApplicationDbContext db)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
			_db = db;
		}
		
		[HttpGet("get")]
		public async Task<IActionResult> GetCategories()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			var categories = await _db.ToDoCategories.Where(x => x.UserID == user.Id)
				.Select(x => new {x.Id, x.Title, x.Color}).ToArrayAsync();
			return new JsonResult(categories);
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateCategory([FromBody] RenameCategoryData data)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }
			if (data.Title.Trim().IsNullOrEmpty()) { return BadRequest("Title cannot be empty"); }

			var category = new ToDoCategory { Title = data.Title, Color = data.Color, TaskIDs = new List<int>(), UserID = user.Id, user = user };
			await _db.ToDoCategories.AddAsync(category);
			await _db.SaveChangesAsync();

			return Ok();
		}

		[HttpDelete("delete")]
		public async Task<IActionResult> DeleteCategory([FromBody] int categoryID)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			var category = await _db.ToDoCategories.FirstOrDefaultAsync(x => x.Id == categoryID && x.user == user);
			if (category == null) { return BadRequest("Category not found"); }

			category.TaskIDs.ForEach(async x =>
			{
				_db.ToDoTasks.Remove(_db.ToDoTasks.FirstOrDefault(task => task.Id == x));
			});


			_db.ToDoCategories.Remove(category);
			await _db.SaveChangesAsync();

			return Ok();
		}

		[HttpPost("rename")]
		public async Task<IActionResult> RenameCategory([FromBody] RenameCategoryData data)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }

			var category = await _db.ToDoCategories.FirstOrDefaultAsync(x => x.Id == data.CategoryID && x.user == user);
			if (category == null) { return BadRequest("Category not found"); }
			if (data.Title.Trim().IsNullOrEmpty()) { return BadRequest("Title cannot be empty"); }


			category.Title = data.Title;
			category.Color = data.Color;

			_db.ToDoCategories.Update(category);
			await _db.SaveChangesAsync();

			return Ok();
		}
	}
}
