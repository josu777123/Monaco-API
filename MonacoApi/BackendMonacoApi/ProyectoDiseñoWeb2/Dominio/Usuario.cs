using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class Usuario
    {
        [Key]
        public int Id_Usuario { get; set; }
        public string Correo { get; set; }
        public string PasswordHash { get; set; }
        public string Rol { get; set; }
        public int? Id_Cliente { get; set; }
        public bool Activo { get; set; }

        [ForeignKey("Id_Cliente")]
        public Cliente Cliente { get; set; }
    }
}
