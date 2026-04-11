using Core.Interfaces;
using Infrastructure.Scrapers;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Playwright;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// habilitar el bloqueo de usuarios por limite de peticiones
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    //bloqueo de 3 peticiones por usuario(en entorno de pruebas)
    options.AddFixedWindowLimiter("fixed", op =>
    {
        op.PermitLimit = 3;
        op.Window = TimeSpan.FromSeconds(5);
    });

    //se define la respuesta por bloqueo de peticiones
    options.OnRejected = (async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            success = false,
            message = "Has superado el limite de peticiones. Intenta nuevamente mas tarde."
        });
    });
});

IBrowser browser = await PlaywrightService.OpenBrowserChromiun();

builder.Services.AddSingleton(browser);
builder.Services.AddScoped<IScraperService, WalmartScraper>();
builder.Services.AddScoped<IScraperService, CuracaoScraper>();
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
app.UseRateLimiter();
app.UseMiddleware<ExeptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().RequireRateLimiting("fixed");

app.Run();
