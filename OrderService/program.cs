using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/order/process", async (HttpContext http) =>
{
    var body = await JsonSerializer.DeserializeAsync<OrderRequest>(http.Request.Body);
    if (body is null || string.IsNullOrEmpty(body.OrderId))
        return Results.BadRequest(new { error = "orderId required" });

    // Simular procesamiento
    await Task.Delay(300);

    return Results.Ok(new { orderId = body.OrderId, status = "processed" });
});

app.Run();

public record OrderRequest(string OrderId);