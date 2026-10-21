
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class Pedido
    {
        [Key]
        public int Id_Pedido { get; set; }

        public int Id_Cliente { get; set; }

        [ForeignKey("Id_Cliente")]
        public Cliente Cliente { get; set; }

        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public string DireccionEntrega { get; set; }
        public string ModalidadEntrega { get; set; }

        public int? Id_Sucursal { get; set; }

        [ForeignKey("Id_Sucursal")]
        public Sucursal Sucursal { get; set; }
    }
}
