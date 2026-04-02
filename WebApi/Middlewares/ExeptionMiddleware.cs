using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares
{
    public class ExeptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExeptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(AppExceptionStatusCode appEx)
            {
                Console.WriteLine($"Error en el proyecto: {appEx.Source}, Mensage: {appEx.Message}");
                context.Response.StatusCode = appEx.StatusCode;
                context.Response.ContentType = "application/json";

                var res = new
                {
                    success = false,
                    //error = "",
                    message = appEx.Message,
                };

                await context.Response.WriteAsJsonAsync(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en middleware global: ", ex.Message);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var res = new
                {
                    success = false,
                    //error = "Error interno",
                    message = "Si el error persiste comunicate con soporte"
                };

                await context.Response.WriteAsJsonAsync(res);
            }
        }
    }
}
