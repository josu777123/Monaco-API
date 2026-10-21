using System.ComponentModel.DataAnnotations;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class Cliente
    {
        [Key]
        public int Id_Cliente { get; set; }

        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string CI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool Activo { get; set; }
    }
}
