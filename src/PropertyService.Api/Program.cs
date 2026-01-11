using PropertyService.Api.Configuration;
using PropertyService.Api.Services;
using PropertyService.Application.Interfaces;
using PropertyService.Application.Services;
using PropertyService.Infrastructure.Data;
using PropertyService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurações
builder.Services.ConfigureSwagger();
builder.Services.AddControllers();
builder.Services.ConfigureJwt(builder.Configuration);

// Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.ConfigureWarnings(warnings => 
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Repositórios
builder.Services.AddScoped<IPropriedadeRepository, PropriedadeRepository>();
builder.Services.AddScoped<ITalhaoRepository, TalhaoRepository>();

// Serviços
builder.Services.AddScoped<PropriedadeService>();
builder.Services.AddScoped<TalhaoService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
