using Core.Interfaces;
using Infrastructure.Scrapers;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IBrowser browser = await PlaywrightService.OpenBrowserChromiun();

builder.Services.AddSingleton(browser);
builder.Services.AddScoped<IScraperService, WalmartScraper>();
builder.Services.AddScoped<IScraperService, SimanScraper>();
builder.Services.AddScoped<IScraperService, SelectosScraper>();
builder.Services.AddScoped<ISearchService, SearchProductsService>();

builder.Services.AddControllers();
//configuracion del error de validacion automatica de ASP.net
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context => {
        return new BadRequestObjectResult(new
        {
            success = false,
            message = "Error de validación",
            //errors = context.ModelState
        });
    };
});

var app = builder.Build();

//middlewares
app.UseMiddleware<ExeptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
