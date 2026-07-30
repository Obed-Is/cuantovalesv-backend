using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Scrapers.Services;
using Scrapers.SitesWeb;
using WebApi.Services;

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

//IBrowser browser;
//try
//{
//    browser = await PlaywrightService.OpenBrowserChromiun();
//    builder.Services.AddSingleton(browser);
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Ocurrio un error al iniciar el navegador de Playwright: {ex.Message}");
//}

//builder.Services.AddScoped<IScraperService, WalmartScraper>();
//builder.Services.AddScoped<IScraperService, CuracaoScraper>();
//builder.Services.AddScoped<IScraperService, SimanScraper>();
//builder.Services.AddScoped<IScraperService, SelectosScraper>();
//builder.Services.AddScoped<ISearchService, SearchProductsService>();
builder.Services.AddScoped<ScraperService>();
builder.Services.AddScoped<WalmartScraper>();
builder.Services.AddScoped<ValidacionesRequests>();

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        //Console.WriteLine(context.ModelState.Values.SelectMany(v => v.Errors));
        var primerError = context.ModelState
            .Values
            .SelectMany(v => v.Errors)
            .FirstOrDefault()?.ErrorMessage;

        return new BadRequestObjectResult(new
        {
            success = false,
            message = primerError ?? "Error de validacion en los datos"
        });
    };
});


var app = builder.Build();

//middlewares
app.UseRateLimiter();
//app.UseMiddleware<ExeptionMiddleware>();
//app.UseExceptionHandler(errorApp =>
//{
//    errorApp.Run(async context =>
//    {
//        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

//        //context.Response.StatusCode = error is Exception
//        //    ? StatusCodes.Status400BadRequest
//        //    : StatusCodes.Status500InternalServerError;

//        //await context.Response.WriteAsJsonAsync(new
//        //{
//        //    success = false,
//        //    message = error?.Message
//        //});

//        if(error is ArgumentException)
//        {
//            context.Response.StatusCode = StatusCodes.Status400BadRequest;
//            await context.Response.WriteAsJsonAsync(new
//            {
//                success = false,
//                message = error.Message
//            });
//            return;
//        }

//        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

//        await context.Response.WriteAsJsonAsync(new
//        {
//            success = false,
//            message = "Ocurrio un error inesperado, si el problema persiste envianos un reporte"
//        });
//    });
//});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().RequireRateLimiting("fixed");

app.Run();
