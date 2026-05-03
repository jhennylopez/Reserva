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
    public partial class frmBuscarAlojamiento : Form
    {
        public frmBuscarAlojamiento()
        {
            InitializeComponent();
        }

        // Método central para cargar y darle formato a la tabla
        private void CargarDatos(int cantidadHuespedes = 0)
        {
            try
            {
                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();
                List<clsAlojamiento> lista = objPuente.BuscarAlojamientosPorHuespedes(cantidadHuespedes);

                // Asignamos la lista a la tabla
                dataGridView1.DataSource = lista;

                // Formateamos las columnas para que se vean presentables al usuario
                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns["Id_alojamiento"].HeaderText = "ID";
                    dataGridView1.Columns["Id_alojamiento"].Width = 40;

                    dataGridView1.Columns["Descripcion"].HeaderText = "Descripción";
                    dataGridView1.Columns["Descripcion"].Width = 200;

                    dataGridView1.Columns["Ubicacion"].HeaderText = "Ubicación";
                    dataGridView1.Columns["Ubicacion"].Width = 150;

                    dataGridView1.Columns["Max_huespedes"].HeaderText = "Capacidad Máx.";
                    dataGridView1.Columns["Max_huespedes"].Width = 110;

                    dataGridView1.Columns["Num_habitaciones"].HeaderText = "Habitaciones";
                    dataGridView1.Columns["Num_habitaciones"].Width = 90;

                    dataGridView1.Columns["Num_banos"].HeaderText = "Baños";
                    dataGridView1.Columns["Num_banos"].Width = 60;

                    dataGridView1.Columns["Id_administrador"].Visible = false;
                }

                // Avisamos si no hay lugares suficientemente grandes
                if (lista.Count == 0 && cantidadHuespedes > 0)
                {
                    MessageBox.Show($"No contamos con alojamientos que tengan capacidad para {cantidadHuespedes} o más huéspedes en este momento.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los alojamientos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmBuscarAlojamiento_Load(object sender, EventArgs e)
        {
            // Al abrir, cargamos todos los alojamientos disponibles (0 = mostrar todos)
            CargarDatos(0);
        }

        private void EjecutarBusqueda()
        {
            try
            {
                // 1. Validamos si hay algo que buscar (si está vacío, enviamos 0 para traer todo)
                int capacidadRequerida = 0;
                if (!string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    if (!int.TryParse(textBox1.Text, out capacidadRequerida))
                    {
                        MessageBox.Show("Por favor, ingrese un número válido.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // 2. Llamamos a la lógica (Capa Lógica)
                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();
                List<clsAlojamiento> resultados = objPuente.BuscarAlojamientosPorHuespedes(capacidadRequerida);

                // 3. Refrescamos el DataGridView
                dataGridView1.DataSource = null; // Limpiamos primero
                dataGridView1.DataSource = resultados;

                // 4. Feedback visual si no hay resultados
                if (resultados.Count == 0)
                {
                    MessageBox.Show("No se encontraron alojamientos con esa capacidad.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EjecutarBusqueda();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validamos que SOLO se puedan ingresar números y la tecla de borrar
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                EjecutarBusqueda();
            }
        }
    }
}