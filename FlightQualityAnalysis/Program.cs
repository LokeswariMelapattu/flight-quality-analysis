using FlightQualityAnalysis;
using FlightQualityAnalysis.Domain.Interfaces;
using FlightQualityAnalysis.Handlers;
using FlightQualityAnalysis.Infrastructure;
using FlightQualityAnalysis.Services; 

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// Add services to the container.
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Read AppSettings fron appsetings.json configuration
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .Build();  

//Register Services
builder.Services.AddSingleton<IFlightAnalysisRepository, FlightAnalysisRepository>();
builder.Services.AddSingleton<IFlightAnalysisService, FlightAnalysisService>();

// Register Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Quality Analysis v1"));
}
  
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
