using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PubQuizBackend.Model;
using PubQuizBackend.Other;
using PubQuizBackend.Repository.Implementation;
using PubQuizBackend.Repository.Interface;
using PubQuizBackend.Service.Implementation;
using PubQuizBackend.Service.Interface;
using PubQuizBackend.Util;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(

//options =>
//{
//    options.Conventions.Add(new RoleAndAudienceConvention());
//}
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PubQuizContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:DBConnection"]));

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IPostalCodeRepository, PostalCodeRepository>();
builder.Services.AddScoped<IPrizeRepository, PrizeRepository>();
builder.Services.AddScoped<IQuizAnswerRepository, QuizAnswerRepository>();
builder.Services.AddScoped<IQuizCategoryRepository, QuizCategoryRepository>();
builder.Services.AddScoped<IQuizEditionRepository, QuizEditionRepository>();
builder.Services.AddScoped<IQuizLeagueRepository, QuizLeagueRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IUpcomingQuizQuestionRepository, UpcomingQuizQuestionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEloCalculatorRepository, EloCalculatorRepository>();
builder.Services.AddScoped<IRatingHistoryRepository, RatingHistoryRepository>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IQuizAnswerService, QuizAnswerService>();
builder.Services.AddScoped<IQuizCategoryService, QuizCategoryService>();
builder.Services.AddScoped<IQuizEditionService, QuizEditionService>();
builder.Services.AddScoped<IQuizEditionApplicationService, QuizEditionApplicationService>();
builder.Services.AddScoped<IQuizLeagueService, QuizLeagueService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IUpcomingQuizQuestionService, UpcomingQuizQuestionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEloCalculatorService, EloCalculatorService>();
builder.Services.AddScoped<IRatingHistoryService, RatingHistoryService>();

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
                builder.Configuration["Jwt:Audience:Attendee"]
            ],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

var allowedOrigins = new[] { "https://localhost:7147", "https://localhost:5001", "https://example.com" };

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "AllowFrontend",
            builder =>
            {
                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        );
    }
);


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var quizCategoryRepo = scope.ServiceProvider.GetRequiredService<IQuizCategoryRepository>();
    var categories = await quizCategoryRepo.GetAll();

    QuizCategoryProvider.Initialize(categories);

    var eloCalculatorService = scope.ServiceProvider.GetRequiredService<IEloCalculatorService>();

    await eloCalculatorService.CalculateKappa();
}

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
