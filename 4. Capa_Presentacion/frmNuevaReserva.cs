using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Capa_Entidades;
using Capa_Logica;

namespace _4.Capa_Presentacion
{
    public partial class frmNuevaReserva : Form
    {
        // Constructor
        public frmNuevaReserva()
        {
            InitializeComponent();
        }

        private void frmNuevaReserva_Load(object sender, EventArgs e)
        {
            ConfigurarControles();
            CargarDatosIniciales();
        }

        private void ConfigurarControles()
        {
            // 1. Fechas: No permitir reservar en el pasado
            dateTimePicker1.MinDate = DateTime.Today;
            dateTimePicker2.MinDate = DateTime.Today.AddDays(1);

            // 2. Límites de texto
            textBox2.MaxLength = 10; // Cédula
            textBox1.MaxLength = 2;  // Máximo 99 personas (ajustable)

            // 3. Tipos de Reserva (Limpiamos para evitar repetidos)
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new string[] { "Completa", "Compartida", "Privada" });
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            // 4. DataGridView Profesional
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;

            // 5. El ID Alojamiento (textBox3) debe estar habilitado pero limpio
            textBox3.Clear();
        }

        private void CargarDatosIniciales()
        {
            try
            {
                clsPuenteReserva pRes = new clsPuenteReserva();
                // Llamamos con ID 0 para traer TODO (gracias al cambio en la Capa de Datos)
                DataTable dt = pRes.BuscarReservaPorId(0);

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Transformación con LINQ para que el Grid se vea perfecto
                    dataGridView1.DataSource = dt.AsEnumerable().Select(a => new {
                        ID_Reserva = a.Field<int>("Id_reserva"),
                        Entrada = a.Field<DateTime>("fecha_ingreso").ToShortDateString(),
                        Salida = a.Field<DateTime>("fecha_salida").ToShortDateString(),
                        Huéspedes = a.Field<int>("numero_personas"),
                        Tipo = a.Field<string>("tipo"),
                        Cédula = a.Field<string>("ci"),
                        Alojamiento = a.Field<int>("Id_alojamiento")
                    }).ToList();

                    dataGridView1.ClearSelection();
                }
                else
                {
                    dataGridView1.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial de reservas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            // CORRECCIÓN 3: Verificar vacíos primero
            if (string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox1.Text) ||
                comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // CORRECCIÓN 4: Validar que sean números ANTES del Parse
            if (!int.TryParse(textBox3.Text, out int idAloj) || idAloj <= 0)
            {
                MessageBox.Show("El ID del alojamiento debe ser un número válido mayor a cero.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return false;
            }

            if (!int.TryParse(textBox1.Text, out int numPersonas) || numPersonas <= 0)
            {
                MessageBox.Show("El número de huéspedes debe ser un número válido mayor a cero.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            // CORRECCIÓN 5: Validar cédula con longitud mínima
            if (textBox2.Text.Trim().Length < 10)
            {
                MessageBox.Show("La cédula debe tener al menos 10 dígitos.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            // Validar fechas
            if (dateTimePicker2.Value.Date <= dateTimePicker1.Value.Date)
            {
                MessageBox.Show("La fecha de salida debe ser posterior a la de entrada.",
                    "Error de Fechas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ProcesarReserva()
        {
            // CORRECCIÓN 6: ValidarCampos() primero, Parse después (nunca al revés)
            if (!ValidarCampos()) return;

            try
            {
                // Ahora el TryParse es seguro porque ValidarCampos() ya lo verificó
                int idAlojamiento = int.Parse(textBox3.Text);
                string cedula = textBox2.Text.Trim();
                int numPersonas = int.Parse(textBox1.Text);

                clsPuenteHuesped puenteH = new clsPuenteHuesped();
                var huesped = puenteH.ObtenerHuespedes()
                                     .FirstOrDefault(h => h.Ci == cedula);

                if (huesped == null)
                {
                    MessageBox.Show($"No se encontró ningún huésped con la cédula: {cedula}",
                        "Huésped no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    textBox2.Focus();
                    return;
                }

                // Verificar disponibilidad
                clsPuenteReserva pRes = new clsPuenteReserva();
                DataTable dtExistentes = pRes.BuscarReservaPorId(0);

                bool ocupado = dtExistentes.AsEnumerable().Any(r =>
                    r.Field<int>("Id_alojamiento") == idAlojamiento &&
                    dateTimePicker1.Value.Date < r.Field<DateTime>("fecha_salida").Date &&
                    dateTimePicker2.Value.Date > r.Field<DateTime>("fecha_ingreso").Date);

                if (ocupado)
                {
                    MessageBox.Show("El alojamiento ya está reservado en ese rango de fechas.",
                        "No disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                clsReserva nuevaRes = new clsReserva
                {
                    Fecha_ingreso = dateTimePicker1.Value.Date,
                    Fecha_salida = dateTimePicker2.Value.Date,
                    Numero_personas = numPersonas,
                    Tipo = comboBox2.SelectedItem.ToString(),
                    Id_huesped = huesped.Id_huesped,
                    Id_alojamiento = idAlojamiento
                };

                pRes.IngresarReserva(nuevaRes);

                MessageBox.Show("✔ Reserva registrada con éxito.", "Sistema",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormulario();
                CargarDatosIniciales();
            }
            catch (ArgumentException ex)
            {
                // CORRECCIÓN 7: Separar errores de negocio de errores técnicos
                MessageBox.Show("Error de validación: " + ex.Message, "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message, "Error Crítico",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox2.SelectedIndex = -1;
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today.AddDays(1);
            textBox2.Focus(); // Ponemos el foco en la cédula para la siguiente reserva
        }

        // --- EVENTOS DE CONTROLES ---

        private void button2_Click(object sender, EventArgs e) // Botón "Reservar"
        {
            ProcesarReserva();
        }

        private void button1_Click(object sender, EventArgs e) // Botón "Cerrar"
        {
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Control para que al tocar la tabla se cargue el ID del alojamiento
            if (e.RowIndex >= 0)
            {
                textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells["Alojamiento"].Value.ToString();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }
    }
}