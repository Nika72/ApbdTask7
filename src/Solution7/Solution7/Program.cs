using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Ensure this namespace is included for configuration access
using Solution7.Repositories; // Assuming your repository interfaces and implementations are in this namespace
using Solution7.Services; // Ensure this namespace contains your service interfaces and implementations

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register repository interfaces and their implementations
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();

// Register OrderRepository with connection string from configuration
builder.Services.AddScoped<IOrderRepository>(provider => 
    new OrderRepository(provider.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"))
);

// Add WarehouseService to the DI container for use in controllers
// Ensure IWarehouseService and WarehouseService are correctly registered
builder.Services.AddScoped<IWarehouseService, WarehouseService>(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();