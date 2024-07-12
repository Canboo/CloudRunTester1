using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("Content-Disposition");
        });
});

builder.Services.AddSingleton<SecretManagerService>();
var secretManagerService = new SecretManagerService();
string connectionString = await secretManagerService.GetConnectionStringAsync(
    Configuration.GetSection("GCP:ProjectId").Get<string>(),
    Configuration.GetSection("GCP:DatabaseIp").Get<string>()
);

builder.Services.AddDbContext<ApiContext>(opt => { opt.UseSqlServer(connectionString); });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<PremSystem>(Configuration.GetSection("PremSystem"));
builder.Services.AddHttpClient()
    .AddHttpClient<DeptHelper>();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
