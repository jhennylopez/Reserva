using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capa_Entidades;

namespace Capa_Datos
{
    public class clsOperacionHuesped
    {
        clsConexion objConectar = new clsConexion();

        public List<clsHuesped> ListarHuespedes()
        {
            List<clsHuesped> lista = new List<clsHuesped>();
            try
            {
                objConectar.Abrir();
                SqlCommand comandoSql = new SqlCommand("SELECT * FROM Huesped", objConectar.conectar);
                SqlDataReader leerDatos = comandoSql.ExecuteReader();

                while (leerDatos.Read())
                {
                    lista.Add(new clsHuesped
                    {
                        Id_huesped = Convert.ToInt32(leerDatos["Id_huesped"]),
                        Ci = Convert.ToString(leerDatos["Ci"]),
                        Nombres = Convert.ToString(leerDatos["nombres"]),
                        Apellidos = Convert.ToString(leerDatos["apellidos"]),
                        Correo = Convert.ToString(leerDatos["correo"]),
                        Telefono = Convert.ToString(leerDatos["telefono"]),
                        Contrasena = Convert.ToString(leerDatos["contrasena"])
                    });
                }
            }
            finally
            {
                objConectar.Cerrar();
            }
            return lista;
        }

        public void IngresarHuesped(clsHuesped DatosI)
        {
            try
            {
                objConectar.Abrir();
                string query = "INSERT INTO Huesped (Ci, nombres, apellidos, correo, telefono, contrasena) VALUES (@ci, @nom, @ape, @correo, @tel, @pass)";
                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@ci", DatosI.Ci);
                comandoSql.Parameters.AddWithValue("@nom", DatosI.Nombres);
                comandoSql.Parameters.AddWithValue("@ape", DatosI.Apellidos);
                comandoSql.Parameters.AddWithValue("@correo", DatosI.Correo);
                comandoSql.Parameters.AddWithValue("@tel", DatosI.Telefono);
                comandoSql.Parameters.AddWithValue("@pass", DatosI.Contrasena);
                comandoSql.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }

        public clsHuesped BuscarHuesped(int idBuscar)
        {
            try
            {
                objConectar.Abrir();
                SqlCommand comandoSql = new SqlCommand("SELECT * FROM Huesped WHERE Id_huesped = @id", objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@id", idBuscar);
                SqlDataReader leerDatos = comandoSql.ExecuteReader();

                if (leerDatos.Read())
                {
                    return new clsHuesped
                    {
                        Id_huesped = Convert.ToInt32(leerDatos["Id_huesped"]),
                        Ci = Convert.ToString(leerDatos["Ci"]),
                        Nombres = Convert.ToString(leerDatos["nombres"]),
                        Apellidos = Convert.ToString(leerDatos["apellidos"]),
                        Correo = Convert.ToString(leerDatos["correo"]),
                        Telefono = Convert.ToString(leerDatos["telefono"]),
                        Contrasena = Convert.ToString(leerDatos["contrasena"])
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

        public void ActualizarHuesped(clsHuesped DatosI)
        {
            try
            {
                objConectar.Abrir();
                string query = "UPDATE Huesped SET Ci=@ci, nombres=@nom, apellidos=@ape, correo=@correo, telefono=@tel, contrasena=@pass WHERE Id_huesped=@id";
                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@id", DatosI.Id_huesped);
                comandoSql.Parameters.AddWithValue("@ci", DatosI.Ci);
                comandoSql.Parameters.AddWithValue("@nom", DatosI.Nombres);
                comandoSql.Parameters.AddWithValue("@ape", DatosI.Apellidos);
                comandoSql.Parameters.AddWithValue("@correo", DatosI.Correo);
                comandoSql.Parameters.AddWithValue("@tel", DatosI.Telefono);
                comandoSql.Parameters.AddWithValue("@pass", DatosI.Contrasena);
                comandoSql.ExecuteNonQuery();
            }
            finally
            {
                objConectar.Cerrar();
            }
        }

        public clsHuesped EliminarHuesped(int idEliminar)
        {
            clsHuesped huespedEliminado = BuscarHuesped(idEliminar);

            if (huespedEliminado != null)
            {
                try
                {
                    objConectar.Abrir();
                    SqlCommand comandoSql = new SqlCommand("DELETE FROM Huesped WHERE Id_huesped = @id", objConectar.conectar);
                    comandoSql.Parameters.AddWithValue("@id", idEliminar);
                    comandoSql.ExecuteNonQuery();
                }
                finally
                {
                    objConectar.Cerrar();
                }
            }
            return huespedEliminado;
        }

        public bool ExisteCedulaDuplicada(string ci, int idHuesped = 0)
        {
            try
            {
                objConectar.Abrir();
                string query = "SELECT COUNT(*) FROM Huesped WHERE Ci = @ci AND Id_huesped != @id";
                SqlCommand comandoSql = new SqlCommand(query, objConectar.conectar);
                comandoSql.Parameters.AddWithValue("@ci", ci);
                comandoSql.Parameters.AddWithValue("@id", idHuesped);
                int count = Convert.ToInt32(comandoSql.ExecuteScalar());
                return count > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                objConectar.Cerrar();
            }
        }
    }
}