using Ayna.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region MVC Services
builder.Services.AddControllersWithViews();
#endregion

#region Database Configuration
builder.Services.AddDbContext<AynaDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("AynaConnection");

    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        // Connection resilience
        sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        );

        // Command timeout
        sqlServerOptions.CommandTimeout(60);
    });

    // Enable detailed EF Core logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
#endregion

#region Authentication Configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Path configuration
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";

        // Cookie expiration
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true; // Reset expiration on activity

        // Cookie security
        options.Cookie.Name = "Ayna.Auth";
        options.Cookie.HttpOnly = true; // Prevent JavaScript access
        options.Cookie.IsEssential = true; // Required for GDPR
        options.Cookie.SameSite = SameSiteMode.Lax; // CSRF protection

        // HTTPS only in production
        if (!builder.Environment.IsDevelopment())
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        }

        // Events for validation
        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                // Check if user still exists and is active
                var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var dbContext = context.HttpContext.RequestServices
                        .GetRequiredService<AynaDbContext>();

                    var userExists = await dbContext.Users
                        .AnyAsync(u => u.UserId == int.Parse(userId) && u.UserStatus == "Active");

                    if (!userExists)
                    {
                        context.RejectPrincipal();
                        await context.HttpContext.SignOutAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme);
                    }
                }
            }
        };
    });
#endregion

#region Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // User type policies
    options.AddPolicy("FarmerOnly", policy =>
        policy.RequireClaim("UserType", "Farmer"));

    options.AddPolicy("CharityOnly", policy =>
        policy.RequireClaim("UserType", "Charity"));

    options.AddPolicy("DonorOnly", policy =>
        policy.RequireClaim("UserType", "Donor"));

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("Role", "Admin"));

    // Combined policies
    options.AddPolicy("FarmerOrCharity", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("UserType", "Farmer") ||
            context.User.HasClaim("UserType", "Charity")));

    options.AddPolicy("DonorOrCharity", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("UserType", "Donor") ||
            context.User.HasClaim("UserType", "Charity")));
});
#endregion

#region Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.Name = "Ayna.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    if (!builder.Environment.IsDevelopment())
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
});

// Add distributed memory cache for session storage
builder.Services.AddDistributedMemoryCache();
#endregion

#region Infrastructure Services
// HTTP Context Accessor (for accessing HttpContext in services)
builder.Services.AddHttpContextAccessor();

// Memory Cache
builder.Services.AddMemoryCache();

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
#endregion

#region Logging Configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure log levels
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
#endregion

// Build the application
var app = builder.Build();

#region Exception Handling & Error Pages
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Front/Error");
    app.UseHsts();
    app.UseStatusCodePagesWithReExecute("/Front/Error", "?statusCode={0}");
}
#endregion

#region Custom Middleware - Early Stage
app.UseErrorLogging();
app.UseSecurityHeaders();
app.UseHttpsEnforcement();
// Request/Response logging (disable in production for performance)
if (app.Environment.IsDevelopment())
{
    app.UseRequestResponseLogging();
}

// Response compression
app.UseResponseCompression();
#endregion

#region Standard ASP.NET Core Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
#endregion

#region Session & Authentication
app.UseSession();
app.UseSessionTimeout(120);
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region Endpoint Mapping

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Front}/{action=Home}/{id?}");

app.MapControllerRoute(
    name: "auth",
    pattern: "Auth/{action=Login}/{id?}",
    defaults: new { controller = "Auth" });

app.MapControllerRoute(
    name: "front",
    pattern: "Front/{action=Home}/{id?}",
    defaults: new { controller = "Front" });

app.MapControllerRoute(
    name: "farmer",
    pattern: "Farmer/{action=Index}/{id?}",
    defaults: new { controller = "Farmer" });

app.MapControllerRoute(
    name: "charity",
    pattern: "Charity/{action=Index}/{id?}",
    defaults: new { controller = "Charity" });

app.MapControllerRoute(
    name: "donor",
    pattern: "Donor/{action=Index}/{id?}",
    defaults: new { controller = "Donor" });
#endregion

// Log application startup
app.Logger.LogInformation("Ayna application started successfully");
app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

// Run the application
app.Run();