using OverTask.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.Identity.Client;
using OverTask.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OverTask.Services;
using OverTask.Utilities;

internal class Program
{
	private static void Main(string[] args)
	{

		var builder = WebApplication.CreateBuilder(args);

		var SveltePolicy = "_sveltePolicy";

		var jwtKey = builder.Configuration.GetValue<string>("JwtKey");
		var jwtIssuer = builder.Configuration.GetValue<string>("JwtIssuer");

		// Add services to the container.
		//var connectionString = builder.Configuration.GetConnectionString("OverTaskDbConnection") ?? throw new InvalidOperationException("Connection string 'OverTaskDbConnection' not found.");
		var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(connectionString));
		builder.Services.AddDatabaseDeveloperPageExceptionFilter();

		builder.Services.AddCors(options => {
			options.AddPolicy(name: SveltePolicy,
				policy => {
					policy.WithOrigins(
						"https://overtask.vercel.app",
						"http://localhost:5027"
					)
					.AllowAnyHeader()
					.AllowCredentials()
					.AllowAnyMethod();
					/*policy.WithOrigins("http://localhost:5027").AllowAnyHeader().AllowCredentials().AllowAnyMethod();
					policy.WithOrigins("https://overtask.vercel.app").AllowAnyHeader().AllowCredentials().AllowAnyMethod();*/
				});
		});

		builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options => {
			options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;
			options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
			options.ClaimsIdentity.EmailClaimType = ClaimTypes.Email;
		}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
		{
			options.RequireHttpsMetadata = false; // Set to true in production
			options.SaveToken = true;
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtIssuer,
				ValidAudience = jwtIssuer,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
				ClockSkew = TimeSpan.FromDays(2)
			};
		});

		builder.Services.AddControllersWithViews();

		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<IDbInitializer, DbInitializer>();


		builder.Services.AddRazorPages();


		//SWAGGER
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1.0", new OpenApiInfo{ Title = "OvertaskAPI", Version = "1.0" });
		});


		var app = builder.Build();



		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseMigrationsEndPoint();
		}
		else
		{
			app.UseExceptionHandler("/Home/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		//SWAGGER
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "OverTaskApi v1.0");
		});

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseCors(SveltePolicy);

		
		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");

		app.MapRazorPages();

		//run only once to seed the database

		//using (var scope = app.Services.CreateScope())
		//{
		//	var dbinitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		//	dbinitializer.Initialize();
		//}

		app.Run();
	}

}