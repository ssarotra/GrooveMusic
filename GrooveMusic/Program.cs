using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using GrooveMusic.SetupClass;
using GrooveMusic.Repositories;
using GrooveMusic.Database;
using GrooveMusic.Service;

var builder = WebApplication.CreateBuilder(args);


// Bind MongoDBSettings from appsettings.json
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));

// Register MongoDBContext
builder.Services.AddSingleton<MongoDBContext>();

// Register Repositories
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<SessionRepository>();

// Register Services
builder.Services.AddSingleton<AuthService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
