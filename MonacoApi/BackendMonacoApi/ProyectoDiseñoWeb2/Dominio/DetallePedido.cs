using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class DetallePedido
    {
        [Key]
        public int Id_Detalle { get; set; }

        public int Id_Pedido { get; set; }

        [ForeignKey("Id_Pedido")]
        public Pedido Pedido { get; set; }

        public int Id_Producto { get; set; }

        [ForeignKey("Id_Producto")]
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}