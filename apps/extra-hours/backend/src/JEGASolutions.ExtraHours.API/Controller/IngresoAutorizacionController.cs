using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using JEGASolutions.ExtraHours.Core.Dto;

using JEGASolutions.ExtraHours.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace JEGASolutions.ExtraHours.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngresoAutorizacionController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public IngresoAutorizacionController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // POST: api/IngresoAutorizacion
        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> AutorizarIngreso([FromBody] IngresoAutorizacionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Construir el cuerpo del correo en HTML
            var subject = $"Autorización de ingreso en día de descanso: {dto.EmployeeName} ({dto.Date})";
            var body = $@"
                <h3>Autorización de Ingreso</h3>
                <p><b>Empleado:</b> {dto.EmployeeName}</p>
                <p><b>Fecha de ingreso:</b> {dto.Date}</p>
                <p><b>Hora estimada de ingreso:</b> {dto.EstimatedEntryTime}</p>
                <p><b>Hora estimada de salida:</b> {dto.EstimatedExitTime}</p>
                <p><b>Motivo/Labor:</b> {dto.TaskDescription}</p>
                <p><b>Autorizado por:</b> {dto.ManagerName}</p>
            ";
            // Destinatarios principales y copia
            var to = "jaialgallo@gmail.com;jaialgallo@gmail.com"; // Ajustar según necesidad
            var cc = "jaialgallo@gmail.com"; // Ajustar según necesidad

            // Enviar a todos los destinatarios principales
            foreach (var email in to.Split(';'))
            {
                await _emailService.SendEmailAsync(email.Trim(), subject, body);
            }
            // Enviar copia
            await _emailService.SendEmailAsync(cc, subject, body);

            return Ok(new { message = "Correo de autorización enviado correctamente." });
        }
    }
}



