using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OverTask.Data;
using OverTask.Models;
using OverTask.Models.InputModels;
using System;	
using System.Net.WebSockets;
using System.Threading.Tasks.Dataflow;
using System.Xaml.Permissions;

namespace OverTask.Controllers
{
	[ApiController]
	[Route("group")]
	public class GroupController : ControllerBase
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AuthenticationController> _logger;
		private readonly ApplicationDbContext _db;

		public GroupController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AuthenticationController> logger, ApplicationDbContext db)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
			_db = db;
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateGroup(GroupInput groupInput)
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) { return Unauthorized(); }
			if(groupInput.Name.Trim().IsNullOrEmpty()) return BadRequest("Name cant be empty");
			if(groupInput.JoinCode.Trim().IsNullOrEmpty()) 
			{
				Random random = new Random();
				const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
				do
				{
					groupInput.JoinCode = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
				} while (await _db.Groups.AnyAsync(x => x.JoinCode == groupInput.JoinCode));
			}

			var group = new Group
			{
				Name = groupInput.Name,
				JoinCode = groupInput.JoinCode,
				OwnerID = user.Id,
				Owner = user,
				UserIDs = new List<int>() { user.Id },
				PictureURL = "/group_1.jpg"
			};

			if(await _db.Groups.AnyAsync(x => x.JoinCode == group.JoinCode))
			{
				return BadRequest($"Group with JoinCode '{group.JoinCode}' already exists");
			}
			
			await _db.Groups.AddAsync(group);
			await _db.SaveChangesAsync();

			var result = new object();
			var usersInGroup = await _db.ApplicationUsers.Where(x => group.UserIDs.Contains(x.Id))
				.Select(x => new { x.Id, x.UserName, x.ProfilePictureURL })
				.ToListAsync();
			result = new { Group = group, Users = usersInGroup };
			

			return new JsonResult(result);

		}

		[HttpPost("join")]
		public async Task<IActionResult> JoinGroup([FromBody] string joinCode)
		{

			var user = await _userManager.GetUserAsync(User);
			if(user == null)
			{
				return Unauthorized();
			}

			var group = await _db.Groups.FirstOrDefaultAsync(x => x.JoinCode == joinCode);
			if(group == null)
			{
				return BadRequest($"Group with JoinCode '{joinCode}' wasnt found");
			}
			if (group.UserIDs.Contains(user.Id)) return BadRequest("User is already a part of this group");

			group.UserIDs.Add(user.Id);

			_db.Groups.Update(group);
			await _db.SaveChangesAsync();

			var result = new object();
			var usersInGroup = await _db.ApplicationUsers.Where(x => group.UserIDs.Contains(x.Id))
				.Select(x => new { x.Id, x.UserName, x.ProfilePictureURL })
				.ToListAsync();
			result = new { Group = group, Users = usersInGroup };


			return new JsonResult(result);
		}

		[HttpPost("edit")]
		public async Task<IActionResult> EditGroup(GroupEditInput groupInput)
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) return Unauthorized();

			var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupInput.ID);
			if(group == null) return BadRequest("Couldnt find group");
			if (groupInput.Name.Trim().IsNullOrEmpty()) return BadRequest("Name cant be empty");
			if (group.OwnerID != user.Id) return Unauthorized();


			group.Name = groupInput.Name;
			group.PictureURL = groupInput.PictureURL;

			_db.Groups.Update(group);
			await _db.SaveChangesAsync();

			var result = new object();
			var usersInGroup = await _db.ApplicationUsers.Where(x => group.UserIDs.Contains(x.Id))
				.Select(x => new { x.Id, x.UserName, x.ProfilePictureURL })
				.ToListAsync();
			result = new { Group = group, Users = usersInGroup };


			return new JsonResult(result);
		}	

		[HttpPost("leave")]
		public async Task<IActionResult> LeaveGroup([FromBody] int groupId)
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) return Unauthorized();

			var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
			if(group == null) return BadRequest("Couldnt find groupt");

			group.UserIDs.Remove(user.Id);
			

			if(group.UserIDs.Count == 0)
			{
				_db.Groups.Remove(group);
				await _db.SaveChangesAsync();
				return Ok();
			}

			if(group.OwnerID == user.Id)
			{
				var tempMemberID = group.UserIDs.FirstOrDefault();
				group.OwnerID = tempMemberID;
				group.Owner = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == tempMemberID);

			}

			_db.Groups.Update(group);
			await _db.SaveChangesAsync();


			return Ok();
		}

		[HttpDelete("delete")]
		public async Task<IActionResult> DeleteGroup([FromBody] int groupId)
		{
			var user = await _userManager.GetUserAsync(User); 
			
			if(user == null) return Unauthorized();

			var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupId); 
			if (group == null) return BadRequest();
			
			
			if (user.Id != group.OwnerID)
			{
				return Unauthorized();
			}
			
			_db.Groups.Remove(group);
			await _db.SaveChangesAsync();
			return Ok();
			
		}

		[HttpPost("promote")]
		public async Task<IActionResult> PromoteUser([FromBody] int[] data)
		{
			if (data.Length != 2) return BadRequest("Invalid parameters");
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized();
			int groupId = data[0];
			var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupId && x.OwnerID == user.Id);
			if (group == null) return Unauthorized();
			int promoteId = data[1];

			if (!group.UserIDs.Contains(promoteId)) return BadRequest("Couldn't find the user to promote");

			group.OwnerID = promoteId;
			group.Owner = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == promoteId);

			_db.Groups.Update(group);
			await _db.SaveChangesAsync();

			var result = new object();
			var usersInGroup = await _db.ApplicationUsers.Where(x => group.UserIDs.Contains(x.Id))
				.Select(x => new { x.Id, x.UserName, x.ProfilePictureURL })
				.ToListAsync();
			result = new { Group = group, Users = usersInGroup };


			return new JsonResult(result);
		}

		[HttpPost("kick")]
		public async Task<IActionResult> KickUser([FromBody] int[] data)
		{
			if (data.Length != 2) return BadRequest("Invalid parameters");
			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized();
			int groupId = data[0];
			var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupId && x.OwnerID == user.Id);
			if (group == null) return Unauthorized();
			int kickId = data[1];

			if (!group.UserIDs.Contains(kickId)) return BadRequest("Couldn't find the user to kick");


			group.UserIDs.Remove(kickId);
			_db.Groups.Update(group);
			await _db.SaveChangesAsync();

			var result = new object();
			var usersInGroup = await _db.ApplicationUsers.Where(x => group.UserIDs.Contains(x.Id))
				.Select(x => new { x.Id, x.UserName, x.ProfilePictureURL })
				.ToListAsync();
			result = new { Group = group, Users = usersInGroup };


			return new JsonResult(result);
		}
	}
	
}
