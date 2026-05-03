using Capa_Datos;
using Capa_Entidades;
using System;
using System.Collections.Generic;

namespace Capa_Logica
{
    public class clsPuenteAlojamiento
    {
        clsOperacionAlojamiento objOperacion = new clsOperacionAlojamiento();

        public int ObtenerIdAdminPorTelefono(string telefono)
        {
            return objOperacion.ObtenerIdAdminPorTelefono(telefono);
        }

        public void IngresarAlojamiento(clsAlojamiento DatosI)
        {
            objOperacion.IngresarAlojamiento(DatosI);
        }
        public List<clsAlojamiento> BuscarAlojamientosPorHuespedes(int cantidad)
        {
            return objOperacion.BuscarAlojamientosPorHuespedes(cantidad);
        }
        public clsAlojamiento BuscarAlojamiento(int idBuscar)
        {
            return objOperacion.BuscarAlojamiento(idBuscar);
        }

        public void EliminarAlojamiento(int idEliminar)
        {
            objOperacion.EliminarAlojamiento(idEliminar);
        }
        public void ActualizarAlojamiento(clsAlojamiento DatosI)
        {
            objOperacion.ActualizarAlojamiento(DatosI);
        }

        public string ObtenerTelefonoAdminPorId(int idAdmin)
        {
            return objOperacion.ObtenerTelefonoAdminPorId(idAdmin);
        }
    }
}