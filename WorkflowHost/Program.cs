using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapPost("/api/workflow/start", async (HttpContext http) =>
{
    var request = await JsonSerializer.DeserializeAsync<StartWorkflowRequest>(http.Request.Body);
    if (request is null || string.IsNullOrEmpty(request.OrderId))
        return Results.BadRequest(new { error = "orderId required" });

    var client = new HttpClient();
    //change client to dapr client


    // Invocar OrderService vía Dapr
    var orderPayload = JsonSerializer.Serialize(new OrderRequest(request.OrderId));
    Console.WriteLine($"Order Payload: {orderPayload}");
    var orderResp = await client.PostAsync($"http://localhost:3500/v1.0/invoke/orderservice/method/api/order/process",
        new StringContent(orderPayload, Encoding.UTF8, "application/json"));
    if (!orderResp.IsSuccessStatusCode)
        return Results.StatusCode(500);

    var orderResult = await orderResp.Content.ReadAsStringAsync();

    // Invocar PaymentService vía Dapr (simulando pago)
    var paymentPayload = JsonSerializer.Serialize(new PaymentRequest( request.OrderId,  42.0 ));
    Console.WriteLine($"Payment Payload: {paymentPayload}");
    var payResp = await client.PostAsync($"http://localhost:3500/v1.0/invoke/paymentservice/method/api/payment/pay",
        new StringContent(paymentPayload, Encoding.UTF8, "application/json"));
    if (!payResp.IsSuccessStatusCode)
        return Results.StatusCode(500);

    var paymentResult = await payResp.Content.ReadAsStringAsync();

    return Results.Ok(new
    {
        order = JsonDocument.Parse(orderResult).RootElement,
        payment = JsonDocument.Parse(paymentResult).RootElement
    });
});

app.Run();

public record StartWorkflowRequest(string OrderId);
public record OrderRequest(string OrderId);
public record PaymentRequest(string OrderId, double Amount);

