using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Entidades;
using Capa_Logica;

namespace _4.Capa_Presentacion
{
    public partial class frmEliminarAlojamiento : Form
    {
        public frmEliminarAlojamiento()
        {
            InitializeComponent();
        }

        // Método para cargar todos los alojamientos en el ListBox para referencia visual
        private void CargarListaAlojamientos()
        {
            try
            {
                listBox1.Items.Clear();
                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();

                // Usamos el método que creamos anteriormente (pasando 0 para traer todos)
                List<clsAlojamiento> lista = objPuente.BuscarAlojamientosPorHuespedes(0);

                if (lista.Count == 0)
                {
                    listBox1.Items.Add("No hay alojamientos registrados en el sistema.");
                }
                else
                {
                    foreach (var aloj in lista)
                    {
                        listBox1.Items.Add($"ID: {aloj.Id_alojamiento} | {aloj.Descripcion} - {aloj.Ubicacion}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmEliminarAlojamiento_Load(object sender, EventArgs e)
        {
            CargarListaAlojamientos();
        }

        private void ProcesarEliminacion()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor, ingrese el ID del alojamiento que desea eliminar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            try
            {
                int idEliminar = int.Parse(textBox1.Text);
                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();

                // Buscamos si el ID existe realmente
                clsAlojamiento alojamientoEncontrado = objPuente.BuscarAlojamiento(idEliminar);

                if (alojamientoEncontrado != null)
                {
                    // Confirmación de seguridad
                    DialogResult respuesta = MessageBox.Show(
                        $"¿Está seguro que desea eliminar este alojamiento?\n\n" +
                        $"Descripción: {alojamientoEncontrado.Descripcion}\n" +
                        $"Ubicación: {alojamientoEncontrado.Ubicacion}",
                        "Confirmar Eliminación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (respuesta == DialogResult.Yes)
                    {
                        objPuente.EliminarAlojamiento(idEliminar);

                        MessageBox.Show("Alojamiento eliminado correctamente del sistema.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        textBox1.Clear();
                        CargarListaAlojamientos(); // Refrescamos el ListBox
                        textBox1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró ningún alojamiento con ese ID.", "Error de Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.SelectAll();
                    textBox1.Focus();
                }
            }
            catch (SqlException ex)
            {
                // Capturamos el error si el alojamiento tiene reservas activas (Violación de Llave Foránea)
                if (ex.Number == 547)
                {
                    MessageBox.Show("No se puede eliminar este alojamiento porque tiene reservas asociadas en el sistema. Debe cancelar las reservas primero.", "Acción Denegada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    MessageBox.Show("Error de base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcesarEliminacion();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarEliminacion();
            }
        }
    }
}