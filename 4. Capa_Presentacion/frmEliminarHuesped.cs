using Capa_Entidades;
using Capa_Logica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _4.Capa_Presentacion
{
    public partial class frmEliminarHuesped : Form
    {
        public frmEliminarHuesped()
        {
            InitializeComponent();
        }
        private void frmEliminarHuesped_Load(object sender, EventArgs e)
        {
            // Limitamos a 10 caracteres exactos de la cédula ecuatoriana
            textBox1.MaxLength = 10;
        }

        private void ProcesarEliminacion()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor, ingrese un número de cédula para eliminar.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            try
            {
                clsPuenteHuesped objPuente = new clsPuenteHuesped();

                // Obtenemos todos los huéspedes y buscamos el que coincida con la cédula ingresada
                List<clsHuesped> listaHuespedes = objPuente.ObtenerHuespedes();
                clsHuesped huespedEncontrado = listaHuespedes.FirstOrDefault(h => h.Ci == textBox1.Text);

                if (huespedEncontrado != null)
                {
                    //Confirmación de seguridad antes de borrar
                    DialogResult respuesta = MessageBox.Show(
                        $"¿Está seguro que desea eliminar al huésped: {huespedEncontrado.Nombres} {huespedEncontrado.Apellidos}?\n\nEsta acción eliminará su registro de la base de datos.",
                        "Confirmar Eliminación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    // Si el usuario confirma que Sí quiere borrar
                    if (respuesta == DialogResult.Yes)
                    {
                        // Le pasamos el ID interno que encontramos a nuestro método original
                        objPuente.EliminarHuesped(huespedEncontrado.Id_huesped);

                        MessageBox.Show($"Huésped eliminado correctamente:\n\n" +
                                        $"Cédula: {huespedEncontrado.Ci}\n" +
                                        $"Nombre: {huespedEncontrado.Nombres} {huespedEncontrado.Apellidos}",
                                        "Eliminación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        textBox1.Clear();
                        textBox1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró ningún huésped registrado con ese número de cédula.", "Error de Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.SelectAll();
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al eliminar el huésped: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Clear();
                textBox1.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProcesarEliminacion();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // Al presionar Enter, ejecutar la eliminación
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarEliminacion();
            }
        }
    }
}


