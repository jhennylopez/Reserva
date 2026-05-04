using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidades;
using Capa_Datos;

namespace Capa_Logica
{
    public class clsPuenteHuesped
    {
        clsOperacionHuesped objOperacion = new clsOperacionHuesped();

        public List<clsHuesped> ObtenerHuespedes()
        {
            return objOperacion.ListarHuespedes();
        }

        public void IngresarHuesped(clsHuesped DatosI)
        {
            objOperacion.IngresarHuesped(DatosI);
        }

        public clsHuesped BuscarHuesped(int idBuscar)
        {
            return objOperacion.BuscarHuesped(idBuscar);
        }

        public void ActualizarHuesped(clsHuesped DatosI)
        {
            objOperacion.ActualizarHuesped(DatosI);
        }

        public clsHuesped EliminarHuesped(int idEliminar)
        {
            return objOperacion.EliminarHuesped(idEliminar);
        }

        public bool ExisteCedulaDuplicada(string ci, int idHuesped = 0)
        {
            return objOperacion.ExisteCedulaDuplicada(ci, idHuesped);
        }
        public bool ExisteCorreoDuplicado(string correo, int idHuesped = 0)
        {
            return objOperacion.ExisteCorreoDuplicado(correo, idHuesped);
        }
    }
}