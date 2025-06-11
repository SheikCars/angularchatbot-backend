// -------------------------------------------------------------------
// Program.cs (Corrected for ASP.NET Core 6/7)
// -------------------------------------------------------------------
using ChatbotApi.Data;
using ChatbotApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// --- Configure Services ---

// 1. Add DbContext with SQL Server
builder.Services.AddDbContext<ChatContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Register NLP service with HttpClient
builder.Services.AddHttpClient<INlpService, NlpService>();

// 3. Add Controllers
builder.Services.AddControllers();

// 4. Enable CORS for Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200")
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

// 5. Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chatbot API",
        Version = "v1",
        Description = "Simple chatbot backend for Angular integration"
    });
});

var app = builder.Build();

// --- Configure Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chatbot API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
