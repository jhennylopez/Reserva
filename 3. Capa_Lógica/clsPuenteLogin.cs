using _2.Capa_Datos;
using Capa_Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Logica
{
    public class clsPuenteLogin
    {
        clsOperacionLogin objDato = new clsOperacionLogin();

        public bool Ingresar(string correo, string pass)
        {
            return objDato.ValidarCredenciales(correo, pass);
        }
    }
}