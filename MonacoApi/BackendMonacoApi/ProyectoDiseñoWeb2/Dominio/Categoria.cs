using System.ComponentModel.DataAnnotations;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class Categoria
    {
        [Key]
        public int Id_Categoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Activo { get; set; }

    }
}
