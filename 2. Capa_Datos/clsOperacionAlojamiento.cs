using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidades;


namespace Capa_Datos
{
    public class clsOperacionAlojamiento
    {
        clsConexion objConectar = new clsConexion();

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
                // SE AÑADIÓ: precio_por_noche
                string query = "INSERT INTO Alojamiento (descripcion, ubicacion, max_huespedes, num_habitaciones, num_banos, Id_administrador, precio_por_noche) " +
                               "VALUES (@desc, @ubi, @maxH, @numHab, @numBan, @idAdmin, @precio)";
                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);

                comandoSql.Parameters.AddWithValue("@desc", DatosI.Descripcion);
                comandoSql.Parameters.AddWithValue("@ubi", DatosI.Ubicacion);
                comandoSql.Parameters.AddWithValue("@maxH", DatosI.Max_huespedes);
                comandoSql.Parameters.AddWithValue("@numHab", DatosI.Num_habitaciones);
                comandoSql.Parameters.AddWithValue("@numBan", DatosI.Num_banos);
                comandoSql.Parameters.AddWithValue("@idAdmin", DatosI.Id_administrador);
                comandoSql.Parameters.AddWithValue("@precio", DatosI.Precio_por_noche);

                comandoSql.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }

        public List<clsAlojamiento> BuscarAlojamientosPorHuespedes(int cantidad)
        {
            List<clsAlojamiento> lista = new List<clsAlojamiento>();
            try
            {
                objConectar.Abrir();
                string query;

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
                        Id_administrador = Convert.ToInt32(leerDatos["Id_administrador"]),
                        Precio_por_noche = Convert.ToDecimal(leerDatos["precio_por_noche"]) // SE AÑADIÓ LA LECTURA
                    });
                }
            }
            finally
            {
                objConectar.Cerrar();
            }
            return lista;
        }

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
                        Ubicacion = Convert.ToString(leerDatos["ubicacion"]),
                        Max_huespedes = Convert.ToInt32(leerDatos["max_huespedes"]),
                        Num_habitaciones = Convert.ToInt32(leerDatos["num_habitaciones"]),
                        Num_banos = Convert.ToInt32(leerDatos["num_banos"]),
                        Id_administrador = Convert.ToInt32(leerDatos["Id_administrador"]),
                        Precio_por_noche = Convert.ToDecimal(leerDatos["precio_por_noche"]) // VITAL PARA LA FACTURA
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

        public void ActualizarAlojamiento(clsAlojamiento DatosI)
        {
            try
            {
                objConectar.Abrir();
                // SE AÑADIÓ: precio_por_noche
                string query = "UPDATE Alojamiento SET descripcion=@desc, ubicacion=@ubi, max_huespedes=@maxH, " +
                               "num_habitaciones=@numHab, num_banos=@numBan, Id_administrador=@idAdmin, precio_por_noche=@precio " +
                               "WHERE Id_alojamiento=@id";

                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@id", DatosI.Id_alojamiento);
                comandoSql.Parameters.AddWithValue("@desc", DatosI.Descripcion);
                comandoSql.Parameters.AddWithValue("@ubi", DatosI.Ubicacion);
                comandoSql.Parameters.AddWithValue("@maxH", DatosI.Max_huespedes);
                comandoSql.Parameters.AddWithValue("@numHab", DatosI.Num_habitaciones);
                comandoSql.Parameters.AddWithValue("@numBan", DatosI.Num_banos);
                comandoSql.Parameters.AddWithValue("@idAdmin", DatosI.Id_administrador);
                comandoSql.Parameters.AddWithValue("@precio", DatosI.Precio_por_noche);

                comandoSql.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }

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