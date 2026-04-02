using Core.Exceptions;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PlaywrightService
    {
        public static async Task<IBrowser> OpenBrowserChromiun()
        {
            try
            {
                IPlaywright? playwright = await Playwright.CreateAsync();

                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
                });

                return browser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al iniciar Playwright: {ex}");
                throw new AppExceptionStatusCode(500, "Error interno");
            }
        }
    }
}
