using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Application.Services;
using RestaurantePDV.Infrastructure.Data;
using RestaurantePDV.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Entity Framework
builder.Services.AddDbContext<RestauranteContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IComandaRepository, ComandaRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();

// Novos repositórios do sistema administrativo
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
builder.Services.AddScoped<IEscalaTrabalhoRepository, EscalaTrabalhoRepository>();
builder.Services.AddScoped<IClienteRestricaoRepository, ClienteRestricaoRepository>();

// Repositórios do Controle de Caixa
builder.Services.AddScoped<ICaixaRepository, CaixaRepository>();
builder.Services.AddScoped<IMovimentoCaixaRepository, MovimentoCaixaRepository>();

// Services
builder.Services.AddScoped<IComandaService, ComandaService>();
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// Novos serviços do sistema administrativo
builder.Services.AddScoped<IFuncionarioService, FuncionarioService>();
builder.Services.AddScoped<IEscalaTrabalhoService, EscalaTrabalhoService>();
builder.Services.AddScoped<IClienteRestricaoService, ClienteRestricaoService>();

// Serviços do Controle de Caixa
builder.Services.AddScoped<ICaixaService, CaixaService>();
builder.Services.AddScoped<IMovimentoCaixaService, MovimentoCaixaService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AdminOrGerente", policy => policy.RequireRole("Admin", "Gerente"));
    options.AddPolicy("AllRoles", policy => policy.RequireRole("Admin", "Gerente", "UsuarioComum"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Restaurante PDV API", Version = "v1" });
    
    // JWT Bearer configuration for Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
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

