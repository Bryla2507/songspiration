using SongSpiration.BLL;
using SongSpiration.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // <-- Kluczowy using dla Swaggera
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Kontrolery i Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SongSpiration API", Version = "v1" });

    // Definicja zabezpieczeń (Naprawia błędy CS0246, CS0103)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// 2. Baza Danych (SQLite)
// 2. Baza Danych (SQLite)
builder.Services.AddSongSpirationDal(options =>
{
    // Pobiera string z appsettings.json, który wcześniej uzupełniliśmy
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 3. Logika biznesowa (Serwisy)
builder.Services.AddSongSpirationBll();

// 4. Autentykacja i Autoryzacja
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TWOJ_BARDZO_DLUGI_I_TAJNY_KLUCZ_MIN_32_ZNAKI"))
    };
});

builder.Services.AddAuthorization();

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SongSpiration API V1");
    });
}

app.UseHttpsRedirection();

// Serwowanie plików z wwwroot/uploads
app.UseStaticFiles(); 

app.UseCors();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();