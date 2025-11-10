using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/payment/pay", async (HttpContext http) =>
{
    var body = await JsonSerializer.DeserializeAsync<PaymentRequest>(http.Request.Body);
    if (body is null || string.IsNullOrEmpty(body.OrderId))
        return Results.BadRequest(new { error = "orderId required" });

    // Simular procesamiento de pago
    await Task.Delay(200);

    return Results.Ok(new { orderId = body.OrderId, paid = true, amount = body.Amount });
});

app.Run();

public record PaymentRequest(string OrderId, double Amount);