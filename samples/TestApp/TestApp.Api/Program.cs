using StronglyTypedIds.Json;
using TestApp.Domain;
var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
  options.SerializerOptions.AddStronglyTypedIdConverters();
});

var app = builder.Build();

app.MapGet("/{id:int}", (OrderId id) =>
{
  var orderId = OrderId.New();
  return Results.Ok(orderId);
});

app.Run();
