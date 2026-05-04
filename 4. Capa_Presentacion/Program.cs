using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4.Capa_Presentacion
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Instanciamos el Login
            frmLogin login = new frmLogin();

            // 2. Lo mostramos. El programa se "pausa" aquí hasta que el Login se cierre.
            if (login.ShowDialog() == DialogResult.OK)
            {
                // 3. Si el login responde con un "OK", arrancamos oficialmente la aplicación con Form1
                Application.Run(new Form1());
            }
            else
            {
                // 4. Si el usuario cerró el login en la "X", apagamos todo.
                Application.Exit();
            }
        }
    }
}
