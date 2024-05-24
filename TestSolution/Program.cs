using TestSolution.Infrastructrue.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ForsquareSearchPlacesClient>();
builder.Services.Configure<ForsquareHttpClientOptions>(
	builder.Configuration.GetSection(ForsquareHttpClientOptions.Section));

builder.Services.AddTransient<IForsquareSearchPlacesClient, ForsquareSearchPlacesClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }