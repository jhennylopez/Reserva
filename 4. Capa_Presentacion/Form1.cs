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
            // Llamar al login antes de mostrar el menú principal
            frmLogin login = new frmLogin();
            if (login.ShowDialog() == DialogResult.OK)
            {
                this.Text = $"Sistema de Reservas - Bienvenido {clsSesion.Nombre} ({clsSesion.Rol})";
                ConfigurarPermisos();
            }
            else
            {
                Application.Exit(); // Si cancela el login, cierra todo
            }
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

                MessageBox.Show("Modo Huésped: Acceso limitado a reservaciones.", "Información");
            }
            else
            {
                // Administrador tiene acceso total (por defecto todos están en true)
                MessageBox.Show("Modo Administrador: Acceso total habilitado.", "Información");
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
                "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                foreach (Form frm in this.MdiChildren)
                {
                    frm.Close();
                }

                MessageBox.Show("Sesión cerrada correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sistema de Gestión de Reservas v1.0\n\n" +
                "Desarrollado por: GRUPO 2\n" +
                "Institución: ESPOCH\n" +
                "Año: 2026", "Acerca del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}