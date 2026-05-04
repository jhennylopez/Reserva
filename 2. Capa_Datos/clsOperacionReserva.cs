using Capa_Entidades;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos
{
    public class clsOperacionReserva
    {
        clsConexion objConectar = new clsConexion();

        public void IngresarReservaConFactura(clsReserva DatosReserva, clsFactura DatosFactura)
        {
            SqlTransaction transaccion = null;
            try
            {
                objConectar.Abrir();
                transaccion = objConectar.conectar.BeginTransaction();

                // 1. Insertar Reserva y obtener el ID generado mediante SCOPE_IDENTITY()
                string queryReserva = @"INSERT INTO Reserva (fecha_ingreso, fecha_salida, numero_personas, tipo, Id_huesped, Id_alojamiento) 
                                VALUES (@ingreso, @salida, @numPer, @tipo, @idHuesped, @idAloj);
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmdReserva = new SqlCommand(queryReserva, objConectar.conectar, transaccion);
                cmdReserva.Parameters.AddWithValue("@ingreso", DatosReserva.Fecha_ingreso.Date);
                cmdReserva.Parameters.AddWithValue("@salida", DatosReserva.Fecha_salida.Date);
                cmdReserva.Parameters.AddWithValue("@numPer", DatosReserva.Numero_personas);
                cmdReserva.Parameters.AddWithValue("@tipo", DatosReserva.Tipo);
                cmdReserva.Parameters.AddWithValue("@idHuesped", DatosReserva.Id_huesped);
                cmdReserva.Parameters.AddWithValue("@idAloj", DatosReserva.Id_alojamiento);

                int idReservaGenerado = Convert.ToInt32(cmdReserva.ExecuteScalar());

                // 2. Insertar Factura vinculada al ID anterior
                string queryFactura = @"INSERT INTO Factura (Id_reserva, dias_ocupacion, subtotal, impuestos, total) 
                                VALUES (@idRes, @dias, @subtotal, @impuestos, @total)";

                SqlCommand cmdFactura = new SqlCommand(queryFactura, objConectar.conectar, transaccion);
                cmdFactura.Parameters.AddWithValue("@idRes", idReservaGenerado);
                cmdFactura.Parameters.AddWithValue("@dias", DatosFactura.Dias_ocupacion);
                cmdFactura.Parameters.AddWithValue("@subtotal", DatosFactura.Subtotal);
                cmdFactura.Parameters.AddWithValue("@impuestos", DatosFactura.Impuestos);
                cmdFactura.Parameters.AddWithValue("@total", DatosFactura.Total);

                cmdFactura.ExecuteNonQuery();

                transaccion.Commit();
            }
            catch (Exception ex)
            {
                if (transaccion != null) transaccion.Rollback();
                throw new Exception("Error al procesar la transacción de reserva: " + ex.Message);
            }
            finally
            {
                objConectar.Cerrar();
            }
        }

        public DataTable ConsultarReservasPorCedula(string cedula)
        {
            DataTable dtReservas = new DataTable();
            try
            {
                objConectar.Abrir();
                string query = @"SELECT 
                                    r.Id_reserva AS [ID Reserva],
                                    h.nombres + ' ' + h.apellidos AS [Huésped],
                                    a.descripcion + ' (' + a.ubicacion + ')' AS [Alojamiento],
                                    r.fecha_ingreso AS [Fecha Ingreso],
                                    r.fecha_salida AS [Fecha Salida],
                                    r.tipo AS [Tipo],
                                    r.numero_personas AS [N° Personas]
                                 FROM Reserva r
                                 INNER JOIN Huesped h ON r.Id_huesped = h.Id_huesped
                                 INNER JOIN Alojamiento a ON r.Id_alojamiento = a.Id_alojamiento
                                 WHERE h.ci = @cedula";

                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@cedula", cedula);

                SqlDataAdapter adaptador = new SqlDataAdapter(comandoSql);
                adaptador.Fill(dtReservas);
            }
            finally
            {
                objConectar.Cerrar();
            }
            return dtReservas;
        }

        public DataTable BuscarReservaPorId(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                objConectar.Abrir();
                string query = @"SELECT r.*, h.ci FROM Reserva r 
                                 INNER JOIN Huesped h ON r.Id_huesped = h.Id_huesped 
                                 WHERE (@id = 0 OR r.Id_reserva = @id)";
                SqlCommand cmd = new SqlCommand(query, objConectar.conectar);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
            }
            finally
            {
                objConectar.Cerrar();
            }
            return dt;
        }

        public void ActualizarReserva(clsReserva Datos)
        {
            try
            {
                objConectar.Abrir();
                string query = @"UPDATE Reserva SET fecha_ingreso=@fi, fecha_salida=@fs, 
                                 numero_personas=@np, tipo=@t, Id_alojamiento=@ia 
                                 WHERE Id_reserva=@id";
                SqlCommand cmd = new SqlCommand(query, objConectar.conectar);
                cmd.Parameters.AddWithValue("@fi", Datos.Fecha_ingreso);
                cmd.Parameters.AddWithValue("@fs", Datos.Fecha_salida);
                cmd.Parameters.AddWithValue("@np", Datos.Numero_personas);
                cmd.Parameters.AddWithValue("@t", Datos.Tipo);
                cmd.Parameters.AddWithValue("@ia", Datos.Id_alojamiento);
                cmd.Parameters.AddWithValue("@id", Datos.Id_reserva);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }

        public DataTable ConsultarTodasLasReservas()
        {
            DataTable dt = new DataTable();
            try
            {
                objConectar.Abrir();
                string query = @"SELECT r.Id_reserva, h.nombres + ' ' + h.apellidos AS Huésped, 
                                a.descripcion AS Alojamiento, r.fecha_ingreso
                         FROM Reserva r
                         INNER JOIN Huesped h ON r.Id_huesped = h.Id_huesped
                         INNER JOIN Alojamiento a ON r.Id_alojamiento = a.Id_alojamiento";
                SqlDataAdapter adp = new SqlDataAdapter(query, objConectar.conectar);
                adp.Fill(dt);
            }
            finally
            {
                objConectar.Cerrar();
            }
            return dt;
        }

        public void EliminarReserva(int id)
        {
            try
            {
                objConectar.Abrir();
                SqlCommand cmd = new SqlCommand("DELETE FROM Reserva WHERE Id_reserva = @id", objConectar.conectar);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }
    }
}