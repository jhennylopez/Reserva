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

            // Configuración visual profesional para el DataGridView
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.MultiSelect = false;
        }

        // Te recomiendo llamar a este método en el evento Load de tu formulario
        // para que al abrir la ventana se vean todos los huéspedes registrados.
        private void frmBuscarHuesped_Load(object sender, EventArgs e)
        {
            CargarDatos();
        }

        // Modificamos el método para que cargue HUÉSPEDES y acepte un filtro opcional
        private void CargarDatos(string cedulaFiltro = "")
        {
            try
            {
                clsPuenteHuesped objPuente = new clsPuenteHuesped();
                List<clsHuesped> lista = objPuente.ObtenerHuespedes();

                // Si el usuario ingresó una cédula, filtramos la lista
                if (!string.IsNullOrWhiteSpace(cedulaFiltro))
                {
                    lista = lista.Where(h => h.Ci == cedulaFiltro).ToList();
                }

                // Asignamos la lista a la tabla
                dataGridView1.DataSource = lista;

                // Formateamos las columnas para Huéspedes
                if (dataGridView1.Columns.Count > 0)
                {
                    // Ocultamos el ID interno y contraseñas por seguridad (si existen)
                    if (dataGridView1.Columns.Contains("Id_huesped"))
                        dataGridView1.Columns["Id_huesped"].Visible = false;

                    if (dataGridView1.Columns.Contains("Contrasena"))
                        dataGridView1.Columns["Contrasena"].Visible = false;

                    // Cambiamos los encabezados
                    if (dataGridView1.Columns.Contains("Ci"))
                        dataGridView1.Columns["Ci"].HeaderText = "Cédula";

                    if (dataGridView1.Columns.Contains("Nombres"))
                        dataGridView1.Columns["Nombres"].HeaderText = "Nombres";

                    if (dataGridView1.Columns.Contains("Apellidos"))
                        dataGridView1.Columns["Apellidos"].HeaderText = "Apellidos";

                    if (dataGridView1.Columns.Contains("Telefono"))
                        dataGridView1.Columns["Telefono"].HeaderText = "Teléfono";

                    if (dataGridView1.Columns.Contains("Correo"))
                        dataGridView1.Columns["Correo"].HeaderText = "Correo Electrónico";

                    // Ajusta el ancho de las columnas automáticamente al espacio disponible
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los huéspedes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcesarBusqueda()
        {
            // Si el campo está vacío, cargamos todos los huéspedes nuevamente
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                CargarDatos();
                return;
            }

            try
            {
                // Llamamos a cargar datos pasándole la cédula buscada
                CargarDatos(textBox1.Text);

                // Verificamos si el DataGridView quedó vacío tras aplicar el filtro
                if (dataGridView1.Rows.Count == 0)
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
            DialogResult result = MessageBox.Show("¿Está seguro que desea cancelarla búsqueda?", "RommyEc", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
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
    }
}
