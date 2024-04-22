using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OverTask.Data;
using OverTask.Models;
using OverTask.Models.InputModels;

namespace OverTask.Controllers
{
	[ApiController]
	[Route("group/event")]
	public class GroupEventController : ControllerBase
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AuthenticationController> _logger;
		private readonly ApplicationDbContext _db;

		public GroupEventController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AuthenticationController> logger, ApplicationDbContext db)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
			_db = db;
		}

		[HttpPost("save")]
		public async Task<IActionResult> SaveGroupEvent(GroupEventsInformation groupEventsInformation)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null) { return Unauthorized(); }
			var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupEventsInformation.GroupID);
			if (group == null) { return BadRequest("Group Not Found"); }
			if (!group.UserIDs.Contains(user.Id)) { return BadRequest("User is not a part of this group"); }


			var currentEvents = _db.GroupEvents.Where(x => x.GroupId == groupEventsInformation.GroupID);
			_db.GroupEvents.RemoveRange(currentEvents);

			var eventsInformation = groupEventsInformation.eventsInformation;

			foreach (var item in eventsInformation)
			{
				foreach (var _event in item.events)
				{
					_db.GroupEvents.Add(new GroupEvent
					{
						GroupId = group.Id,
						Group = group,

						Name = _event.title,
						Time = _event.time,

						Day = item.day,
						Month = item.month,
						Year = item.year,
						
					});
				}
			}
			_db.SaveChanges();
			_logger.LogInformation("saved group data");
			return Ok();
		}

		[HttpPost("get")]
		public async Task<IActionResult> GetGroupEvents([FromBody] int groupID)
		{
			var user = await _userManager.GetUserAsync(User);
			if(user == null) { return Unauthorized(); }

			var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupID);
			if(group == null) { return BadRequest(); }
			if (!group.UserIDs.Contains(user.Id)) { return BadRequest("User is not a part of this group"); }


			List<EventsInformation> eventsInformation = new List<EventsInformation>();

			var groupEvents = _db.GroupEvents.Where(x => x.GroupId == group.Id);

			foreach (var _event in groupEvents)
			{
				var sameDateEventInfo = eventsInformation.Where(x =>
				x.day == _event.Day &&
				x.month == _event.Month &&
				x.year == _event.Year
				).FirstOrDefault();

				if (sameDateEventInfo != null)
				{
					eventsInformation.Remove(sameDateEventInfo);
					sameDateEventInfo.events = sameDateEventInfo.events.Concat(new PersonalUserEventInput[] {
						new PersonalUserEventInput
						{
							time = _event.Time,
							title = _event.Name
						}
					}).ToArray();
					eventsInformation.Add(sameDateEventInfo);
				}

				else
				{
					eventsInformation.Add(new EventsInformation
					{
						day = _event.Day,
						month = _event.Month,
						year = _event.Year,
						events = new PersonalUserEventInput[]
						{
							new PersonalUserEventInput
							{
								time = _event.Time,
								title = _event.Name
							}
						}
					});
				}
			}
			var result = eventsInformation.ToArray();
			_logger.LogInformation("dispatched data");
			return new JsonResult(new { result });
		}
	}
}
