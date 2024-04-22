using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OverTask.Models
{
	public class GroupEvent
	{
		[Key]
		public int Id { get; set; }

		public int GroupId { get; set; }
		[ForeignKey("GroupId")]
		public Group Group { get; set; }
		public string Name { get; set; }
		public string Time { get; set; }
		public int Day { get; set; }
		public int Month { get; set; }
		public int Year { get; set; }
	}


}
