using Imagize.Core.Extensions;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Host.UseSerilog(
    (ctx, lc) => lc
    .WriteTo.Console());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Imagizer.Net API",
        Description = "An ASP.NET Core Web Api for image manipulation",
    });
});

builder.Services.AddHttpClient();

string allowedOrigins = Environment.GetEnvironmentVariable("IMAGIZE_ALLOWED_ORIGINS") ?? "";
string allowedFileTypes = Environment.GetEnvironmentVariable("IMAGIZE_ALLOWED_FILETYPES") ?? "";

builder.Services.AddImagize(config =>
{
    config.AllowedOrigins = allowedOrigins;
    config.AllowedFileTypes = allowedFileTypes;
});

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("X-Request-Header");
    logging.ResponseHeaders.Add("X-Response-Header");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});



builder.Configuration.AddEnvironmentVariables(prefix: "IMAGIZE_");

Console.WriteLine($"allowedOrigins:{allowedOrigins}");

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



#if DEBUG
//foreach (KeyValuePair<string, string> c in builder.Configuration.AsEnumerable())
//{
//    Console.WriteLine(c.Key + " = " + c.Value);
//}
#endif

app.Run();
