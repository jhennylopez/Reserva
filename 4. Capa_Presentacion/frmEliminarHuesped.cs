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

            // Cargamos la lista de huéspedes en el DataGridView al abrir el formulario
            CargarGrid();
        }

        // --- NUEVO MÉTODO PARA CARGAR EL DATAGRIDVIEW ---
        private void CargarGrid()
        {
            try
            {
                clsPuenteHuesped objPuente = new clsPuenteHuesped();
                List<clsHuesped> lista = objPuente.ObtenerHuespedes();

                dataGridView1.DataSource = lista;

                // Formato profesional para la tabla
                if (dataGridView1.Columns.Count > 0)
                {
                    // Ocultamos datos internos o sensibles
                    if (dataGridView1.Columns.Contains("Id_huesped"))
                        dataGridView1.Columns["Id_huesped"].Visible = false;
                    if (dataGridView1.Columns.Contains("Contrasena"))
                        dataGridView1.Columns["Contrasena"].Visible = false;

                    // Cambiamos los nombres de las cabeceras
                    if (dataGridView1.Columns.Contains("Ci"))
                        dataGridView1.Columns["Ci"].HeaderText = "Cédula";
                    if (dataGridView1.Columns.Contains("Nombres"))
                        dataGridView1.Columns["Nombres"].HeaderText = "Nombres";
                    if (dataGridView1.Columns.Contains("Apellidos"))
                        dataGridView1.Columns["Apellidos"].HeaderText = "Apellidos";
                    if (dataGridView1.Columns.Contains("Telefono"))
                        dataGridView1.Columns["Telefono"].HeaderText = "Teléfono";
                    if (dataGridView1.Columns.Contains("Correo"))
                        dataGridView1.Columns["Correo"].HeaderText = "Correo";

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.MultiSelect = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de huéspedes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    // Confirmación de seguridad antes de borrar
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

                        // --- ACTUALIZAMOS EL GRID DESPUÉS DE ELIMINAR ---
                        CargarGrid();
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
                MessageBox.Show($"Ocurrió un error al eliminar el huésped. Verifique que no tenga reservas activas asociadas. Detalles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            DialogResult result = MessageBox.Show("¿Está seguro que desea cancelar la operación?", "RommyEc", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        // --- NUEVO EVENTO PARA SELECCIONAR CON UN CLIC ---
        // (Asegúrate de enlazar este evento en la ventana de propiedades del DataGridView)
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Verifica que no se haya hecho clic en la cabecera
            {
                // Toma la cédula de la fila seleccionada y la pone en el TextBox
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Ci"].Value.ToString();
            }
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


