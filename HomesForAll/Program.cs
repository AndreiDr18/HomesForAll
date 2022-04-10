using HomesForAll.DAL;
using HomesForAll.DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using HomesForAll.Services;
using HomesForAll.DAL.UserRoles;
using System.Text;
using Serilog;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:7165"));

ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddServices();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Homes For All",
        Description = "An API used for housing centralisation and tenancy pairing for refugees and people in need of accomodations due to lack of financial status.",
    });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "HomesForAllDocumentation.xml");
    c.IncludeXmlComments(filePath);
});

//Making the database connection + adding a DBContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b=> b.MigrationsAssembly("HomesForAll.DAL"))
);

builder.Services.AddIdentity<User, IdentityRole<Guid>>(opt =>
{
    opt.User.RequireUniqueEmail = true;
    opt.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidAudience = Environment.GetEnvironmentVariable("JWT:VALIDAUDIENCE"),
        ValidIssuer = Environment.GetEnvironmentVariable("JWT:VALIDISSUER"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT:SECRET")))
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await RoleSeeder.InitRoles(app.Services.CreateScope());


app.Run();

