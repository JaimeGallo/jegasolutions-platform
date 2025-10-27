using JEGASolutions.ExtraHours.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JEGASolutions.ExtraHours.API.Controller
{
    [Route("api/managers")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerRepository _managerRepository;
        private readonly IUserRepository _userRepository;

        public ManagerController(IManagerRepository managerRepository, IUserRepository userRepository)
        {
            _managerRepository = managerRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Obtiene la lista de todos los managers con su información de usuario
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "superusuario,manager")]
        public async Task<IActionResult> GetAllManagers()
        {
            try
            {
                var managers = await _managerRepository.GetAllAsync();
                
                var result = new List<object>();
                
                foreach (var manager in managers)
                {
                    // Obtener información del usuario asociado
                    var user = await _userRepository.GetUserByIdAsync(manager.manager_id);
                    
                    if (user != null && !user.DeletedAt.HasValue) // Solo managers activos
                    {
                        result.Add(new
                        {
                            id = manager.manager_id,
                            name = user.name,
                            email = user.email,
                            position = user.role == "manager" ? (manager.Department ?? "Sin departamento") : user.role,
                            department = manager.Department,
                            role = user.role
                        });
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener la lista de managers", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un manager específico por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "superusuario,manager")]
        public async Task<IActionResult> GetManagerById(long id)
        {
            try
            {
                var manager = await _managerRepository.GetByIdAsync(id);
                
                if (manager == null)
                    return NotFound(new { error = "Manager no encontrado" });

                var user = await _userRepository.GetUserByIdAsync(manager.manager_id);
                
                if (user == null)
                    return NotFound(new { error = "Usuario asociado no encontrado" });

                return Ok(new
                {
                    id = manager.manager_id,
                    name = user.name,
                    email = user.email,
                    department = manager.Department,
                    role = user.role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al obtener el manager", details = ex.Message });
            }
        }
    }
}
