using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using OverTask.Data;
using OverTask.Models;
using OverTask.Models.InputModels;
using Microsoft.EntityFrameworkCore;

namespace OverTask.Controllers
{
	[ApiController]
	[Route("event")]
	public class PersonalUserEventController : ControllerBase
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AuthenticationController> _logger;
		private readonly ApplicationDbContext _db;

		public PersonalUserEventController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<AuthenticationController> logger, ApplicationDbContext db)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_logger = logger;
			_db = db;
		}

		[HttpPost("save")]
		public async Task<IActionResult> SaveEvents(EventsInformation[] eventInformation)
		{
			if (User?.Identity?.IsAuthenticated != true)
			{
				return Unauthorized();
			}

			var user = await _userManager.GetUserAsync(User);

			if(user == null)
			{
				return BadRequest();
			}


			var currentEvents = _db.PersonalUserEvents.Where(x => x.User == user);
			_db.PersonalUserEvents.RemoveRange(currentEvents);

			foreach(var item in eventInformation)
			{
				foreach (var _event in item.events)
				{
					_db.PersonalUserEvents.Add(new PersonalUserEvent
					{
						Name = _event.title,
						Time  = _event.time,

						Day = item.day,
						Month = item.month,
						Year = item.year,
						
						User = user,
						UserId = user.Id,
					});
				}
			}
			await _db.SaveChangesAsync();
			_logger.LogInformation("Saved data");
			return Ok();
		}
		[HttpGet("get")]
		public async Task<IActionResult> GetEvents()
		{
			if (User?.Identity?.IsAuthenticated != true)
			{
				return Unauthorized();
			}

			var user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return BadRequest();
			}

			List<EventsInformation> eventsInformation = new List<EventsInformation>();

			var userEvents = _db.PersonalUserEvents.Where(x => x.User == user);

			foreach(var _event in userEvents)
			{
				var sameDateEventInfo = eventsInformation.Where(x =>
				x.day == _event.Day &&
				x.month == _event.Month &&
				x.year == _event.Year
				).FirstOrDefault();

				if ( sameDateEventInfo != null)
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
