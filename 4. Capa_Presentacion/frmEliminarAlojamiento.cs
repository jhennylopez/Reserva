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

        private void frmEliminarAlojamiento_Load(object sender, EventArgs e)
        {
            // Cargamos la lista de alojamientos en el DataGridView al abrir el formulario
            CargarGridAlojamientos();
        }

        // --- NUEVO MÉTODO PARA CARGAR EL DATAGRIDVIEW ---
        private void CargarGridAlojamientos()
        {
            try
            {
                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();
                // Traemos todos los alojamientos (pasando 0)
                List<clsAlojamiento> lista = objPuente.BuscarAlojamientosPorHuespedes(0);

                dataGridView1.DataSource = lista;

                // Formato profesional para la tabla
                if (dataGridView1.Columns.Count > 0)
                {
                    // Ocultamos la llave foránea del administrador por ser dato interno
                    if (dataGridView1.Columns.Contains("Id_administrador"))
                        dataGridView1.Columns["Id_administrador"].Visible = false;

                    // Cambiamos los nombres de las cabeceras para el usuario
                    if (dataGridView1.Columns.Contains("Id_alojamiento"))
                    {
                        dataGridView1.Columns["Id_alojamiento"].HeaderText = "ID";
                        dataGridView1.Columns["Id_alojamiento"].Width = 40;
                    }
                    if (dataGridView1.Columns.Contains("Descripcion"))
                        dataGridView1.Columns["Descripcion"].HeaderText = "Descripción";
                    if (dataGridView1.Columns.Contains("Ubicacion"))
                        dataGridView1.Columns["Ubicacion"].HeaderText = "Ubicación";
                    if (dataGridView1.Columns.Contains("Max_huespedes"))
                        dataGridView1.Columns["Max_huespedes"].HeaderText = "Cap. Máx.";
                    if (dataGridView1.Columns.Contains("Num_habitaciones"))
                        dataGridView1.Columns["Num_habitaciones"].HeaderText = "Habitaciones";
                    if (dataGridView1.Columns.Contains("Num_banos"))
                        dataGridView1.Columns["Num_banos"].HeaderText = "Baños";

                    // Ajustes de comportamiento del Grid
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.MultiSelect = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de alojamientos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcesarEliminacion()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor, ingrese o seleccione el ID del alojamiento que desea eliminar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        // --- ACTUALIZAMOS EL GRID DESPUÉS DE ELIMINAR ---
                        CargarGridAlojamientos();
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
                // Excelente práctica: Capturamos el error si el alojamiento tiene reservas activas (Violación de Llave Foránea)
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

        private void button1_Click(object sender, EventArgs e)
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
                // Toma el ID del alojamiento de la fila seleccionada y lo pone en el TextBox
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Id_alojamiento"].Value.ToString();
            }
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