using Capa_Entidades;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos
{
    public class clsOperacionReserva
    {
        clsConexion objConectar = new clsConexion();

        public void IngresarReserva(clsReserva DatosI)
        {
            try
            {
                objConectar.Abrir();
                // Asegúrate que los nombres de las columnas coincidan exactamente con tu imagen:
                // Id_reserva, fecha_ingreso, fecha_salida, numero_personas, tipo, Id_huesped, Id_alojamiento
                string query = "INSERT INTO Reserva (fecha_ingreso, fecha_salida, numero_personas, tipo, Id_huesped, Id_alojamiento) " +
                               "VALUES (@ingreso, @salida, @numPer, @tipo, @idHuesped, @idAloj)";

                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);

                // Usamos .Date para evitar conflictos de horas/minutos en SQL
                comandoSql.Parameters.AddWithValue("@ingreso", DatosI.Fecha_ingreso.Date);
                comandoSql.Parameters.AddWithValue("@salida", DatosI.Fecha_salida.Date);
                comandoSql.Parameters.AddWithValue("@numPer", DatosI.Numero_personas);
                comandoSql.Parameters.AddWithValue("@tipo", DatosI.Tipo);
                comandoSql.Parameters.AddWithValue("@idHuesped", DatosI.Id_huesped);
                comandoSql.Parameters.AddWithValue("@idAloj", DatosI.Id_alojamiento);

                comandoSql.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // Esto es vital para saber POR QUÉ falló (ej. Llave foránea, nulos, etc.)
                throw new Exception("Error en SQL al insertar: " + ex.Message);
            }
            finally
            {
                objConectar.Cerrar();
            }
        }
        // Método para consultar reservas filtrando por la Cédula del huésped
        public DataTable ConsultarReservasPorCedula(string cedula)
        {
            DataTable dtReservas = new DataTable();
            try
            {
                objConectar.Abrir();
                // Usamos INNER JOIN para traer datos legibles en lugar de solo IDs
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

                // Llenamos la tabla de datos con el resultado de la consulta
                SqlDataAdapter adaptador = new SqlDataAdapter(comandoSql);
                adaptador.Fill(dtReservas);
            }
            finally
            {
                objConectar.Cerrar();
            }
            return dtReservas;
        }
        // Método para buscar una reserva específica por ID y traer la cédula del huésped
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
        // Método para listar todas las reservas de forma legible para el ListBox
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

        // Método para eliminar la reserva físicamente
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