using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class Producto
    {
        [Key]
        public int Id_Producto { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal PorcentajeAlcohol { get; set; }
        public int VolumenMl { get; set; }
        public int Stock { get; set; }
        public int Id_Categoria { get; set; }
        public bool Activo { get; set; }

        [ForeignKey("Id_Categoria")]
        public Categoria Categoria { get; set; }
    }
}
