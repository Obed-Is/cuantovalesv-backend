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
        private static readonly string[] options = new[] {
                        "--no-sandbox",
                        "--disable-setuid-sandbox",
                        "--disable-gpu",
                        "--disable-dev-shm-usage",
                        "--disable-extensions",
                        "--disable-background-networking",
                        "--disable-default-apps",
                        "--no-first-run",
                        "--disable-translate",
                        "--disable-sync",
                        "--blink-settings=imagesEnabled=false"
                    };

        public static async Task<IBrowser> OpenBrowserChromiun()
        {
            try
            {
                IPlaywright? playwright = await Playwright.CreateAsync();

                var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Args = options
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
