using GrowsmartAPI.Data;
using GrowsmartAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5050");

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "GrowsmartAPI", Version = "v1" });
    // Use FullName for schema IDs to avoid name collisions across controllers
    c.CustomSchemaIds(type => type.FullName);
    // Resolve any remaining route ambiguities by picking the first matching action
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

// Fix missing wwwroot when running from bin folder
var contentRoot = builder.Environment.ContentRootPath;
var wwwrootPath = Path.Combine(contentRoot, "wwwroot");
if (!Directory.Exists(wwwrootPath))
{
    var projectRoot = Path.GetFullPath(Path.Combine(contentRoot, "..", "..", ".."));
    var projectWwwroot = Path.Combine(projectRoot, "wwwroot");
    if (Directory.Exists(projectWwwroot))
    {
        builder.Environment.WebRootPath = projectWwwroot;
    }
}

// Configure Entity Framework MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configure large payload handling
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100_000_000; // 100MB
    options.ValueLengthLimit = 100_000_000;
    options.MemoryBufferThreshold = 100_000_000;
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100_000_000; // 100MB
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<IPlantIdentityService, PlantIdService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();

var app = builder.Build();

// Automatically apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;git 
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        context.Database.Migrate();

        // Seed Plant Species if empty
        if (!context.Species.Any())
        {
            context.Species.AddRange(SeedData.GetInitialSpecies());
            context.SaveChanges();
            logger.LogInformation("Seeded Plant Species library.");
        }

        // Seed Common Diseases if empty
        if (!context.CommonDiseases.Any())
        {
            context.CommonDiseases.AddRange(SeedData.GetInitialDiseases());
            context.SaveChanges();
            logger.LogInformation("Seeded Common Diseases library.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        // If it's a "Duplicate column" error, the DB is already in the right state, so we can ignore it
        if (ex.ToString().Contains("Duplicate column name"))
        {
            logger.LogWarning("Database migration conflict (column already exists): " + ex.Message + ". Continuing since DB is already updated.");
        }
        else
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            // We can still try to continue if it's just a migration issue
        }
    }
}

app.UseRouting();
app.UseCors("AllowAll"); // Apply CORS after routing but before auth
app.UseStaticFiles();


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // Disable HTTPS redirection in dev to avoid protocol mismatch on port 5050
}
else 
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();


app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var conn = dbContext.Database.GetDbConnection();
    await conn.OpenAsync();
    using var command = conn.CreateCommand();
    command.CommandText = @"
        CREATE TABLE IF NOT EXISTS PasswordResetOtps (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Email VARCHAR(255) NOT NULL,
            OtpCode VARCHAR(255) NOT NULL,
            ExpiryTime DATETIME(6) NOT NULL,
            IsUsed TINYINT(1) NOT NULL DEFAULT 0,
            CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6)
        );
    ";
    await command.ExecuteNonQueryAsync();
}

app.Run();
