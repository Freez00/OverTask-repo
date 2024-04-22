using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OverTask.Models
{
	public class ToDoCategory
	{
		[Key]
		public int Id { get; set; }

		public string Title { get; set; }

		public string Color { get; set; } = "#darkorchid";

		public List<int> TaskIDs { get; set; }

		[ForeignKey("UserID")]
		public int UserID { get; set; }

		public ApplicationUser user { get; set; }
	}
}
