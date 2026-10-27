using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class SolicitudSoporte
    {
        [Key]
        public int Id_Solicitud { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public int? Id_Cliente { get; set; }

        [ForeignKey("Id_Cliente")]
        public Cliente Cliente { get; set; }
    }
}
