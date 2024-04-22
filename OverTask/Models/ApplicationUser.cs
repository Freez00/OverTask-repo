using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace OverTask.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Key]
        public override int Id { get; set; }

		[Required]
		public override string UserName { get; set;}

        public string ProfilePictureURL { get; set; }
	}
}
