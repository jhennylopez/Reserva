using Capa_Datos;
using Capa_Entidades;
using System;
using System.Data;

namespace Capa_Logica
{
    public class clsPuenteReserva
    {
        clsOperacionReserva objOperacion = new clsOperacionReserva();

        public void IngresarReserva(clsReserva DatosI)
        {
            objOperacion.IngresarReserva(DatosI);
        }
        // Agrega esto usando System.Data; en la parte superior si no lo tienes
        public DataTable ConsultarReservasPorCedula(string cedula)
        {
            return objOperacion.ConsultarReservasPorCedula(cedula);
        }
        public DataTable BuscarReservaPorId(int id)
        {
            return objOperacion.BuscarReservaPorId(id);
        }

        public void ActualizarReserva(clsReserva Datos)
        {
            objOperacion.ActualizarReserva(Datos);
        }
        public DataTable ConsultarTodasLasReservas()
        {
            return objOperacion.ConsultarTodasLasReservas();
        }

        public void EliminarReserva(int id)
        {
            objOperacion.EliminarReserva(id);
        }
    }
}