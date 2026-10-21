using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoDiseñoWeb2.Dominio
{
    public class Pago
    {
        [Key]
        public int Id_Pago { get; set; }
        public int Id_Pedido { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha { get; set; }

        [ForeignKey("Id_Pedido")]
        public Pedido Pedido { get; set; }
    }
}
