// Program.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using QuestionBankPOC.Data;
using QuestionBankPOC.Services;
using QuestionBankPOC.Middleware;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Firebase Configuration
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("qbpoc-10050-firebase-adminsdk-fbsvc-e3dd07861f.json"),
});

// JWT Authentication Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{builder.Configuration["Firebase:ProjectId"]}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{builder.Configuration["Firebase:ProjectId"]}",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Firebase:ProjectId"],
            ValidateLifetime = true
        };

        // Enable debug output to see the claims being validated
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context => 
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("JWT Token validated successfully");
                
                // Log the incoming claims for debugging
                var claims = context.Principal?.Claims.ToList();
                if (claims != null)
                {
                    foreach (var claim in claims)
                    {
                        logger.LogInformation($"Original Claim: {claim.Type}={claim.Value}");
                    }
                }
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // More flexible admin policy
    options.AddPolicy("AdminOnly", policy => 
    {
        policy.RequireAssertion(context =>
        {
            var httpContext = context.Resource as HttpContext;
            var logger = httpContext?.RequestServices.GetService<ILogger<Program>>();
                
            // Check for multiple claim types that might contain the role
            bool isAdmin = context.User.HasClaim(c => c.Type == "role" && c.Value == "admin") || 
                          context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "admin") ||
                          context.User.HasClaim(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == "admin");
            
            if (logger != null)
            {
                logger.LogInformation($"AdminOnly policy evaluation: isAdmin = {isAdmin}");
                foreach (var claim in context.User.Claims)
                {
                    logger.LogInformation($"Claim in policy check: {claim.Type} = {claim.Value}");
                }
            }
            
            return isAdmin;
        });
    });
    
    // More flexible evaluator or admin policy
    options.AddPolicy("EvaluatorOrAdmin", policy =>
    {
        policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(c => c.Type == "role" && (c.Value == "admin" || c.Value == "evaluator")) ||
                   context.User.HasClaim(c => c.Type == ClaimTypes.Role && (c.Value == "admin" || c.Value == "evaluator"));
        });
    });
    
    // More flexible all roles policy
    options.AddPolicy("AllRoles", policy =>
    {
        policy.RequireAssertion(context =>
        {
            return context.User.HasClaim(c => c.Type == "role" && (c.Value == "admin" || c.Value == "evaluator" || c.Value == "candidate")) ||
                   context.User.HasClaim(c => c.Type == ClaimTypes.Role && (c.Value == "admin" || c.Value == "evaluator" || c.Value == "candidate"));
        });
    });
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJS", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Register custom services
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowNextJS");
app.UseAuthentication();
// Add our custom Firebase middleware after authentication but before authorization
app.UseFirebaseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
