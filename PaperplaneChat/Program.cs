using Azure.AI.OpenAI;
using Azure;
using PaperplaneChat.Services.Interfaces;
using PaperplaneChat.Services;

namespace PaperplaneChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var configuration = builder.Configuration;
            var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]!);
            var apiKey = configuration["AzureOpenAI:ApiKey"]!;
            builder.Services.AddSingleton(new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey)));
            
            builder.Services.AddScoped<IChatService, ChatService>();

            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}