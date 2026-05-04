using Capa_Entidades;
using Capa_Logica;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
namespace _4.Capa_Presentacion
{
    public partial class frmActualizarAlojamiento : Form
    {
        
        private int idAlojamientoActual = 0;

        public frmActualizarAlojamiento()
        {
            InitializeComponent();
            estadoInicial();
        }

        private void frmActualizarAlojamiento_Load(object sender, EventArgs e)
        {
            textBox6.MaxLength = 10; 
        }


        private void estadoInicial()
        {
            textBox7.Enabled = true;
            textBox7.Clear();
            button1.Enabled = true; 
            textBox1.Enabled = false; textBox1.Clear();
            textBox2.Enabled = false; textBox2.Clear();
            textBox3.Enabled = false; textBox3.Clear();
            textBox4.Enabled = false; textBox4.Clear();
            textBox5.Enabled = false; textBox5.Clear();
            textBox6.Enabled = false; textBox6.Clear();

            button1.Enabled = false; 
            idAlojamientoActual = 0;

            textBox7.Focus();
        }

        private bool CamposEstanLlenos()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) || string.IsNullOrWhiteSpace(textBox6.Text))
            {
                return false;
            }
            return true;
        }


        private void ProcesarBusqueda()
        {
            if (string.IsNullOrWhiteSpace(textBox7.Text))
            {
                MessageBox.Show("Ingrese un ID de alojamiento para buscar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox7.Focus();
                return;
            }

            try
            {
                int idBuscar = int.Parse(textBox7.Text);
                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();

                clsAlojamiento alojEncontrado = objPuente.BuscarAlojamiento(idBuscar);

                if (alojEncontrado != null)
                {
                    idAlojamientoActual = alojEncontrado.Id_alojamiento;
                    textBox1.Text = alojEncontrado.Descripcion;
                    textBox2.Text = alojEncontrado.Ubicacion;

                    List<clsAlojamiento> todos = objPuente.BuscarAlojamientosPorHuespedes(0);
                    clsAlojamiento alojamientoCompleto = todos.FirstOrDefault(a => a.Id_alojamiento == idBuscar);

                    if (alojamientoCompleto != null)
                    {
                        textBox3.Text = alojamientoCompleto.Num_habitaciones.ToString();
                        textBox4.Text = alojamientoCompleto.Max_huespedes.ToString();
                        textBox5.Text = alojamientoCompleto.Num_banos.ToString();

                        // Buscamos el teléfono del admin
                        textBox6.Text = objPuente.ObtenerTelefonoAdminPorId(alojamientoCompleto.Id_administrador);

                        textBox7.Enabled = false;
                        button1.Enabled = true;


                        textBox1.Enabled = true;
                        textBox2.Enabled = true;
                        textBox3.Enabled = true;
                        textBox4.Enabled = true;
                        textBox5.Enabled = true;
                        textBox6.Enabled = true;
                        button2.Enabled = true;

                        textBox1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("No se encontró ningún alojamiento con ese ID.", "Búsqueda sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    estadoInicial();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcesarActualizacion()
        {
            if (!CamposEstanLlenos())
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int habitaciones = int.Parse(textBox3.Text);
                int huespedes = int.Parse(textBox4.Text);
                int banos = int.Parse(textBox5.Text);

                if (habitaciones <= 0 || huespedes <= 0 || banos <= 0)
                {
                    MessageBox.Show("Habitaciones, huéspedes y baños deben ser mayores a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();

                // Validamos que el teléfono del nuevo (o actual) encargado exista
                int idAdmin = objPuente.ObtenerIdAdminPorTelefono(textBox6.Text);

                if (idAdmin == 0)
                {
                    MessageBox.Show("No existe un administrador registrado con ese número de teléfono.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox6.Focus();
                    return;
                }

                // Preparamos los datos
                clsAlojamiento objEntidades = new clsAlojamiento();
                objEntidades.Id_alojamiento = idAlojamientoActual;
                objEntidades.Descripcion = textBox1.Text;
                objEntidades.Ubicacion = textBox2.Text;
                objEntidades.Num_habitaciones = habitaciones;
                objEntidades.Max_huespedes = huespedes;
                objEntidades.Num_banos = banos;
                objEntidades.Id_administrador = idAdmin;

                objPuente.ActualizarAlojamiento(objEntidades);

                MessageBox.Show("Los datos del alojamiento se actualizaron correctamente.", "Actualización Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                estadoInicial();
            }
            catch (FormatException)
            {
                MessageBox.Show("Asegúrese de ingresar solo números enteros donde corresponda.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ProcesarActualizacion();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcesarBusqueda();
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            // ID: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; ProcesarBusqueda(); }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox2.Focus(); }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox3.Focus(); }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Habitaciones: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox4.Focus(); }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Huéspedes: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox5.Focus(); }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Baños: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox6.Focus(); }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Teléfono admin: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; ProcesarActualizacion(); }
        }
    }
}