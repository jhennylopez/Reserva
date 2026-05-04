using System;
using System.Data;
using System.Windows.Forms;
using Capa_Logica;

namespace _4.Capa_Presentacion
{
    public partial class frmCancelarReserva : Form
    {
        public frmCancelarReserva()
        {
            InitializeComponent();
        }

        private void frmCancelarReserva_Load(object sender, EventArgs e)
        {
            // Limitamos los caracteres del textbox de ID
            textBox1.MaxLength = 10;

            // Cargamos las reservas en el DataGridView al iniciar
            CargarGridReservas();
        }

        private void CargarGridReservas()
        {
            try
            {
                clsPuenteReserva pRes = new clsPuenteReserva();
                DataTable dt = pRes.ConsultarTodasLasReservas();

                // Asignamos los datos al DataGridView
                dataGridView1.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    button2.Enabled = false; // Desactivar botón eliminar si no hay datos
                }
                else
                {
                    button2.Enabled = true;

                    // Formato profesional para la tabla
                    if (dataGridView1.Columns.Count > 0)
                    {
                        if (dataGridView1.Columns.Contains("Id_reserva"))
                        {
                            dataGridView1.Columns["Id_reserva"].HeaderText = "ID";
                            dataGridView1.Columns["Id_reserva"].Width = 50;
                        }

                        if (dataGridView1.Columns.Contains("Huésped"))
                            dataGridView1.Columns["Huésped"].HeaderText = "Huésped";

                        if (dataGridView1.Columns.Contains("Alojamiento"))
                            dataGridView1.Columns["Alojamiento"].HeaderText = "Alojamiento";

                        if (dataGridView1.Columns.Contains("fecha_ingreso"))
                            dataGridView1.Columns["fecha_ingreso"].HeaderText = "Fecha Ingreso";

                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        dataGridView1.ReadOnly = true;
                        dataGridView1.AllowUserToAddRows = false;
                        dataGridView1.MultiSelect = false;
                        dataGridView1.RowHeadersVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de reservas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EjecutarCancelacion()
        {
            // 1. Controlar que el campo no esté vacío
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor, ingrese o seleccione el ID de la reservación a cancelar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            try
            {
                int idAEliminar = int.Parse(textBox1.Text);

                // 2. Controlar que no se ingrese el 0 ni números negativos
                if (idAEliminar <= 0)
                {
                    MessageBox.Show("El ID de la reserva debe ser un número mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.SelectAll();
                    textBox1.Focus();
                    return;
                }

                clsPuenteReserva pRes = new clsPuenteReserva();

                // 3. Controlar que el ID ingresado realmente exista en la base de datos
                DataTable reservaExiste = pRes.BuscarReservaPorId(idAEliminar);

                if (reservaExiste == null || reservaExiste.Rows.Count == 0)
                {
                    MessageBox.Show($"No se encontró ninguna reserva activa con el ID {idAEliminar}.", "Registro No Encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.SelectAll();
                    textBox1.Focus();
                    return;
                }

                // 4. Si pasa todas las validaciones, pedimos confirmación de seguridad 
                DialogResult confirm = MessageBox.Show($"¿Está seguro que desea cancelar la reservación número {idAEliminar}?",
                    "Confirmar Cancelación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    pRes.EliminarReserva(idAEliminar);

                    MessageBox.Show("La reservación ha sido cancelada exitosamente.", "Proceso Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox1.Clear();
                    // Refrescamos el grid para quitar la reserva eliminada visualmente
                    CargarGridReservas();
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cancelar. Verifique que el ID sea correcto. Detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EjecutarCancelacion();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Está seguro que desea salir de esta ventana?", "RommyEc", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["Id_reserva"].Value.ToString();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 1. Permitir solo números y la tecla de borrar (tu código actual)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // 2. AQUÍ BLOQUEAS EL CERO (Si el textbox está vacío y presionan '0', lo ignora)
            if (e.KeyChar == '0' && textBox1.Text.Length == 0)
            {
                e.Handled = true;
            }

            // 3. Ejecutar con Enter (tu código actual)
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                EjecutarCancelacion();
            }
        }
    }
}