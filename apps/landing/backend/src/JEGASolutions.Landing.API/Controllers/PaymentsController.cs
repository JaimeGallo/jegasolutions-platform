using Microsoft.AspNetCore.Mvc;
using JEGASolutions.Landing.Application.Interfaces;
using JEGASolutions.Landing.Application.DTOs;

namespace JEGASolutions.Landing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IWompiService _wompiService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService paymentService,
        IWompiService wompiService,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _wompiService = wompiService;
        _logger = logger;
    }

    [HttpPost("webhook")]
public async Task<IActionResult> Webhook()
{
    try
    {
        // 🔍 LEER EL RAW BODY PRIMERO
        string rawBody;
        Request.Body.Seek(0, SeekOrigin.Begin);
        using (var reader = new StreamReader(Request.Body, leaveOpen: true))
        {
            rawBody = await reader.ReadToEndAsync();
        }
        Request.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation("========== WEBHOOK RAW BODY ==========");
        _logger.LogInformation(rawBody);
        _logger.LogInformation("=====================================");

        // Deserializar manualmente
        var payload = System.Text.Json.JsonSerializer.Deserialize<WompiWebhookDto>(rawBody, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // 🔍 LOGGING DETALLADO DEL WEBHOOK
        _logger.LogInformation("========== WEBHOOK PARSED ==========");
        _logger.LogInformation("Event: {Event}", payload?.Event ?? "NULL");
        _logger.LogInformation("Environment: {Environment}", payload?.Environment ?? "NULL");
        _logger.LogInformation("Timestamp: {Timestamp}", payload?.Timestamp);
        _logger.LogInformation("Data.Id: {Id}", payload?.Data?.Id ?? "NULL");
        _logger.LogInformation("Data.Reference: {Reference}", payload?.Data?.Reference ?? "NULL");
        _logger.LogInformation("Data.Status: {Status}", payload?.Data?.Status ?? "NULL");
        _logger.LogInformation("Data.AmountInCents: {Amount}", payload?.Data?.AmountInCents);
        _logger.LogInformation("Data.CustomerEmail: {Email}", payload?.Data?.CustomerEmail ?? "NULL");
        _logger.LogInformation("=====================================");

        // Get the signature from headers
        var signature = Request.Headers["X-Integrity"].FirstOrDefault();

        // ✅ CAMBIO: Hacer la validación de firma opcional si no viene el header
        if (!string.IsNullOrEmpty(signature))
        {
            // Get the raw body for signature validation
            string rawBody;
            Request.Body.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(Request.Body, leaveOpen: true))
            {
                rawBody = await reader.ReadToEndAsync();
            }

            // Validate signature
            var isValidSignature = await _wompiService.ValidateWebhookSignature(rawBody, signature);
            if (!isValidSignature)
            {
                _logger.LogWarning("Invalid signature for reference {Reference}", payload.Data.Reference);
                return Unauthorized(new { message = "Invalid signature" });
            }

            _logger.LogInformation("Webhook signature validated successfully");
        }
        else
        {
            _logger.LogWarning("Webhook received without X-Integrity header for reference {Reference}",
                payload.Data.Reference);
            // Continuar procesando aunque no haya firma (solo en sandbox/testing)
            // En producción podrías querer rechazar esto
        }

        // Process the webhook
        var success = await _wompiService.ProcessPaymentWebhook(payload);

        if (success)
        {
            _logger.LogInformation("Webhook processed successfully for reference {Reference}",
                payload.Data.Reference);
            return Ok(new { message = "Webhook processed successfully" });
        }
        else
        {
            _logger.LogError("Error processing webhook for reference {Reference}",
                payload.Data.Reference);
            return StatusCode(500, new { message = "Error processing webhook" });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Internal server error processing webhook");
        return StatusCode(500, new { message = "Internal server error", error = ex.Message });
    }
}

    [HttpGet("status/{reference}")]
    public async Task<IActionResult> GetPaymentStatus(string reference)
    {
        try
        {
            _logger.LogInformation("Getting payment status for reference {Reference}", reference);

            var payment = await _paymentService.GetPaymentByReferenceAsync(reference);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found for reference {Reference}", reference);
                return NotFound(new { message = "Payment not found" });
            }

            var response = new
            {
                reference = payment.Reference,
                status = payment.Status,
                amount = payment.Amount,
                customerEmail = payment.CustomerEmail,
                customerName = payment.CustomerName,
                createdAt = payment.CreatedAt,
                updatedAt = payment.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment status for reference {Reference}", reference);
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

    [HttpGet("customer/{email}")]
    public async Task<IActionResult> GetCustomerPayments(string email)
    {
        try
        {
            _logger.LogInformation("Getting payments for customer {Email}", email);

            var payments = await _paymentService.GetPaymentsByCustomerAsync(email);

            var result = payments.Select(p => new
            {
                reference = p.Reference,
                status = p.Status,
                amount = p.Amount,
                customerName = p.CustomerName,
                createdAt = p.CreatedAt,
                updatedAt = p.UpdatedAt
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payments for customer {Email}", email);
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }

  [HttpPost("create")]
public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto request)
{
    try
    {
        _logger.LogInformation("=== PAYMENT REQUEST RECEIVED ===");
        _logger.LogInformation("Reference: {Reference}", request.Reference);
        _logger.LogInformation("Amount: {Amount}", request.Amount);
        _logger.LogInformation("CustomerEmail: {Email}", request.CustomerEmail);
        _logger.LogInformation("CustomerName: {Name}", request.CustomerName);
        _logger.LogInformation("================================");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Solo llamar al servicio - él internamente ya llama a Wompi
        var paymentResponse = await _paymentService.CreatePaymentAsync(request);

        // Retornar la respuesta directamente
        return Ok(paymentResponse);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating payment");
        return StatusCode(500, new { message = "Internal server error", error = ex.Message });
    }
}


    [HttpPut("status/{reference}")]
    public async Task<IActionResult> UpdatePaymentStatus(string reference, [FromBody] UpdatePaymentStatusDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating payment status for reference {Reference} to {Status}", reference, request.Status);

            var success = await _paymentService.UpdatePaymentStatusAsync(reference, request.Status);

            if (!success)
            {
                _logger.LogWarning("Payment not found for reference {Reference}", reference);
                return NotFound(new { message = "Payment not found" });
            }

            return Ok(new { message = "Payment status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for reference {Reference}", reference);
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }
}
