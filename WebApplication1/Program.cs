using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using PdfSharp.Fonts;
using WebApplication1.Controllers;
using WebApplication1.Repositories;
using Google.Cloud.SecretManager.V1;
using Newtonsoft.Json.Linq;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            string pathToKeyFile = builder.Environment.ContentRootPath + "msd63a2024-d05b0e7fc641.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToKeyFile);

            //access the secret vaults
            
            string project = builder.Configuration["project"].ToString(); //reads the project id from appsettings.json

   
            string secretId = "mykeys"; //recommended way would be to read these details from appsettings.json
            string secretVersionId = "1";
         
                // Create the client.
                SecretManagerServiceClient client = SecretManagerServiceClient.Create();

                // Build the resource name.
                SecretVersionName secretVersionName = new SecretVersionName(project, secretId, secretVersionId);

                // Call the API.
                AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

                // Convert the payload to a string. Payloads are bytes by default.
                String payload = result.Payload.Data.ToStringUtf8();

                dynamic jsonObject = JObject.Parse(payload);


                string clientId = jsonObject["Authentication:Google:ClientId"].ToString();
                string clientSecret = jsonObject["Authentication:Google:ClientSecret"].ToString();
                string redisConnection = jsonObject["redis"].ToString();


            builder.Services
                     .AddAuthentication(options =>
                     {
                         options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                         options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                     })
                     .AddCookie()
                     .AddGoogle(options =>
                     {
                         options.ClientId = clientId; //builder.Configuration["Authentication:Google:ClientId"];
                         options.ClientSecret = clientSecret;//builder.Configuration["Authentication:Google:ClientSecret"];
                     });


            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                   CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                   CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });





            builder.Services.AddRazorPages();

         
            string bucket = builder.Configuration["bucket"].ToString();

            //Services is a collection holding all the initialized services (i.e a pool of services) so when there's a
            //controller asking for an instance of a particular class, it exists and the Injector class can give it to it
            builder.Services.AddScoped<BlogsRepository>(x=>new BlogsRepository(project));
            builder.Services.AddScoped<PostsRepository>(x => new PostsRepository(project));
            builder.Services.AddScoped<BucketsRepository>(x => new BucketsRepository(project, bucket));
            builder.Services.AddScoped<PubsubRepository>(x => new PubsubRepository(project, "msd63a2024ra"));
            builder.Services.AddScoped<IFontResolver,FileFontResolver>();
            builder.Services.AddScoped<RedisRepository>(x=> new RedisRepository(redisConnection));
            builder.Services.AddScoped<LogsRepository>(x => new LogsRepository(project));



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

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }


        private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here.
                if (true)
                {
                    // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

    }
}