using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidades
{
    public class clsReserva
    {
        public int Id_reserva { get; set; }
        public DateTime Fecha_ingreso { get; set; }
        public DateTime Fecha_salida { get; set; }
        public int Numero_personas { get; set; }
        public string Tipo { get; set; }
        public int Id_huesped { get; set; } 
        public int Id_alojamiento { get; set; } 
    }
}