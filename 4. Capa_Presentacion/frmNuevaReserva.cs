using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Entidades;
using Capa_Logica;

namespace _4.Capa_Presentacion
{
    public partial class frmNuevaReserva : Form
    {
        public frmNuevaReserva()
        {
            InitializeComponent();
        }

        private void frmNuevaReserva_Load(object sender, EventArgs e)
        {
            // 1. Configuramos las fechas (no se puede reservar en el pasado)
            dateTimePicker1.MinDate = DateTime.Today;
            dateTimePicker2.MinDate = DateTime.Today.AddDays(1);

            // 2. Limitamos el tamaño de la cédula
            textBox2.MaxLength = 10;

            // 3. Cargamos los tipos de reserva en el ComboBox2
            comboBox2.Items.Clear();
            comboBox2.Items.Add("Completa");
            comboBox2.Items.Add("Compartida");
            comboBox2.Items.Add("Privada");
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList; 

            // 4. Cargamos los alojamientos disponibles
            CargarAlojamientos();
        }

        private void CargarAlojamientos()
        {
            try
            {
                clsPuenteAlojamiento objPuenteAloj = new clsPuenteAlojamiento();
                List<clsAlojamiento> lista = objPuenteAloj.BuscarAlojamientosPorHuespedes(0);

                listBox1.Items.Clear();
                comboBox1.Items.Clear();
                comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

                foreach (var aloj in lista)
                {
                    listBox1.Items.Add($"ID: {aloj.Id_alojamiento} | Capacidad: {aloj.Max_huespedes} pers. | {aloj.Descripcion} - {aloj.Ubicacion}");
                    comboBox1.Items.Add(aloj.Id_alojamiento);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los alojamientos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, seleccione un ID de alojamiento.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, seleccione el Tipo de reserva.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Por favor, ingrese el número de huéspedes y la cédula.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dateTimePicker2.Value <= dateTimePicker1.Value)
            {
                MessageBox.Show("La Fecha de Salida debe ser posterior a la Fecha de Entrada.", "Error de Fechas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ProcesarReserva()
        {
            if (!ValidarCampos()) return;

            try
            {
                int idAlojamiento = int.Parse(comboBox1.SelectedItem.ToString());
                string tipoReserva = comboBox2.SelectedItem.ToString();
                int numeroPersonas = int.Parse(textBox1.Text);
                string cedula = textBox2.Text;
                DateTime fechaEntrada = dateTimePicker1.Value;
                DateTime fechaSalida = dateTimePicker2.Value;

                if (numeroPersonas <= 0)
                {
                    MessageBox.Show("El número de huéspedes debe ser mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 1. Verificar si el huésped existe
                clsPuenteHuesped puenteHuesped = new clsPuenteHuesped();
                List<clsHuesped> listaH = puenteHuesped.ObtenerHuespedes();
                clsHuesped huesped = listaH.FirstOrDefault(h => h.Ci == cedula);

                if (huesped == null)
                {
                    MessageBox.Show("El huésped con esta cédula no existe en el sistema. Regístrelo primero.", "Huésped no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // 2. Verificar capacidad del alojamiento 
                clsPuenteAlojamiento puenteAloj = new clsPuenteAlojamiento();
                List<clsAlojamiento> listaA = puenteAloj.BuscarAlojamientosPorHuespedes(0);
                clsAlojamiento aloj = listaA.FirstOrDefault(a => a.Id_alojamiento == idAlojamiento);

                if (aloj != null && numeroPersonas > aloj.Max_huespedes)
                {
                    MessageBox.Show($"No se puede proceder. El alojamiento ID {idAlojamiento} tiene una capacidad máxima de {aloj.Max_huespedes} personas.", "Capacidad Excedida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 3. Crear la entidad de la reserva y guardar
                clsReserva nuevaReserva = new clsReserva();
                nuevaReserva.Fecha_ingreso = fechaEntrada;
                nuevaReserva.Fecha_salida = fechaSalida;
                nuevaReserva.Numero_personas = numeroPersonas;
                nuevaReserva.Tipo = tipoReserva;
                nuevaReserva.Id_huesped = huesped.Id_huesped;
                nuevaReserva.Id_alojamiento = idAlojamiento;

                clsPuenteReserva puenteReserva = new clsPuenteReserva();
                puenteReserva.IngresarReserva(nuevaReserva);

                MessageBox.Show($"¡Reserva guardada con éxito!\n\nHuésped: {huesped.Nombres} {huesped.Apellidos}\nAlojamiento asignado: ID {idAlojamiento}", "Reserva Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = -1;
                textBox1.Clear();
                textBox2.Clear();
                dateTimePicker1.Value = DateTime.Today;
                dateTimePicker2.Value = DateTime.Today.AddDays(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al guardar la reserva: " + ex.Message, "Error Interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); // Botón Cancelar
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcesarReserva(); // Botón Reservar
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Número de huéspedes: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cédula: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }
    }
}