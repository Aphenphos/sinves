using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using sinves.Models;
using sinves.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("BusinessDB"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwt =>
    {
        var keystr = builder.Configuration["JWT:Key"];
        var vIssuer = builder.Configuration["JWT:Issuer"];
        var vAudience = builder.Configuration["JWT:Audience"];
        byte[] key = Encoding.ASCII.GetBytes(keystr);
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = vIssuer,
            ValidateAudience = true,
            ValidAudience = vAudience,
            RequireExpirationTime = false, //in dev will need refresh token for prod (ideally)
            ValidateLifetime = true
        };
        jwt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["JWT"];
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddSingleton<BusinessService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddControllers();

var app = builder.Build();
app.UseCors(x => x
    .WithOrigins("http://127.0.0.1:3000")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    );
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
