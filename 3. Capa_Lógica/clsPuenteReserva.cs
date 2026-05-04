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
            // CORRECCIÓN 1: Validaciones de negocio AQUÍ, antes de tocar la BD
            if (DatosI == null)
                throw new ArgumentNullException("DatosI", "Los datos de la reserva no pueden ser nulos.");

            if (DatosI.Fecha_salida.Date <= DatosI.Fecha_ingreso.Date)
                throw new ArgumentException("La fecha de salida debe ser posterior a la de ingreso.");

            if (DatosI.Numero_personas <= 0)
                throw new ArgumentException("El número de personas debe ser mayor a cero.");

            if (DatosI.Id_huesped <= 0)
                throw new ArgumentException("El huésped no es válido.");

            if (DatosI.Id_alojamiento <= 0)
                throw new ArgumentException("El alojamiento no es válido.");

            try
            {
                objOperacion.IngresarReserva(DatosI);
            }
            catch (Exception ex)
            {
                // CORRECCIÓN 2: Relanzar con contexto claro para la UI
                throw new Exception("No se pudo registrar la reserva: " + ex.Message, ex);
            }
        }

        public DataTable ConsultarReservasPorCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                throw new ArgumentException("La cédula no puede estar vacía.");
            try
            {
                return objOperacion.ConsultarReservasPorCedula(cedula);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar reservas por cédula: " + ex.Message, ex);
            }
        }

        public DataTable BuscarReservaPorId(int id)
        {
            try
            {
                return objOperacion.BuscarReservaPorId(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar la reserva: " + ex.Message, ex);
            }
        }

        public void ActualizarReserva(clsReserva Datos)
        {
            if (Datos == null)
                throw new ArgumentNullException("Datos", "Los datos no pueden ser nulos.");

            if (Datos.Id_reserva <= 0)
                throw new ArgumentException("El ID de reserva no es válido para actualizar.");

            try
            {
                objOperacion.ActualizarReserva(Datos);
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo actualizar la reserva: " + ex.Message, ex);
            }
        }

        public DataTable ConsultarTodasLasReservas()
        {
            try
            {
                return objOperacion.ConsultarTodasLasReservas();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar reservas: " + ex.Message, ex);
            }
        }

        public void EliminarReserva(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de reserva no es válido para eliminar.");
            try
            {
                objOperacion.EliminarReserva(id);
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo eliminar la reserva: " + ex.Message, ex);
            }
        }
    }
}