using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using GuessingGame.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.Core.Domain.Image.Services;
using MediatR;
using GuessingGame.SharedKernel;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Hubs;
using GuessingGame.Hubs.Services;

namespace GuessingGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   
            //  services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));
             services.AddDistributedMemoryCache();
			 services.AddSession(options =>
			 {
			 	options.IdleTimeout = TimeSpan.FromSeconds(60); // We're keeping this low to facilitate testing. Would normally be higher. Default is 20 minutes
			 	options.Cookie.IsEssential = true;              // Otherwise we need cookie approval
			 });
             services.AddHttpContextAccessor();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddSignalR();
            services.AddRazorPages();
            services.AddMediatR(typeof(Startup));
            // services.AddScoped<IGameCreation, GameCreation>();
            services.AddScoped<IOracleService, OracleService>();
            services.AddTransient<IFileUploader, FileUploader>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IGameController, GameController>();
            services.AddTransient<IImageCombiner, ImageCombiner>();
            // services.AddScoped<ApplicationDbContext>();
            services.AddScoped(typeof(ApplicationDbContext));

            services.AddTransient<IGameCreation, GameCreation>();
            // services.AddSingleton<IGameCreation, GameCreation>();
            services.AddTransient<IGameValidator, GameValidator>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext Db, IMediator mediator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();

                if(!Db.Images.Any())
                {   
                    var newFileReader = new FileReader();
                    var ICombiner = new ImageCombiner(Db, mediator);
                    newFileReader.ReadFile();
                    Db.Images.AddRange(newFileReader.Images);
                    Db.SaveChangesAsync();
                    ICombiner.Startup();
                }
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
           


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            // app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<NotificationHub>("/notificationhub");
            });
        }
    }
}
