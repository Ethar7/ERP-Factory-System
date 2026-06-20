// using ErpFactory.Api.Data;
// using ErpFactory.Api.Services;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using Microsoft.OpenApi;
// using System.Text;
// using System.Text.Json.Serialization;

// var builder = WebApplication.CreateBuilder(args);

// // =========================
// // CONTROLLERS
// // =========================
// builder.Services.AddControllers()
//     .AddJsonOptions(options =>
//         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// // =========================
// // DATABASE
// // =========================
// builder.Services.AddDbContext<ErpFactoryDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// // =========================
// // SERVICES
// // =========================
// builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
// builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
// builder.Services.AddHttpContextAccessor();

// // =========================
// // AUTHENTICATION (JWT BEARER)
// // =========================
// var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
// var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

// if (string.IsNullOrWhiteSpace(jwtOptions.Key))
// {
//     throw new InvalidOperationException("JWT signing key is missing. Configure Jwt:Key in appsettings or environment variables.");
// }

// builder.Services.Configure<JwtOptions>(jwtSection);

// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidIssuer = jwtOptions.Issuer,
//             ValidateAudience = true,
//             ValidAudience = jwtOptions.Audience,
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
//             ValidateLifetime = true,
//             ClockSkew = TimeSpan.FromMinutes(1)
//         };

//         options.Events = new JwtBearerEvents
//         {
//             OnMessageReceived = context =>
//             {
//                 var authorizationHeader = context.Request.Headers.Authorization.ToString();

//                 if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
//                 {
//                     var token = authorizationHeader["Bearer ".Length..].Trim();

//                     if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
//                     {
//                         context.Token = token[1..^1];
//                     }
//                 }

//                 return Task.CompletedTask;
//             }
//         };
//     });

// // =========================
// // AUTHORIZATION (RBAC)
// // =========================
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy =>
//         policy.RequireRole("Admin"));

//     options.AddPolicy("ProjectManagerOnly", policy =>
//         policy.RequireRole("ProjectManager"));

//     options.AddPolicy("InventoryOnly", policy =>
//         policy.RequireRole("InventoryUser"));

//     options.AddPolicy("AccountantOnly", policy =>
//         policy.RequireRole("Accountant"));
// });

// // =========================
// // SWAGGER (SIMPLE - NO OPENAPI ERRORS)
// // =========================
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "ERP Factory API",
//         Version = "v1"
//     });

//     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//     {
//         Name = "Authorization",
//         Type = SecuritySchemeType.Http,
//         Scheme = "bearer",
//         BearerFormat = "JWT",
//         In = ParameterLocation.Header,
//         Description = "Paste only the accessToken value returned from /api/Auth/login, without quotes and without the Bearer prefix."
//     });

//     options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
//     {
//         [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
//     });
// });

// var app = builder.Build();

// // =========================
// // PIPELINE
// // =========================
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// app.UseDefaultFiles();
// app.UseStaticFiles();

// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllers();
// app.MapFallbackToFile("index.html");

// app.Run();
using ErpFactory.Api.Data;
using ErpFactory.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // تم التصحيح: هذا هو الـ Namespace الصحيح
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// =========================
// CORS
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// =========================
// CONTROLLERS
// =========================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// =========================
// DATABASE
// =========================
builder.Services.AddDbContext<ErpFactoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =========================
// SERVICES
// =========================
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddHttpContextAccessor();

// =========================
// JWT CONFIG
// =========================
var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    throw new InvalidOperationException("JWT Key is missing.");
}

builder.Services.Configure<JwtOptions>(jwtSection);

// =========================
// AUTHENTICATION
// =========================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var auth = context.Request.Headers.Authorization.ToString();
                if (auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = auth["Bearer ".Length..].Trim();
                    if (token.Length >= 2 && token[0] == '"' && token[^1] == '"')
                        context.Token = token[1..^1];
                    else
                        context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

// =========================
// AUTHORIZATION
// =========================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("ProjectManagerOnly", p => p.RequireRole("ProjectManager"));
    options.AddPolicy("InventoryOnly", p => p.RequireRole("InventoryUser"));
    options.AddPolicy("AccountantOnly", p => p.RequireRole("Accountant"));
});

// =========================
// SWAGGER (FIXED)
// =========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ERP Factory API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token (e.g. your_token_value)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Custom Middleware Exception Handler for JSON Responses (especially useful for API calls on external servers)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = ex.Message
        });
    }
});

// =========================
// DATABASE AUTOMATIC MIGRATION & VIEWS
// =========================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ErpFactoryDbContext>();
    try
    {
        // Run EF database migrations automatically on startup
        dbContext.Database.Migrate();

        // Create database views if they do not exist
        await dbContext.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'ProjectCostSummary')
            BEGIN
                EXEC('CREATE VIEW ProjectCostSummary AS
                SELECT
                    p.ProjectID,
                    p.ProjectName,
                    p.TotalEstimatedBudget,
                    ISNULL(pc.ProductionDirectCost, 0) AS ProductionDirectCost,
                    ISNULL(sc.SiteDirectCost, 0) AS SiteDirectCost,
                    ISNULL(pc.ProductionDirectCost, 0) + ISNULL(sc.SiteDirectCost, 0) AS TotalDirectCost
                FROM Projects p
                LEFT JOIN (
                    SELECT
                        ProjectID,
                        SUM(LaborCost + MoldDepreciationCost) AS ProductionDirectCost
                    FROM ProductionOrders
                    GROUP BY ProjectID
                ) pc ON pc.ProjectID = p.ProjectID
                LEFT JOIN (
                    SELECT
                        ProjectID,
                        SUM(SupervisorLaborCost + DailyExpenses) AS SiteDirectCost
                    FROM SiteOperations
                    GROUP BY ProjectID
                ) sc ON sc.ProjectID = p.ProjectID')
            END
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'JournalEntryBalance')
            BEGIN
                EXEC('CREATE VIEW JournalEntryBalance AS
                SELECT
                    je.JournalEntryID,
                    je.ReferenceType,
                    je.ReferenceID,
                    SUM(jel.Debit) AS TotalDebit,
                    SUM(jel.Credit) AS TotalCredit,
                    SUM(jel.Debit) - SUM(jel.Credit) AS BalanceDifference
                FROM JournalEntries je
                LEFT JOIN JournalEntryLines jel ON jel.JournalEntryID = je.JournalEntryID
                GROUP BY je.JournalEntryID, je.ReferenceType, je.ReferenceID')
            END
        ");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "حدث خطأ أثناء تهيئة قاعدة البيانات وإنشاء الـ Views تلقائياً.");
    }
}

// =========================
// PIPELINE
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(c => c.Run(async ctx =>
    {
        ctx.Response.StatusCode = 500;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.WriteAsJsonAsync(new { success = false, message = "حدث خطأ داخلي في الخادم." });
    }));
}

app.Run();