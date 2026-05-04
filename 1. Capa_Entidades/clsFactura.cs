using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Capa_Entidades
{
    public class clsFactura
    {
        public int Id_factura { get; set; }
        public int Id_reserva { get; set; }
        public DateTime Fecha_emision { get; set; }
        public int Dias_ocupacion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
    }
}
