using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(o => {
    o.AddPolicy("AllowAll", a => a.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});

var conn = new SqliteConnection($"Data Source={builder.Configuration["Db:Location"]}carlist.db");
builder.Services.AddDbContext<CarListDbContext>(o => o.UseSqlite(conn));


builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CarListDbContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JwtSettings:Key is not configured")))
    };
});

builder.Services.AddAuthorization(options => {
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();
});

builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));



var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<CarListDbContext>();
    var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();

    context?.Database.Migrate();

    // Recreate users with properly hashed passwords
    var adminUser = await userManager!.FindByEmailAsync("admin@localhost.com");
    if (adminUser != null)
    {
        // Remove existing user to recreate with correct password
        await userManager.DeleteAsync(adminUser);
    }

    var regularUser = await userManager.FindByEmailAsync("user@localhost.com");
    if (regularUser != null)
    {
        await userManager.DeleteAsync(regularUser);
    }

    // Create admin user with correct password hash
    adminUser = new IdentityUser
    {
        Id = "408aa945-3d84-4421-8342-7269ec64d949",
        Email = "admin@localhost.com",
        UserName = "admin@localhost.com",
        EmailConfirmed = true,
        NormalizedEmail = "ADMIN@LOCALHOST.COM",
        NormalizedUserName = "ADMIN@LOCALHOST.COM"
    };
    await userManager.CreateAsync(adminUser, "P@ssword1");
    await userManager.AddToRoleAsync(adminUser, "Administrator");

    // Create regular user with correct password hash
    regularUser = new IdentityUser
    {
        Id = "3f4631bd-f907-4409-b416-ba356312e659",
        Email = "user@localhost.com",
        UserName = "user@localhost.com",
        EmailConfirmed = true,
        NormalizedEmail = "USER@LOCALHOST.COM",
        NormalizedUserName = "USER@LOCALHOST.COM"
    };
    await userManager.CreateAsync(regularUser, "P@ssword1");
    await userManager.AddToRoleAsync(regularUser, "User");
}

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

app.MapGet("/cars", async (CarListDbContext db) =>
{
    Log.Information("Fetching all cars");
    return await db.Cars.ToListAsync();
});

app.MapGet("/cars/{id}", async (int id, CarListDbContext db) =>
    await db.Cars.FindAsync(id) is Car car ? Results.Ok(car) : Results.NotFound()
);


app.MapPut("/cars/{id}", async (int id, Car car, CarListDbContext db) => { 
    var record = await db.Cars.FindAsync(id);
    if (record is null) return Results.NotFound();

    record.Make = car.Make;
    record.Model= car.Model;
    record.Vin = car.Vin;

    await db.SaveChangesAsync();

    return Results.NoContent();

});

app.MapDelete("/cars/{id}", async (int id, CarListDbContext db) => {
    var record = await db.Cars.FindAsync(id);
    if (record is null) return Results.NotFound();
    db.Remove(record);
    await db.SaveChangesAsync();

    return Results.NoContent();

});

app.MapPost("/cars", async (Car car, CarListDbContext db) => {
    await db.AddAsync(car);
    await db.SaveChangesAsync();

    return Results.Created($"/cars/{car.Id}", car);

});

app.MapPost("/login", async (LoginDto loginDto, UserManager<IdentityUser> _userManager) => {
    var user = await _userManager.FindByNameAsync(loginDto.Username);

    if(user is null)
    {
        return Results.Unauthorized();
    }

    var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);

    if (!isValidPassword)
    {
        return Results.Unauthorized();
    }

    // Generate an access token
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JwtSettings:Key is not configured")));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var roles = await _userManager.GetRolesAsync(user);
    var claims = await _userManager.GetClaimsAsync(user);
    var tokenClaims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        new Claim("email_confirmed", user.EmailConfirmed.ToString())
    }.Union(claims)
    .Union(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var securityToken = new JwtSecurityToken(
        issuer: builder.Configuration["JwtSettings:Issuer"],
        audience: builder.Configuration["JwtSettings:Audience"],
        claims: tokenClaims,
        expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(builder.Configuration["JwtSettings:DurationInMintues"])),
        signingCredentials: credentials
    );

    var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);


    var response = new AuthResponseDto
    {
        UserId = user.Id,
        Username = user.UserName ?? string.Empty,
        Token = accessToken
    };

    return Results.Ok(response);
}).AllowAnonymous();


app.Run();


internal class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

internal class AuthResponseDto
{
    public required string UserId { get; set; }
    public required string Username { get; set; }
    public required string Token { get; set; }
}
