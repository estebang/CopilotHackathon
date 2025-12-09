var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ADD NEW ENDPOINTS HERE
app.MapGet("/days-between", (DateTime startDate, DateTime endDate) =>
{
    var days = Math.Abs((endDate - startDate).Days);
    return Results.Ok(new { startDate, endDate, daysBetween = days });
})
.WithName("GetDaysBetween")
.WithOpenApi();

// Validate phone numbrer endpoint
// receive by querystring a parameter called phoneNumber
// validate phoneNumber with Spanish format, for example +34666777888
// if phoneNumber is valid return true
app.MapGet("/validate-phone", (string phoneNumber) =>
{
    var regex = new System.Text.RegularExpressions.Regex(@"^\+34\d{9}$");
    var isValid = regex.IsMatch(phoneNumber);
    return Results.Ok(new { phoneNumber, isValid });
})
.WithName("ValidatePhoneNumber")
.WithOpenApi();

app.Run();

// Needed to be able to access this type from the MinimalAPI.Tests project.
public partial class Program
{ }
