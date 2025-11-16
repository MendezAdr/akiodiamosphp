using CrudCafeteria.Models;
using CrudCafeteria.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CrudCafeteria.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioService _usuarioSvc;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public string Username { get; set; } = string.Empty;
        [BindProperty]
        public string Password { get; set; } = string.Empty;
        
        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;
        [TempData]
        public string SuccessMessage { get; set; } = string.Empty;

        public LoginModel(UsuarioService usuarioSvc, ILogger<LoginModel> logger)
        {
            _usuarioSvc = usuarioSvc;
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("--- Página de Login cargada (OnGet) ---");
        }

        // --- MANEJADOR PARA EL LOGIN ---
        public async Task<IActionResult> OnPostLoginAsync()
        {
            _logger.LogInformation("--- OnPostLoginAsync INVOCADO ---");
            _logger.LogInformation("Valor recibido de 'Username': {Username}", Username);
            // El Password NO debe logearse directamente por seguridad

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("El modelo de login no es válido. ModelState: {@ModelState}", ModelState); // Logea detalles
                ErrorMessage = "Datos del formulario no válidos.";
                return Page();
            }

            var usuario = await _usuarioSvc.ValidarUsuarioAsync(Username, Password);

            if (usuario != null)
            {
                // ... (Lógica de creación de claims y cookie) ...
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Username),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Usuario") // Asegura que el Rol no sea null
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // La cookie persiste entre sesiones del navegador
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Expira en 30 minutos
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                
                _logger.LogInformation("Login exitoso para: {Username}. Redirigiendo a /gastos.", Username);
                return LocalRedirect("/gastos");
            }

            _logger.LogWarning("Login fallido para: {Username}. 'ValidarUsuarioAsync' devolvió null (Usuario o contraseña incorrectos).", Username);
            ErrorMessage = "Usuario o contraseña incorrectos.";
            return Page();
        }

        // --- MANEJADOR PARA EL USUARIO DE PRUEBA ---
        public async Task<IActionResult> OnPostTestUserAsync()
        {
            _logger.LogInformation("--- OnPostTestUserAsync INVOCADO ---");
            try
            {
                // 1. Verifica si ya existe un usuario 'admin'
                //    (Nota: 'ValidarUsuarioAsync' necesita la contraseña correcta "123456")
                var existingUser = await _usuarioSvc.ValidarUsuarioAsync("admin", "123456");
                if (existingUser != null)
                {
                    _logger.LogWarning("El usuario 'admin' de prueba ya existe.");
                    SuccessMessage = "El usuario 'admin' ya existe. Intenta iniciar sesión.";
                    return Page();
                }

                // 2. Si no existe, verifica si el 'admin' de la BD tiene una contraseña incorrecta
                var userInDb = await _usuarioSvc.GetUsuarioByUsernameAsync("admin"); // <-- Necesitaremos añadir este método
                if (userInDb != null)
                {
                     _logger.LogWarning("El usuario 'admin' ya existe pero la contraseña de prueba falló. Intenta iniciar sesión.");
                     ErrorMessage = "El usuario 'admin' ya existe. Si olvidaste la contraseña, reseteala en la BD.";
                     return Page();
                }

                // 3. Si no existe, crea el nuevo usuario
                var testUser = new Usuario
                {
                    Username = "admin",
                    Password = "123456", // El servicio lo hashea
                    Email = "admin@test.com",
                    FechaNacimiento = DateTime.Parse("2000-01-01"), // Fecha de ejemplo
                    RolId = 1 // Asume que RolId 1 ("Admin") existe
                };

                await _usuarioSvc.CreateUsuarioAsync(testUser);
                _logger.LogInformation("Usuario 'admin' de prueba creado exitosamente.");
                SuccessMessage = "¡Usuario 'admin' creado! Ahora inicia sesión con admin / 123456.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico al crear usuario de prueba 'admin'.");
                ErrorMessage = $"Error al crear: {ex.Message}";
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Detalle de error interno.");
                }
            }
            
            return Page();
        }
        

        // **NOTA: Necesitas estos métodos en UsuarioService.cs para OnPostTestUserAsync**
        // public async Task<Rol?> GetRolByIdAsync(int id) { ... }
        // public async Task<Rol> CreateRolAsync(Rol rol) { ... }
    }
}