using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.DataAccess.MongoDb.Abstract;
using Core.DataAccess.MongoDb.Concrete;
using Core.DependencyResolvers;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Encryption;
using Core.Utilities.IoC;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace WebApplication
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WebApplication", Version = "v1"});
            });
            services.AddDependencyResolvers(new ICoreModule[]
            {
                new CoreModule()
            });
            services.AddTransient<FileLogger>();
            services.AddTransient<MongoDbLogger>();
            services.AddTransient<ElasticsearchLogger>();

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["TokenOptions:Issuer"],
                        ValidAudience = Configuration["TokenOptions:Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            SecurityKeyHelper.CreateSecurityKey(Configuration["TokenOptions:SecurityKey"])
                    };
                });

            services.Configure<MongoDbSettings>(
                Configuration.GetSection(nameof(MongoDbSettings)));
            services.AddSingleton<IMongoDbSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ServiceTool.ServiceProvider = app.ApplicationServices;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication v1"));
            }

            app.UseHttpsRedirection();
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials
            app.UseRouting();

            //Authentication comes before Authorization.
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}