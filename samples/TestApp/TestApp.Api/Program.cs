using System.Text.Json;
using TestApp.Domain;
var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.AddStronglyTypedIdConverters();
});

var app = builder.Build();

app.MapGet("/", () =>
{
  var orderId = OrderId.New();
  var customerId = CustomerId.Empty;
  return Results.Ok(new
  {
    Guidid = orderId,
    IntId =  customerId,
    
  });
});

app.Run();
