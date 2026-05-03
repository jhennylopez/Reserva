using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Capa_Datos
{
    public class clsConexion
    {
        string cadena = "Data Source=DESKTOP-PRK9TKH\\SQLEXPRESS; Initial Catalog=SistemaReservasBD; Integrated Security=True; TrustServerCertificate=True;";
        public SqlConnection conectar = new SqlConnection();

        public clsConexion()
        {
            conectar.ConnectionString = cadena;
        }

        public void Abrir()
        {
            try
            {
                conectar.Open();
            }
            catch (SqlException ex)
            {
                System.Console.Write("Error al abrir la conexión: " + ex.Message);
            }
        }

        public void Cerrar()
        {
            try
            {
                conectar.Close();
            }
            catch (SqlException ex)
            {
                System.Console.Write("Error al cerrar la conexión: " + ex.Message);
            }
        }
    }
}