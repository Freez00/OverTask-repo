using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OverTask.Models
{
	public class Group
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }

		[Required]
		public string JoinCode { get; set; }
			
		public string PictureURL { get; set; }

		public List<int> UserIDs { get; set; }
		public int OwnerID { get; set; }
		[ForeignKey("OwnerID")]
		public ApplicationUser Owner { get; set; }
	}
}
