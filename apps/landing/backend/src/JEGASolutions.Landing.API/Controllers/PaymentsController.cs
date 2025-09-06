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
    public async Task<IActionResult> Webhook([FromBody] WompiWebhookDto payload)
    {
        try
        {
            _logger.LogInformation("Received webhook for reference {Reference}", payload.Data.Reference);

            // Get the signature from headers
            var signature = Request.Headers["X-Integrity"].FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Missing X-Integrity header for reference {Reference}", payload.Data.Reference);
                return BadRequest(new { message = "Missing X-Integrity header" });
            }

            // Get the raw body for signature validation
            Request.Body.Position = 0;
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            // Validate signature
            var isValidSignature = await _wompiService.ValidateWebhookSignature(rawBody, signature);
            if (!isValidSignature)
            {
                _logger.LogWarning("Invalid signature for reference {Reference}", payload.Data.Reference);
                return Unauthorized(new { message = "Invalid signature" });
            }

            // Process the webhook
            var success = await _wompiService.ProcessPaymentWebhook(payload);

            if (success)
            {
                _logger.LogInformation("Webhook processed successfully for reference {Reference}", payload.Data.Reference);
                return Ok(new { message = "Webhook processed successfully" });
            }
            else
            {
                _logger.LogError("Error processing webhook for reference {Reference}", payload.Data.Reference);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating payment for reference {Reference}", request.Reference);

            var payment = await _paymentService.CreatePaymentAsync(request);

            return CreatedAtAction(
                nameof(GetPaymentStatus),
                new { reference = payment.Reference },
                payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment for reference {Reference}", request.Reference);
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