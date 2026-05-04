using _1.Capa_Entidades;
using Capa_Datos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.Capa_Datos
{
    public class clsOperacionLogin
    {
        clsConexion objCon = new clsConexion();

        public bool ValidarCredenciales(string correo, string pass)
        {
            try
            {
                objCon.Abrir();
                // 1. Intentar buscar como Administrador
                string sqlAdmin = "SELECT Id_administrador, nombres FROM Administrador WHERE correo=@c AND contrasena=@p";
                SqlCommand cmd = new SqlCommand(sqlAdmin, objCon.conectar);
                cmd.Parameters.AddWithValue("@c", correo);
                cmd.Parameters.AddWithValue("@p", pass);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    clsSesion.Id_usuario = (int)dr["Id_administrador"];
                    clsSesion.Nombre = dr["nombres"].ToString();
                    clsSesion.Rol = "Admin";
                    return true;
                }
                dr.Close();

                // 2. Si no es admin, intentar como Huésped
                string sqlHuesped = "SELECT Id_huesped, nombres FROM Huesped WHERE correo=@c AND contrasena=@p";
                cmd = new SqlCommand(sqlHuesped, objCon.conectar);
                cmd.Parameters.AddWithValue("@c", correo);
                cmd.Parameters.AddWithValue("@p", pass);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    clsSesion.Id_usuario = (int)dr["Id_huesped"];
                    clsSesion.Nombre = dr["nombres"].ToString();
                    clsSesion.Rol = "Huesped";
                    return true;
                }
                return false;
            }
            finally { objCon.Cerrar(); }
        }
    }
}
