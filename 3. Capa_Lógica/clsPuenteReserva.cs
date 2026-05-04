using Capa_Datos;
using Capa_Entidades;
using System;
using System.Data;

namespace Capa_Logica
{
    public class clsPuenteReserva
    {
        clsOperacionReserva objOperacion = new clsOperacionReserva();

        // Método de inserción transaccional
        public void IngresarReservaConFactura(clsReserva datosReserva, clsFactura datosFactura)
        {
            objOperacion.IngresarReservaConFactura(datosReserva, datosFactura);
        }

        // Motor de cálculo para la UI y el guardado
        public decimal[] CalcularValoresFactura(DateTime entrada, DateTime salida, decimal precioNoche)
        {
            int dias = (salida.Date - entrada.Date).Days;
            if (dias <= 0) dias = 1;

            decimal subtotal = dias * precioNoche;
            decimal impuestos = subtotal * 0.15m; // IVA 15%
            decimal total = subtotal + impuestos;

            return new decimal[] { dias, subtotal, impuestos, total };
        }


        // Soluciona el error en frmActualizarReserva.cs
        public void ActualizarReserva(clsReserva Datos)
        {
            objOperacion.ActualizarReserva(Datos);
        }

        // Soluciona el error en frmConsultarReserva.cs
        public DataTable ConsultarReservasPorCedula(string cedula)
        {
            return objOperacion.ConsultarReservasPorCedula(cedula);
        }

        // --- MÉTODOS DE CONSULTA Y ELIMINACIÓN ---

        public DataTable ConsultarTodasLasReservas() => objOperacion.ConsultarTodasLasReservas();

        public DataTable BuscarReservaPorId(int id) => objOperacion.BuscarReservaPorId(id);

        public void EliminarReserva(int id) => objOperacion.EliminarReserva(id);
    }
}