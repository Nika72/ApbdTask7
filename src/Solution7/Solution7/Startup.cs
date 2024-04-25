using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Solution7.Repositories;
using Solution7.Services;
using Solution7.Settings;  // This is required if you use DatabaseSettings elsewhere, not needed for connection string configuration directly

namespace Solution7
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DatabaseSettings>(Configuration.GetSection("ConnectionStrings"));
            var dbSettings = Configuration.GetSection("ConnectionStrings").Get<DatabaseSettings>();

            if (string.IsNullOrEmpty(dbSettings?.DefaultConnection))
            {
                throw new InvalidOperationException(
                    "Database connection string 'DefaultConnection' is not configured.");
            }

            // Register the repositories with the actual connection string
            services.AddScoped<IProductRepository>(_ => new ProductRepository(dbSettings.DefaultConnection));
            services.AddScoped<IWarehouseRepository>(_ => new WarehouseRepository(dbSettings.DefaultConnection));
            services.AddScoped<IOrderRepository>(_ => new OrderRepository(dbSettings.DefaultConnection));

            // Register other services
            services.AddScoped<IWarehouseService, WarehouseService>();

            services.AddControllers();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
