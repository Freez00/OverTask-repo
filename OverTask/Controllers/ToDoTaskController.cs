using Microsoft.AspNetCore.Identity;
using OverTask.Data;
using OverTask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using OverTask.Models.InputModels;
using Microsoft.IdentityModel.Tokens;

namespace OverTask.Controllers
{
	[ApiController]
	[Route("todo")]
	public class ToDoTaskController : ControllerBase
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AuthenticationController> _logger;
		private readonly ApplicationDbContext _db;

		public ToDoTaskController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AuthenticationController> logger, ApplicationDbContext db)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
			_db = db;
		}

		[HttpGet("get")]
		public async Task<IActionResult> GetTasks()
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) { return Unauthorized(); }

			var tasks = await _db.ToDoTasks.Where(x => x.UserID == user.Id).ToListAsync();
			var categories = await _db.ToDoCategories.Where(x => x.UserID == user.Id).ToListAsync();


			var result = new List<object>();

			foreach(var category in categories)
			{
				var tasksInCategory = tasks.Where(x => category.TaskIDs.Contains(x.Id)).ToList();
				tasksInCategory.ForEach(x => tasks.Remove(x));

				result.Add(new {
					Category = new {
						Id = category.Id,
						Title = category.Title,
						Color = category.Color
					},
					Tasks = tasksInCategory.Select(task => new
					{
						Id = task.Id,
						Title = task.Title,
						Completed = task.Completed
					})
				});
			}


			
			result.Add(new
			{
				Category = new
				{
					Id = -1,
					Title = "Без категория",
					Color = ""
				},
				Tasks = tasks.Select(task => new
				{
					Id = task.Id,
					Title = task.Title,
					Completed = task.Completed
				}).ToArray()
			});
			

			return new JsonResult(result);
		}

		
		[HttpPost("create")]
		public async Task<IActionResult> CreateTask([FromBody] TaskInputModel input)
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) { return Unauthorized(); }
			if (input.Title.Trim().IsNullOrEmpty()) { return BadRequest("Title cannot be empty"); }


			var task = new ToDoTask { Title = input.Title, Completed = false, UserID = user.Id, user = user};
			await _db.ToDoTasks.AddAsync(task);
			await _db.SaveChangesAsync();

			if(input.CategoryID != null && input.CategoryID != -1)
			{
				var category = await _db.ToDoCategories.FirstOrDefaultAsync(x => x.Id == input.CategoryID);
				if(category == null) { return BadRequest("Category not found"); }

				category.TaskIDs.Add(task.Id);
			}
			
			await _db.SaveChangesAsync();

			return new JsonResult(task);
		}

		
		[HttpPost("update")]
		public async Task<IActionResult> UpdateTask([FromBody] TaskInputModel input)
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) { return Unauthorized(); }

			var task = await _db.ToDoTasks.FirstOrDefaultAsync(x => x.Id == input.Id && x.UserID == user.Id);
			if(task == null) { return BadRequest("Task not found"); }

			if(input.Title.Trim().IsNullOrEmpty()) { return BadRequest("Title cannot be empty"); }

			task.Title = input.Title;
			task.Completed = input.Completed;


			_db.ToDoTasks.Update(task);
			await _db.SaveChangesAsync();

			return Ok();
		}

		[HttpDelete("delete")]
		public async Task<IActionResult> DeleteTask([FromBody] int taskID)
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) { return Unauthorized(); }

			var task = await _db.ToDoTasks.FirstOrDefaultAsync(x => x.Id == taskID && x.UserID == user.Id);
			if(task == null) { return BadRequest("Task not found"); }

			await _db.ToDoCategories.ForEachAsync(category =>
			{
				if(category.TaskIDs.Contains(taskID))
				{
					category.TaskIDs.Remove(taskID);
				}
			});

			_db.ToDoTasks.Remove(task);
			await _db.SaveChangesAsync();

			return Ok();
		}
	}
}
