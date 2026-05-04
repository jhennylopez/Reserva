using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidades
{
    public class clsAlojamiento
    {
        public int Id_alojamiento { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public int Max_huespedes { get; set; }
        public int Num_habitaciones { get; set; }
        public int Num_banos { get; set; }
        public int Id_administrador { get; set; } 
    }
}