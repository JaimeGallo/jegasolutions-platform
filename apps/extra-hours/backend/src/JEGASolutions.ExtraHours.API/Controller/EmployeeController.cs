using JEGASolutions.ExtraHours.Core.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JEGASolutions.ExtraHours.Core.Dto;
using JEGASolutions.ExtraHours.Core.Interfaces;
using System.Threading.Tasks;
using System.Text;
using ClosedXML.Excel;



namespace JEGASolutions.ExtraHours.API.Controller
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUserRepository _userRepository;
        private readonly IManagerRepository _managerRepository;
        private readonly IUserService _userService;

        public EmployeeController(IEmployeeService employeeService, IUserRepository usersRepo, IManagerRepository managerRepository, IUserService userService)
        {
            _employeeService = employeeService;
            _userRepository = usersRepo;
            _managerRepository = managerRepository;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(long id)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(id);

                return Ok(new
                {
                    id = employee.id,
                    name = employee.name,
                    position = employee.position,
                    salary = employee.salary,
                    role = await GetUserRoleById(employee.id),
                    manager = new
                    {
                        id = employee.manager?.manager_id,
                        name = employee.manager?.User?.name ?? "Sin asignar",
                        department = employee.manager?.Department
                    }
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = "Empleado no encontrado" });
            }
        }


        //[Authorize(Roles = "manager, empleado, superusuario")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("manager")]
        [Authorize(Roles = "manager,superusuario")]
        public async Task<IActionResult> GetEmployeesByManager()
        {
            var managerIdStr = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(managerIdStr))
                return Unauthorized();
            long managerId = long.Parse(managerIdStr);
            var empleados = await _employeeService.GetEmployeesByManagerIdAsync(managerId);
            return Ok(empleados);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeWithUserDTO dto)
        {
            // Los managers NO necesitan manager_id, los empleados SÍ
            if (dto.Role?.ToLower() != "manager" && dto.ManagerId == null)
                return BadRequest(new { error = "Manager ID es requerido para empleados" });

            try
            {
                // Verificar si el manager existe (solo si se proporcionó manager_id)
                if (dto.ManagerId.HasValue)
                {
                    var manager = await _managerRepository.GetByIdAsync(dto.ManagerId.Value);
                    if (manager == null)
                        return BadRequest(new { error = "Manager no encontrado con el ID proporcionado" });
                }

                // Verificar si ya existe un usuario con el mismo id
                if (await _userService.UserExistsAsync((int)dto.Id))
                    return BadRequest(new { error = "Ya existe un usuario con ese ID" });

                // Generar contraseña cifrada
                string plainPassword = dto.Password ?? "pasword123";
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                // Crear usuario correspondiente
                var user = new User
                {
                    id = (int)dto.Id,
                    email = dto.Email ?? (dto.Name?.ToLower().Replace(" ", ".") + "@empresa.com") ?? "user@empresa.com",
                    name = dto.Name ?? string.Empty,
                    passwordHash = hashedPassword,
                    role = dto.Role ?? "empleado",
                    username = dto.Username ?? dto.Name?.ToLower().Replace(" ", ".") ?? "user"
                };
                await _userRepository.SaveAsync(user);

                // Si el rol es "manager", necesitamos crear también un registro en la tabla manager
                if (dto.Role?.ToLower() == "manager")
                {
                    var newManager = new Manager
                    {
                        manager_id = dto.Id,
                        Department = dto.Position ?? "General" // Usar posición como departamento por defecto
                    };
                    await _managerRepository.AddAsync(newManager);
                }

                // Crear registro de empleado
                var employee = new Employee
                {
                    id = dto.Id,
                    position = dto.Position ?? string.Empty,
                    manager_id = dto.ManagerId, // Puede ser null para managers
                };
                await _employeeService.AddEmployeeAsync(employee);


                return Created("", new
                {
                    message = "Empleado y usuario agregados exitosamente",
                    role = dto.Role
                });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"Error completo: {ex}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Error interno: {ex.InnerException.Message}");
                    return BadRequest(new
                    {
                        error = $"Error al agregar empleado: {ex.Message}",
                        innerError = ex.InnerException.Message
                    });
                }
                return BadRequest(new { error = $"Error al agregar empleado: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(long id, [FromBody] UpdateEmployeeDTO dto)
        {
            try
            {
                var existingEmployee = await _employeeService.GetByIdAsync(id);

                // Verificar si el usuario existe
                var user = await _userService.GetUserByIdAsync(id);
                string currentRole = user?.role ?? "empleado";

                // Si no se proporciona ManagerId, usamos el actual del empleado
                if (dto.ManagerId == null)
                {
                    dto.ManagerId = existingEmployee.manager_id;
                }
                else
                {
                    // Verificar si el nuevo manager existe
                    var newManager = await _managerRepository.GetByIdAsync(dto.ManagerId.Value);
                    if (newManager == null)
                        return BadRequest(new { error = "Manager no encontrado con el ID proporcionado" });

                    // Actualizar explícitamente el manager_id
                    existingEmployee.manager_id = dto.ManagerId.Value;
                }

                // Actualizar el empleado
                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, dto);

                // Si el rol cambió, actualizar el usuario
                if (dto.Role?.ToLower() == "manager")
                {
                    var existingManager = await _managerRepository.GetByIdAsync(id);
                    if (existingManager == null)
                    {
                        var newManager = new Manager
                        {
                            manager_id = id,
                            Department = dto.Position ?? "General"
                        };
                        await _managerRepository.AddAsync(newManager);
                    }
                    else if (existingManager.Department != dto.Position)
                    {
                        // Actualizar el departamento del manager si cambió
                        existingManager.Department = dto.Position ?? "General";
                        await _managerRepository.UpdateAsync(existingManager);
                    }
                }

                return Ok(new
                {
                    message = "Empleado actualizado correctamente",
                    manager_id = updatedEmployee.manager?.manager_id,
                    manager_name = updatedEmployee.manager?.User?.name ?? "Sin asignar",
                    department = updatedEmployee.manager?.Department,
                    role = currentRole
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = $"Error al actualizar empleado: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(long id)
        {
            try
            {
                Console.WriteLine($"Intentando eliminar empleado con ID: {id}");

                // Obtener el rol del usuario antes de eliminar
                string role = await GetUserRoleById(id);
                Console.WriteLine($"Rol del usuario: {role}");

                // Verificar si el empleado existe antes de intentar eliminarlo
                var employeeExists = await _employeeService.EmployeeExistsAsync(id);

                if (employeeExists)
                {
                    try
                    {
                        // Eliminar el empleado
                        Console.WriteLine("Eliminando empleado...");
                        await _employeeService.DeleteEmployeeAsync(id);
                        Console.WriteLine("Empleado eliminado exitosamente de la tabla employees");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al eliminar empleado: {ex.Message}");
                        // Continuamos con la eliminación del usuario incluso si el empleado no existe
                    }
                }
                else
                {
                    Console.WriteLine("El empleado no existe en la tabla employees, continuando con la eliminación del usuario");
                }

                try
                {
                    // Si era manager, eliminar de la tabla managers
                    if (role?.ToLower() == "manager")
                    {
                        Console.WriteLine("Eliminando registro de manager...");
                        await _managerRepository.DeleteAsync(id);
                        Console.WriteLine("Manager eliminado exitosamente");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar registro de manager: {ex.Message}");
                    // Continuamos con la eliminación del usuario incluso si el manager no existe
                }

                try
                {
                    // Eliminar el usuario asociado
                    Console.WriteLine("Eliminando usuario asociado...");
                    await _userService.DeleteUserAsync((int)id);
                    Console.WriteLine("Usuario asociado eliminado exitosamente");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar usuario asociado: {ex.Message}");
                    return BadRequest(new { error = $"Error al eliminar usuario asociado: {ex.Message}" });
                }

                Console.WriteLine("Proceso de eliminación completado exitosamente");
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general al eliminar empleado: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
                return BadRequest(new { error = $"Error al eliminar empleado: {ex.Message}" });
            }
        }

        // Método auxiliar para obtener el rol del usuario
        private async Task<string> GetUserRoleById(long id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync((int)id);
                return user?.role ?? "empleado";
            }
            catch
            {
                return "empleado"; // Valor por defecto
            }
        }

        [HttpPost("bulk-upload")]
        [Authorize(Roles = "superusuario,manager")]
        public async Task<IActionResult> BulkUploadEmployees([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No se ha proporcionado ningún archivo" });
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".csv" && extension != ".xlsx" && extension != ".xls")
            {
                return BadRequest(new { error = "Formato de archivo no soportado. Use CSV o Excel (.xlsx, .xls)" });
            }

            var results = new
            {
                successful = new List<object>(),
                failed = new List<object>(),
                totalProcessed = 0
            };

            int totalProcessed = 0;

            try
            {
                List<BulkEmployeeDTO> employees;

                if (extension == ".csv")
                {
                    employees = await ParseCSVFile(file);
                }
                else
                {
                    employees = await ParseExcelFile(file);
                }

                totalProcessed = employees.Count;

                foreach (var employeeDto in employees)
                {
                    try
                    {
                        // Validar que el empleado no exista
                        var existingEmployee = await _employeeService.GetByIdAsync(employeeDto.Id);
                        if (existingEmployee != null)
                        {
                            results.failed.Add(new
                            {
                                id = employeeDto.Id,
                                name = employeeDto.Name,
                                error = "El empleado ya existe en el sistema"
                            });
                            continue;
                        }

                        // Validar email único
                        var existingUser = await _userRepository.GetUserByEmailAsync(employeeDto.Email);
                        if (existingUser != null)
                        {
                            results.failed.Add(new
                            {
                                id = employeeDto.Id,
                                name = employeeDto.Name,
                                error = $"El email {employeeDto.Email} ya está registrado"
                            });
                            continue;
                        }

                        // Validar que el manager exista si se proporciona
                        if (employeeDto.ManagerId.HasValue)
                        {
                            var managerExists = await _managerRepository.GetByIdAsync(employeeDto.ManagerId.Value) != null;
                            if (!managerExists)
                            {
                                results.failed.Add(new
                                {
                                    id = employeeDto.Id,
                                    name = employeeDto.Name,
                                    error = $"El manager con ID {employeeDto.ManagerId} no existe"
                                });
                                continue;
                            }
                        }

                        // Crear contraseña hasheada
                        string plainPassword = string.IsNullOrEmpty(employeeDto.Password)
                            ? "password123"
                            : employeeDto.Password;
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                        // Crear usuario
                        var user = new User
                        {
                            id = (int)employeeDto.Id,
                            email = employeeDto.Email,
                            name = employeeDto.Name,
                            passwordHash = hashedPassword,
                            role = employeeDto.Role ?? "empleado",
                            username = employeeDto.Username ?? employeeDto.Name.ToLower().Replace(" ", ".")
                        };
                        await _userRepository.SaveAsync(user);

                        // Si el rol es manager, crear registro en tabla managers
                        if (employeeDto.Role?.ToLower() == "manager")
                        {
                            var newManager = new Manager
                            {
                                manager_id = employeeDto.Id,
                                Department = employeeDto.Position ?? "General"
                            };
                            await _managerRepository.AddAsync(newManager);
                        }

                        // Crear empleado
                        var employee = new Employee
                        {
                            id = employeeDto.Id,
                            position = employeeDto.Position,
                            manager_id = employeeDto.ManagerId
                        };
                        await _employeeService.AddEmployeeAsync(employee);

                        results.successful.Add(new
                        {
                            id = employeeDto.Id,
                            name = employeeDto.Name,
                            email = employeeDto.Email,
                            role = employeeDto.Role
                        });
                    }
                    catch (Exception ex)
                    {
                        results.failed.Add(new
                        {
                            id = employeeDto.Id,
                            name = employeeDto.Name,
                            error = $"Error al procesar: {ex.Message}"
                        });
                    }
                }

                return Ok(new
                {
                    message = "Proceso de carga masiva completado",
                    summary = new
                    {
                        total = totalProcessed,
                        successful = results.successful.Count,
                        failed = results.failed.Count
                    },
                    details = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al procesar el archivo",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Parsea archivo CSV
        /// </summary>
        private async Task<List<BulkEmployeeDTO>> ParseCSVFile(IFormFile file)
        {
            var employees = new List<BulkEmployeeDTO>();

            using (var reader = new StreamReader(file.OpenReadStream(), System.Text.Encoding.UTF8))
            {
                // Leer header
                var headerLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(headerLine))
                {
                    throw new Exception("El archivo CSV está vacío");
                }

                // Procesar cada línea
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var values = line.Split(',');

                    if (values.Length < 4)
                    {
                        continue; // Saltar líneas inválidas
                    }

                    var employee = new BulkEmployeeDTO
                    {
                        Id = long.Parse(values[0].Trim()),
                        Name = values[1].Trim(),
                        Email = values[2].Trim(),
                        Position = values[3].Trim(),
                        Salary = values.Length > 4 && !string.IsNullOrEmpty(values[4].Trim())
                            ? decimal.Parse(values[4].Trim())
                            : 0,
                        Role = values.Length > 5 && !string.IsNullOrEmpty(values[5].Trim())
                            ? values[5].Trim()
                            : "empleado",
                        Username = values.Length > 6 && !string.IsNullOrEmpty(values[6].Trim())
                            ? values[6].Trim()
                            : string.Empty,
                        Password = values.Length > 7 && !string.IsNullOrEmpty(values[7].Trim())
                            ? values[7].Trim()
                            : string.Empty,
                        ManagerId = values.Length > 8 && !string.IsNullOrEmpty(values[8].Trim())
                            ? long.Parse(values[8].Trim())
                            : (long?)null
                    };

                    employees.Add(employee);
                }
            }

            return employees;
        }

        /// <summary>
        /// Parsea archivo Excel usando ClosedXML
        /// </summary>
        private async Task<List<BulkEmployeeDTO>> ParseExcelFile(IFormFile file)
        {
            var employees = new List<BulkEmployeeDTO>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (var workbook = new ClosedXML.Excel.XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Saltar header

                    foreach (var row in rows)
                    {
                        try
                        {
                            var employee = new BulkEmployeeDTO
                            {
                                Id = long.Parse(row.Cell(1).GetString()),
                                Name = row.Cell(2).GetString(),
                                Email = row.Cell(3).GetString(),
                                Position = row.Cell(4).GetString(),
                                Salary = row.Cell(5).IsEmpty() ? 0 : decimal.Parse(row.Cell(5).GetString()),
                                Role = row.Cell(6).IsEmpty() ? "empleado" : row.Cell(6).GetString(),
                                Username = row.Cell(7).IsEmpty() ? string.Empty : row.Cell(7).GetString(),
                                Password = row.Cell(8).IsEmpty() ? string.Empty : row.Cell(8).GetString(),
                                ManagerId = row.Cell(9).IsEmpty() ? (long?)null : long.Parse(row.Cell(9).GetString())
                            };

                            employees.Add(employee);
                        }
                        catch
                        {
                            // Saltar filas con errores
                            continue;
                        }
                    }
                }
            }

            return employees;
        }
    }

    /// <summary>
    /// DTO para carga masiva de empleados
    /// </summary>
    public class BulkEmployeeDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public long? ManagerId { get; set; }
    }

}
