using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using WebApplication1.Repositories;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            string pathToKeyFile = builder.Environment.ContentRootPath + "msd63a2024-d05b0e7fc641.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToKeyFile);



            builder.Services
                     .AddAuthentication(options =>
                     {
                         options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                         options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                     })
                     .AddCookie()
                     .AddGoogle(options =>
                     {
                         options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                         options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                     });

            builder.Services.AddRazorPages();

            string project = builder.Configuration["project"].ToString();

            //Services is a collection holding all the initialized services (i.e a pool of services) so when there's a
            //controller asking for an instance of a particular class, it exists and the Injector class can give it to it
            builder.Services.AddScoped<BlogsRepository>(x=>new BlogsRepository(project));
            builder.Services.AddScoped<PostsRepository>(x => new PostsRepository(project));




            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}