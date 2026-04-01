using Core.Interfaces;
using Infrastructure.Scrapers;
using Infrastructure.Services;
using Microsoft.Playwright;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IBrowser browser = await PlaywrightService.OpenBrowserChromiun();

builder.Services.AddSingleton(browser);
builder.Services.AddScoped<IScraperService, WalmartScraper>();
builder.Services.AddScoped<ISearchService, SearchProductsService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
