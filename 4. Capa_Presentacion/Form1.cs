using _1.Capa_Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4.Capa_Presentacion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Convertimos este Formulario en un Contenedor Padre de otros Formularios
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized; 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // El Login ya llenó los datos de clsSesion, así que Form1 los usa aquí
            this.Text = $"RommyEc - Bienvenido {clsSesion.Nombre} ({clsSesion.Rol})";
            ConfigurarPermisos();
           
        }
        private void ConfigurarPermisos()
        {
            if (clsSesion.Rol == "Huesped")
            {
                // El huésped NO puede gestionar catálogos, solo reservar
                huéspedToolStripMenuItem.Enabled = false;
                alojamientoToolStripMenuItem.Enabled = false;

                // Dentro de Reservas, quizás solo habilitar "Nueva Reserva"
                consultarReservaToolStripMenuItem.Enabled = true;
                actualizarReservaToolStripMenuItem.Enabled = false;
                cancelarReservaToolStripMenuItem.Enabled = false;

                MessageBox.Show("Modo Huésped: Acceso limitado a reservaciones.", "RommyEc");
            }
            else
            {
                // Administrador tiene acceso total (por defecto todos están en true)
                MessageBox.Show("Modo Administrador: Acceso total habilitado.", "RommyEc");
            }
        }
        private void AbrirFormularioHijo(Form formularioHijo)
        {
            Form frmAbierto = null;

            // Revisamos si el formulario que queremos abrir ya está abierto en el sistema
            foreach (Form frm in this.MdiChildren)
            {
                if (frm.GetType() == formularioHijo.GetType())
                {
                    frmAbierto = frm;
                    break;
                }
            }

            // Si ya está abierto, lo traemos al frente. Si no, lo mostramos.
            if (frmAbierto != null)
            {
                frmAbierto.BringToFront();
                frmAbierto.WindowState = FormWindowState.Normal;
            }
            else
            {
                formularioHijo.MdiParent = this;

                //formularioHijo.WindowState = FormWindowState.Maximized;

                formularioHijo.Show();
            }
        }

        private void registrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRegistrarHuesped frmRegistrar = new frmRegistrarHuesped();
            AbrirFormularioHijo(frmRegistrar);
        }

        private void buscarHuéspedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBuscarHuesped frmBuscar = new frmBuscarHuesped();
            AbrirFormularioHijo(frmBuscar);
        }

        private void actualizarHuéspedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmActualizarHuesped frmActualizar = new frmActualizarHuesped();
            AbrirFormularioHijo(frmActualizar);
        }

        private void eliminarHuéspedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEliminarHuesped frmEliminar = new frmEliminarHuesped();
            AbrirFormularioHijo(frmEliminar);
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // --- MÓDULO DE RESERVAS ---

        private void nuevaReservaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNuevaReserva frm = new frmNuevaReserva();
            AbrirFormularioHijo(frm);
        }

        private void consultarReservaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmConsultarReserva frm = new frmConsultarReserva();
            AbrirFormularioHijo(frm);
        }

        private void actualizarReservaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmActualizarReserva frm = new frmActualizarReserva();
            AbrirFormularioHijo(frm);
        }

        private void cancelarReservaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCancelarReserva frm = new frmCancelarReserva();
            AbrirFormularioHijo(frm);
        }

        // --- MÓDULO DE ALOJAMIENTO 

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmRegistrarAlojamiento frm = new frmRegistrarAlojamiento();
            AbrirFormularioHijo(frm);
        }


        private void cerrarSesiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("¿Está seguro que desea cerrar la sesión actual?",
                    "RommyEc | Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                // 1. Cerramos ordenadamente las sub-pantallas que estén abiertas
                foreach (Form frm in this.MdiChildren)
                {
                    frm.Close();
                }

                // 2. Limpiamos las credenciales de la memoria por seguridad
                clsSesion.Nombre = "";
                clsSesion.Rol = "";

                // 3. Reiniciamos la aplicación entera. 
                // Esto apagará Form1 y volverá a ejecutar Program.cs (abriendo frmLogin)
                Application.Restart();
            }
        }
        //Ayuda y Acerca de
        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string rutaAyuda = System.IO.Path.Combine(Application.StartupPath, "ayuda.html");

            try
            {
                if (System.IO.File.Exists(rutaAyuda))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = rutaAyuda,
                        UseShellExecute = true // Necesario para abrir el navegador
                    });
                }
                else
                {
                    MessageBox.Show("No se encontró el manual de usuario en: " + rutaAyuda,
                                    "Archivo Faltante", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ayuda: " + ex.Message);
            }
        }
        private void acercaDeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
        }
        private void registrarAlojamientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRegistrarAlojamiento frmRegistrar = new frmRegistrarAlojamiento();
            AbrirFormularioHijo(frmRegistrar);
        }

        private void buscarAlojamientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmBuscarAlojamiento frmBuscar = new frmBuscarAlojamiento();
            AbrirFormularioHijo(frmBuscar);
        }

        private void actualizarAlojamientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmActualizarAlojamiento frmActualizar = new frmActualizarAlojamiento();
            AbrirFormularioHijo(frmActualizar);
        }

        private void eliminarAlojamientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEliminarAlojamiento frmEliminar = new frmEliminarAlojamiento();
            AbrirFormularioHijo(frmEliminar);
        }

        private void huéspedToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ayudaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Para evitar problemas de rutas relativas, se puede usar una ruta absoluta o colocar el archivo en el mismo directorio que el ejecutable.
            string rutaAyuda = "D:\\Reserva\\ayuda.html";

            try
            {
                if (System.IO.File.Exists(rutaAyuda))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = rutaAyuda,
                        UseShellExecute = true // Necesario para abrir el navegador
                    });
                }
                else
                {
                    MessageBox.Show("No se encontró el manual de usuario en: " + rutaAyuda, "Archivo Faltante", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la ayuda: " + ex.Message);
            }
        }

        private void acercaDeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            string mensaje = "ESCUELA SUPERIOR POLITÉCNICA DE CHIMBORAZO\n" +
            "Facultad de Informática y Electrónica\n" +
            "Asignatura: Aplicaciones Informáticas I\n\n" +
            "PROYECTO: SISTEMA DE RESERVAS MULTICAPA\n" +
            "INTEGRANTES:\n\n" +
            "• ABAD ROMERO ANTHONY PAUL\n" +
            "• CRUZ CHALAN LUIS ALEJANDRO \n" +
            "• LOPEZ ZAMBRANO JHENNY ELIZABETH\n" +
            "• CRISTOFFER ALEXANDER NUÑEZ CAISAPANTA\n" +
            "• ROMERO GAVINO DOMENICA VIVIANA\n" +
            "• RONQUILLO ARMAS ALVIN MARTIN \n" +
            "• SALAZAR TIXE LUIS FERNANDO\n\n" +

            "Riobamba, Ecuador - Mayo 2026";

            MessageBox.Show(mensaje, "RommyEc",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}