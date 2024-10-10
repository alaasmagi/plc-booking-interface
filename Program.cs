using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Runtime.CompilerServices;
using plc_booking_app.Backend;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
    Args = args
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "ALLOW-FROM https://outlook.office365.com");
    await next();
});

app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

BLL.StartSystemCleanTimer();
//BLL.StartRefreshTimer();

app.Run();
