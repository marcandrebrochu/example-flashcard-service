using FlashcardService.Application;
using FlashcardService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql( /*builder.Configuration.GetConnectionString("DefaultConnection")*/"");
});

builder.AddApplicationServices();

builder.Logging.AddFilter("LuckyPennySoftware.MediatR.License", LogLevel.None);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
