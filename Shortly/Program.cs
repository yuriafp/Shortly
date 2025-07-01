using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shortly.API.Common;
using Shortly.API.Data;
using Shortly.API.Data.Repositories;
using Shortly.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

builder.Services.AddScoped<IUrlShortenerRepository, UrlShortenerRepository>();
builder.Services.AddScoped<IReadUrlRepository, UrlShortenerRepository>();
builder.Services.AddScoped<IWriteUrlRepository, UrlShortenerRepository>();

var app = builder.Build();

app.UseMiddleware<Shortly.API.Middleware.GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


#region Redirect to original url

app.MapGet("/{shortCode}", async (string shortCode, IUrlShortenerService service) =>
{
    var result = await service.ProcessRedirectAndCountAsync(shortCode);

    if (result.IsFailure)
    {
        return result.Error switch
        {
            DomainErrors.NotFound => Results.NotFound(),
            DomainErrors.Expired => Results.StatusCode(StatusCodes.Status410Gone),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    return Results.Redirect(result.Value, permanent: false);
});

#endregion 

app.Run();
