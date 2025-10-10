using Application;
using Application.Helpers;
using Application.IRepositories;
using Application.IServices;
using Application.Services;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddDbContext<Context>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
    });
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(options =>
            {
                IConfiguration config = builder.Configuration; // Correct way to access the configuration
                var secretKey = config["Jwt:Key"];

                // Check if secretKey is null or empty
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    throw new InvalidOperationException("JWT secret key is missing in the configuration.");
                }

                var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
                if (secretKeyBytes.Length != 32)
                {
                    secretKeyBytes = System.Security.Cryptography.SHA256.HashData(secretKeyBytes);
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes)
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var exception = context.Exception;
                        Console.WriteLine("Token validation failed: " + exception.Message);
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        // Allow SignalR to read JWT from query string
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notification"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "OCMS.API",
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. " +
                      "\n\nEnter your token in the text input below. " +
                      "\n\nExample: '12345abcde'",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                        Array.Empty<string>()
                    }
                });
});
builder.Services.AddScoped<JwtTokenHelper>();
builder.Services.AddScoped<IUnitOfWork>(sp =>
{
    var context = sp.GetRequiredService<Context>();
    return new UnitOfWork(
        context,
        sp.GetRequiredService<IUserRepository>(),
        sp.GetRequiredService<ISessionRepository>(),
        sp.GetRequiredService<ICertificateRepository>(),
        sp.GetRequiredService<ICertificateTemplateRepository>(),
        sp.GetRequiredService<IClassRepository>(),
        sp.GetRequiredService<IClassTraineeAssignationRepository>(),
        sp.GetRequiredService<ICourseRepository>(),
        sp.GetRequiredService<ICourseCertificateRepository>(),
        sp.GetRequiredService<ICourseSubjectSpecialtyRepository>(),
        sp.GetRequiredService<IDecisionRepository>(),
        sp.GetRequiredService<IDecisionTemplateRepository>(),
        sp.GetRequiredService<IDepartmentRepository>(),
        sp.GetRequiredService<IExternalCertificateRepository>(),
        sp.GetRequiredService<IInstructorAssignationRepository>(),
        sp.GetRequiredService<INotificationRepository>(),
        sp.GetRequiredService<IPlanRepository>(),
        sp.GetRequiredService<IPlanCertificateRepository>(),
        sp.GetRequiredService<IStudyRecordRepository>(),
        sp.GetRequiredService<IReportRepository>(),
        sp.GetRequiredService<IRequestRepository>(),
        sp.GetRequiredService<IRoleRepository>(),
        sp.GetRequiredService<ISpecialtyRepository>(),
        sp.GetRequiredService<ISubjectRepository>(),
        sp.GetRequiredService<ISubjectCertificateRepository>(),
        sp.GetRequiredService<ITraineeAssignationRepository>()
    );
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<ICertificateTemplateRepository, CertificateTemplateRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassTraineeAssignationRepository, ClassTraineeAssignationRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseCertificateRepository, CourseCertificateRepository>();
builder.Services.AddScoped<ICourseSubjectSpecialtyRepository, CourseSubjectSpecialtyRepository>();
builder.Services.AddScoped<IDecisionRepository, DecisionRepository>();
builder.Services.AddScoped<IDecisionTemplateRepository, DecisionTemplateRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IExternalCertificateRepository, ExternalCertificateRepository>();
builder.Services.AddScoped<IInstructorAssignationRepository, InstructorAssignationRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IPlanCertificateRepository, PlanCertificateRepository>();
builder.Services.AddScoped<IStudyRecordRepository, StudyRecordRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectCertificateRepository, SubjectCertificateRepository>();
builder.Services.AddScoped<ITraineeAssignationRepository, TraineeAssignationRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IExternalCertificateService, ExternalCertificateService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR Hub
app.MapHub<Certificate_Management_BE.Hubs.NotificationHub>("/hubs/notification");

app.MapControllers();

// Runtime seeding
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Context>();
    await DataSeeder.SeedAsync(db);
}

app.Run();
