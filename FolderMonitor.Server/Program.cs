using FolderMonitor.Server.Services;
using FolderMonitor.Server.Services.IService;
namespace FolderMonitor.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //make this dynamic
            string folderPath = @"C:\Users\user\Desktop\ProgressProject\FolderMonitor\FolderMonitor.Server\Monitored_Folder";
            var builder = WebApplication.CreateBuilder(args);
            
            // Register HttpClient for AuthService with the correct configuration
            builder.Services.AddHttpClient<AuthService>(client =>
            {
                // Optionally configure the HttpClient (like setting base address, headers, etc.)
                client.BaseAddress = new Uri("https://testserver.moveitcloud.com");  // Example base address
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IFileService, FileService>();
            //builder.Services.AddScoped<IFileService>(sp =>
            //{
            //    var authService = sp.GetRequiredService<IAuthService>();
            //    var logger = sp.GetRequiredService<ILogger<FileService>>();
            //    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();

            //    // Pass the folder path dynamically or from a config
            //   // var folderPath = @"C:\Users\user\Desktop\ProgressProject\FolderMonitor\FolderMonitor.Server\Monitored_Folder";

            //    return new FileService(authService, logger, httpClientFactory);
            //});
            builder.Services.AddSingleton<TokenService>();

            //// Add services for file monitoring
            //builder.Services.AddSingleton<FileService>(sp =>
            //{
            //    // Provide the folder path to monitor
            //    return new FileService(folderPath);
            //});

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Start the file monitoring service
           // var fileService = app.Services.GetRequiredService<FileService>();
           // fileService.StartMonitoring(); // Start monitoring the folder


            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
