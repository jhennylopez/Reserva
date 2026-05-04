using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Capa_Entidades;
using Capa_Logica;
using Microsoft.VisualBasic;

namespace _4.Capa_Presentacion
{
    public partial class frmActualizarReserva : Form
    {
        private int idReservaActual = 0;

        public frmActualizarReserva()
        {
            InitializeComponent();
        }

        private void frmActualizarReserva_Load(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();

            ConfigurarControles();
            ConfigurarDataGridView(); // Nueva configuración visual
            CargarListasReferenciales();
            EstadoInicial(true);
        }

        private void ConfigurarControles()
        {
            comboBox2.Items.AddRange(new string[] { "Completa", "Compartida", "Privada" });
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox2.ReadOnly = true;
        }

        // Configuración visual del DataGridView para que se vea bien
        private void ConfigurarDataGridView()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // 1. Evita que el usuario escriba en las celdas
            dataGridView1.ReadOnly = true;

            // 2. Al hacer clic, se selecciona toda la fila, no solo una celda
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 3. Quita la fila vacía que aparece siempre al final
            dataGridView1.AllowUserToAddRows = false;

            // 4. Hace que las columnas se estiren para ocupar todo el ancho del cuadro
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 5. Oculta la columna gris de la izquierda (selector de filas)
            dataGridView1.RowHeadersVisible = false;

            // 6. Evita que el usuario borre filas con la tecla Suprimir
            dataGridView1.AllowUserToDeleteRows = false;
        }

        private void EstadoInicial(bool bloqueado)
        {
            if (bloqueado)
            {
                textBox3.Clear();
                textBox1.Clear();
                textBox2.Clear();
                idReservaActual = 0;
            }
            textBox3.Enabled = !bloqueado;
            comboBox2.Enabled = !bloqueado;
            dateTimePicker1.Enabled = !bloqueado;
            dateTimePicker2.Enabled = !bloqueado;
            textBox1.Enabled = !bloqueado;
            button2.Enabled = !bloqueado;

            button1.Enabled = bloqueado;
        }

        private void CargarListasReferenciales()
        {
            try
            {
                clsPuenteReserva preserva = new clsPuenteReserva();

                // Asumimos que esto devuelve un DataTable
                DataTable dtReservas = preserva.BuscarReservaPorId(0);

                textBox3.Clear();

                if (dtReservas != null && dtReservas.Rows.Count > 0)
                {
                    // === LA CORRECCIÓN ESTÁ AQUÍ ===
                    // Usamos .AsEnumerable() para que LINQ pueda leer las filas del DataTable
                    dataGridView1.DataSource = dtReservas.AsEnumerable().Select(a => new {
                        ID = a.Field<int>("Id_reserva"),
                        fecha_ingreso = a.Field<DateTime>("fecha_ingreso").ToShortDateString(),
                        fecha_salida = a.Field<DateTime>("fecha_salida").ToShortDateString(),
                        num_personas = a.Field<int>("numero_personas"),
                        tipo = a.Field<string>("tipo"),
                        ID_huesped = a.Field<int>("Id_huesped"), // Ajusta el nombre de la columna si es necesario
                        Id_Alojamiento = a.Field<int>("Id_alojamiento")
                    }).ToList();

                    // Llenamos el ComboBox recorriendo las filas del DataTable original
                    foreach (DataRow fila in dtReservas.Rows)
                    {
                        textBox3.Text = fila["Id_reserva"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el ID de la Reserva:", "Actualizar Reserva");
            if (int.TryParse(input, out int id))
            {
                clsPuenteReserva pRes = new clsPuenteReserva();
                DataTable dt = pRes.BuscarReservaPorId(id);

                if (dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    idReservaActual = id;

                    // Seleccionar en el combo el ID correspondiente
                    textBox3.Text = r["Id_alojamiento"].ToString();
                    comboBox2.SelectedItem = r["tipo"].ToString();
                    dateTimePicker1.Value = Convert.ToDateTime(r["fecha_ingreso"]);
                    dateTimePicker2.Value = Convert.ToDateTime(r["fecha_salida"]);
                    textBox1.Text = r["numero_personas"].ToString();
                    textBox2.Text = r["ci"].ToString();

                    EstadoInicial(false);
                }
                else { MessageBox.Show("Reserva no encontrada."); }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dateTimePicker2.Value <= dateTimePicker1.Value)
                {
                    MessageBox.Show("La fecha de salida debe ser posterior a la de ingreso.", "RommyEc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrEmpty(textBox3.Text)) { MessageBox.Show("Seleccione un alojamiento.", "RommyEc", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                int numPer = int.Parse(textBox1.Text);
                int idAloj = int.Parse(textBox3.Text);

                clsPuenteAlojamiento pAloj = new clsPuenteAlojamiento();
                var aloj = pAloj.BuscarAlojamientosPorHuespedes(0).FirstOrDefault(a => a.Id_alojamiento == idAloj);

                if (aloj != null && numPer > aloj.Max_huespedes)
                {
                    MessageBox.Show($"Capacidad excedida. Este alojamiento solo permite {aloj.Max_huespedes} personas.", "RommyEc", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                clsReserva ent = new clsReserva
                {
                    Id_reserva = idReservaActual,
                    Fecha_ingreso = dateTimePicker1.Value,
                    Fecha_salida = dateTimePicker2.Value,
                    Numero_personas = numPer,
                    Tipo = comboBox2.SelectedItem.ToString(),
                    Id_alojamiento = idAloj
                };

                clsPuenteReserva pRes = new clsPuenteReserva();
                pRes.ActualizarReserva(ent);

                MessageBox.Show("Reserva actualizada correctamente.", "RommyEc", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarListasReferenciales();

                EstadoInicial(true);
                textBox1.Clear();
                textBox2.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !textBox3.Enabled == false) // Solo si no está bloqueado
            {
                var idSeleccionado = dataGridView1.Rows[e.RowIndex].Cells["ID"].Value;
                textBox3.Text = idSeleccionado.ToString();
            }
        }
    }
}