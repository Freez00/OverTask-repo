using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OverTask.Models
{
	public class PersonalUserEvent
	{
		[Key]
		public int Id { get; set; }
		public int UserId { get; set; }
		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; }

		public string Name { get; set; }
		public string Time { get; set; }
		public int Day { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }
	}
}
