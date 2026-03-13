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
  var cutomerId = CustomerId.Empty;
  return Results.Ok(new {id = orderId, cutomerId =  cutomerId});
});

app.Run();
