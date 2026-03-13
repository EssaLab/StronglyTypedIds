using EssaLab.StronglyTypedIds.Convertors.Json;
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
  return Results.Ok(new {id = orderId});
});

app.Run();
