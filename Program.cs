using System.Text;
using blog.Context;
using blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<BlogServices>();

var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

var secretKey = builder.Configuration["JWT:Key"];
var singingCredentials = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

// We're adding auth to our builder to check the JWToken from out services

builder.Services.AddAuthentication(options => {
    //this line of code will set the authentification behavior of our JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //Sets default behavior when authentification fails
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    //configuring JWT beaer options (checking the params)
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidateIssuer = true, //check if the token issuer is valid
        ValidateAudience = true, //checks if the token's audience is valid
        ValidateLifetime = true, //ensures that token has not expired
        ValidateIssuerSigningKey = true, // Chekcing the token signature is valid

        ValidIssuer = "http://localhost:5000",
        ValidAudience = "http://localhost:5000",
        IssuerSigningKey = singingCredentials
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
