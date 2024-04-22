using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using OverTask.Models;

namespace OverTask.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<PersonalUserEvent> PersonalUserEvents { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<GroupEvent> GroupEvents { get; set; }
		public DbSet<ToDoTask> ToDoTasks { get; set; }
		public DbSet<ToDoCategory> ToDoCategories { get; set; }
	}
}
