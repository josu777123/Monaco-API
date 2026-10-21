using System.ComponentModel.DataAnnotations;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class Sucursal
    {
        [Key]
        public int Id_Sucursal { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Telefono { get; set; }
        public string Horario { get; set; }
        public bool Activo { get; set; }
    }
}
