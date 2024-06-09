using System.ComponentModel.DataAnnotations;

namespace AdministradorPedidosApp.Models
{
    public class LoginModel
    {
        [Required]
        public string User { get; set; }

        [Required]
        public string Password { get; set; }

        public bool KeepLoggedIn { get; set; }
    }
}
