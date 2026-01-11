using ManagementService.Application.Interfaces;
using ManagementService.Application.Services;
using ManagementService.Infrastructure.Data;
using ManagementService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Configurar Entity Framework com PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Registrar repositórios
builder.Services.AddScoped<IPropriedadeRepository, PropriedadeRepository>();
builder.Services.AddScoped<ITalhaoRepository, TalhaoRepository>();

// Registrar serviços
builder.Services.AddScoped<PropriedadeService>();
builder.Services.AddScoped<TalhaoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

// Aplicar migrations automaticamente (apenas em desenvolvimento)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
