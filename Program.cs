global using Backend.Data.Models;
global using Backend.Data.DTOs.Response;
global using Backend.Data.DTOs.Request;
global using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager _configuration = builder.Configuration;
builder.Services.AddSingleton<IConfiguration>(_configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Dev stuff
builder.Services.AddSwaggerGen(
options =>
{
options.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
{
  Description = "JWT Authorization header using the Bearer scheme",
  Type = SecuritySchemeType.Http,
  Scheme = "bearer"
});
options.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme,Id = "Bearer" } },Array.Empty<string>() } });
options.SwaggerDoc("v1",new OpenApiInfo { Title = "CAS API",Version = "v1" });
}
);
// Dev stuff end


builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddDbContext<AppDBContext>();

// Add JWT Authentication
builder.Services.AddAuthentication(o =>
{
  o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
  string? _TokenSecret = _configuration.GetValue<string>("JWT:TokenSecret") ?? throw new NullReferenceException("missing token secret!");
  var key = Encoding.UTF8.GetBytes(_TokenSecret);
  config.SaveToken = true;
  config.TokenValidationParameters = new TokenValidationParameters
  {
    ValidIssuer = _configuration.GetValue<string>("JWT:Issuer"),
    ValidAudience = _configuration.GetValue<string>("JWT:Audience"),
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateIssuerSigningKey = true,
  };
});

builder.Services.AddTransient<IInterface,SQLFunctions>();


builder.Services.AddHealthChecks();

var app = builder.Build();

// add health checks endpoint
app.MapHealthChecks("/health");
app.UseCors(x => x
  .AllowAnyMethod()
  .AllowAnyHeader()
  .AllowAnyOrigin());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
