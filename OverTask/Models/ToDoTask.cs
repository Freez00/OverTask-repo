using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OverTask.Models
{
	public class ToDoTask
	{
		[Key] 
		public int Id { get; set; }

		[Required]
		public string Title { get; set; }

		[Required]
		public bool Completed { get; set; }

		[ForeignKey("UserID")]
		public int UserID { get; set; }

		public ApplicationUser user { get; set; }

	}
}
