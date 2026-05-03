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

namespace _4.Capa_Presentacion
{
    public partial class frmBuscarHuesped : Form
    {
        public frmBuscarHuesped()
        {
            InitializeComponent();
            textBox1.MaxLength = 10;
        }

        private void ProcesarBusqueda()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor, ingrese un número de cédula para realizar la búsqueda.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            try
            {
                clsPuenteHuesped objPuente = new clsPuenteHuesped();

                // Extraemos todos los registros y buscamos la coincidencia exacta con la cédula
                List<clsHuesped> listaHuespedes = objPuente.ObtenerHuespedes();
                clsHuesped huespedEncontrado = listaHuespedes.FirstOrDefault(h => h.Ci == textBox1.Text);

                if (huespedEncontrado != null)
                {
                    // Mostramos los datos consolidados en un MessageBox informativo
                    string detallesHuesped = $"¡Huésped encontrado exitosamente!\n\n" +
                                             $"Cédula: {huespedEncontrado.Ci}\n" +
                                             $"Nombres: {huespedEncontrado.Nombres}\n" +
                                             $"Apellidos: {huespedEncontrado.Apellidos}\n" +
                                             $"Correo Electrónico: {huespedEncontrado.Correo}\n" +
                                             $"Teléfono: {huespedEncontrado.Telefono}";

                    MessageBox.Show(detallesHuesped, "Resultado de Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    
                    textBox1.SelectAll();
                    textBox1.Focus();
                }
                else
                {
                    MessageBox.Show("No se encontró ningún huésped registrado con ese número de cédula en el sistema.", "Búsqueda sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.SelectAll();
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al buscar el huésped: {ex.Message}", "Error Interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Clear();
                textBox1.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProcesarBusqueda();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validación para permitir solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // Al presionar Enter, se activa directamente la función de búsqueda
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarBusqueda();
            }
        }
        private void frmBuscarHuesped_Load(object sender, EventArgs e)
        {
        }
    }
}
