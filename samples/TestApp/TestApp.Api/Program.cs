using StronglyTypedIds.Json;
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
  var customerId = CustomerId.New();
  return Results.Ok(orderId);
});

app.Run();
