using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PubQuizBackend.Auth.RoleAndAudience;
using PubQuizBackend.Enums;
using PubQuizBackend.Model;
using PubQuizBackend.Repository.Implementation;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Implementation;
using PubQuizBackend.Service.Interface;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PubQuizContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:DBConnection"]));

builder.Services.AddSingleton<IAuthorizationHandler, MinimumRoleAndAudienceHandler>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPostalCodeRepository, PostalCodeRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IOrganizerRepository, OrganizerRepository>();

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizerService, OrganizerService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudiences =
            [
                builder.Configuration["Jwt:Audience:Admin"],
                builder.Configuration["Jwt:Audience:Organizer"],
                builder.Configuration["Jwt:Audience:User"]
            ],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("User", policy =>
//        policy.Requirements.Add(new MinimumRoleAndAudienceRequirement(
//            Role.ATTENDEE,
//            builder.Configuration["Jwt:Audience:User"]!)));

//    options.AddPolicy("Moderator", policy =>
//        policy.Requirements.Add(new MinimumRoleAndAudienceRequirement(
//            Role.ORGANIZER,
//            builder.Configuration["Jwt:Audience:Moderator"]!)));

//    options.AddPolicy("Admin", policy =>
//        policy.Requirements.Add(new MinimumRoleAndAudienceRequirement(
//            Role.ADMIN,
//            builder.Configuration["Jwt:Audience:Admin"]!)));
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
