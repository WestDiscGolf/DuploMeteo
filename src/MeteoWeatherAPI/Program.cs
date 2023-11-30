using FluentValidation;
using FluentValidation.AspNetCore;
using MeteoWeatherAPI.CustomActionFilter;
using MeteoWeatherAPI.Data;
using MeteoWeatherAPI.Services;
using MeteoWeatherAPI.Validators;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers(mvcOptions => { mvcOptions.Filters.Add<MatchesLatLongFilter>(); });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(config.GetConnectionString("MongoCtx")));
builder.Services.AddScoped(s => new WeatherDbContext(s.GetRequiredService<IMongoClient>()));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IWeatherDataService, WeatherDataService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<IWeatherService, WeatherCacheService>(sp => new(sp.GetRequiredService<WeatherService>(), sp.GetRequiredService<IMemoryCache>()));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LatLongValidator>();


var app = builder.Build();

//Since this is just a backend, we may as well at least have a simple swagger ui display
//even in non-development mode.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
