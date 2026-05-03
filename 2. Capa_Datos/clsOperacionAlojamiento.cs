using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidades;

namespace Capa_Datos
{
    public class clsOperacionAlojamiento
    {
        clsConexion objConectar = new clsConexion();

        // Buscar el ID del administrador usando su teléfono
        public int ObtenerIdAdminPorTelefono(string telefono)
        {
            int idAdmin = 0;
            try
            {
                objConectar.Abrir();
                SqlCommand comandoSql = new SqlCommand("SELECT Id_administrador FROM Administrador WHERE telefono = @tel", objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@tel", telefono);

                object resultado = comandoSql.ExecuteScalar();
                if (resultado != null)
                {
                    idAdmin = Convert.ToInt32(resultado);
                }
            }
            finally
            {
                objConectar.Cerrar();
            }
            return idAdmin;
        }

        public void IngresarAlojamiento(clsAlojamiento DatosI)
        {
            try
            {
                objConectar.Abrir();
                string query = "INSERT INTO Alojamiento (descripcion, ubicacion, max_huespedes, num_habitaciones, num_banos, Id_administrador) " +
                               "VALUES (@desc, @ubi, @maxH, @numHab, @numBan, @idAdmin)";
                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);

                comandoSql.Parameters.AddWithValue("@desc", DatosI.Descripcion);
                comandoSql.Parameters.AddWithValue("@ubi", DatosI.Ubicacion);
                comandoSql.Parameters.AddWithValue("@maxH", DatosI.Max_huespedes);
                comandoSql.Parameters.AddWithValue("@numHab", DatosI.Num_habitaciones);
                comandoSql.Parameters.AddWithValue("@numBan", DatosI.Num_banos);
                comandoSql.Parameters.AddWithValue("@idAdmin", DatosI.Id_administrador);

                comandoSql.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }
        // Método para buscar alojamientos por capacidad de huéspedes
        public List<clsAlojamiento> BuscarAlojamientosPorHuespedes(int cantidad)
        {
            List<clsAlojamiento> lista = new List<clsAlojamiento>();
            try
            {
                objConectar.Abrir();
                string query;

                // Si la cantidad es 0 (caja vacía), mostramos todos. Si no, filtramos.
                if (cantidad == 0)
                {
                    query = "SELECT * FROM Alojamiento";
                }
                else
                {
                    query = "SELECT * FROM Alojamiento WHERE max_huespedes >= @cantidad";
                }

                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);

                if (cantidad > 0)
                {
                    comandoSql.Parameters.AddWithValue("@cantidad", cantidad);
                }

                SqlDataReader leerDatos = comandoSql.ExecuteReader();

                while (leerDatos.Read())
                {
                    lista.Add(new clsAlojamiento
                    {
                        Id_alojamiento = Convert.ToInt32(leerDatos["Id_alojamiento"]),
                        Descripcion = Convert.ToString(leerDatos["descripcion"]),
                        Ubicacion = Convert.ToString(leerDatos["ubicacion"]),
                        Max_huespedes = Convert.ToInt32(leerDatos["max_huespedes"]),
                        Num_habitaciones = Convert.ToInt32(leerDatos["num_habitaciones"]),
                        Num_banos = Convert.ToInt32(leerDatos["num_banos"]),
                        Id_administrador = Convert.ToInt32(leerDatos["Id_administrador"])
                    });
                }
            }
            finally
            {
                objConectar.Cerrar();
            }
            return lista;
        }
        // Método para buscar un alojamiento específico por su ID
        public clsAlojamiento BuscarAlojamiento(int idBuscar)
        {
            try
            {
                objConectar.Abrir();
                SqlCommand comandoSql = new SqlCommand("SELECT * FROM Alojamiento WHERE Id_alojamiento = @id", objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@id", idBuscar);
                SqlDataReader leerDatos = comandoSql.ExecuteReader();

                if (leerDatos.Read())
                {
                    return new clsAlojamiento
                    {
                        Id_alojamiento = Convert.ToInt32(leerDatos["Id_alojamiento"]),
                        Descripcion = Convert.ToString(leerDatos["descripcion"]),
                        Ubicacion = Convert.ToString(leerDatos["ubicacion"])
                    };
                }
                return null;
            }
            catch
            {
                return null;
            }
            finally
            {
                objConectar.Cerrar();
            }
        }

        // Método para eliminar de la base de datos
        public void EliminarAlojamiento(int idEliminar)
        {
            try
            {
                objConectar.Abrir();
                SqlCommand comandoSql = new SqlCommand("DELETE FROM Alojamiento WHERE Id_alojamiento = @id", objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@id", idEliminar);
                comandoSql.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }
        // Método para actualizar los datos del alojamiento
        public void ActualizarAlojamiento(clsAlojamiento DatosI)
        {
            try
            {
                objConectar.Abrir();
                string query = "UPDATE Alojamiento SET descripcion=@desc, ubicacion=@ubi, max_huespedes=@maxH, " +
                               "num_habitaciones=@numHab, num_banos=@numBan, Id_administrador=@idAdmin " +
                               "WHERE Id_alojamiento=@id";
                
                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@id", DatosI.Id_alojamiento);
                comandoSql.Parameters.AddWithValue("@desc", DatosI.Descripcion);
                comandoSql.Parameters.AddWithValue("@ubi", DatosI.Ubicacion);
                comandoSql.Parameters.AddWithValue("@maxH", DatosI.Max_huespedes);
                comandoSql.Parameters.AddWithValue("@numHab", DatosI.Num_habitaciones);
                comandoSql.Parameters.AddWithValue("@numBan", DatosI.Num_banos);
                comandoSql.Parameters.AddWithValue("@idAdmin", DatosI.Id_administrador);
                
                comandoSql.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }

        // Método para obtener el teléfono del administrador sabiendo su ID (para mostrarlo en pantalla)
        public string ObtenerTelefonoAdminPorId(int idAdmin)
        {
            string telefono = "";
            try
            {
                objConectar.Abrir();
                SqlCommand comandoSql = new SqlCommand("SELECT telefono FROM Administrador WHERE Id_administrador = @id", objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@id", idAdmin);
                
                object resultado = comandoSql.ExecuteScalar();
                if (resultado != null)
                {
                    telefono = resultado.ToString();
                }
            }
            finally
            {
                objConectar.Cerrar();
            }
            return telefono;
        }
    }
}