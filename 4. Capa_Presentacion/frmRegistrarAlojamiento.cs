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
    public partial class frmRegistrarAlojamiento : Form
    {
        public frmRegistrarAlojamiento()
        {
            InitializeComponent();
        }

        private void frmRegistrarAlojamiento_Load(object sender, EventArgs e)
        {
        
            textBox6.MaxLength = 10;
        }

        private bool CamposEstanLlenos()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text))
            {
                return false;
            }
            return true;
        }

        private void ProcesarIngreso()
        {
            if (!CamposEstanLlenos())
            {
                MessageBox.Show("Todos los campos son obligatorios. Por favor, complete la información.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int habitaciones = int.Parse(textBox3.Text);
                int huespedes = int.Parse(textBox4.Text);
                int banos = int.Parse(textBox5.Text);
                string telefonoAdmin = textBox6.Text;

                if (habitaciones <= 0 || huespedes <= 0 || banos <= 0)
                {
                    MessageBox.Show("El número de habitaciones, huéspedes admitidos y baños debe ser mayor a cero.", "Validación de Lógica", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();

                // Buscamos si existe un administrador con ese teléfono
                int idAdmin = objPuente.ObtenerIdAdminPorTelefono(telefonoAdmin);

                if (idAdmin == 0)
                {
                    MessageBox.Show("No se encontró ningún Administrador encargado con el número de teléfono ingresado.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox6.SelectAll();
                    textBox6.Focus();
                    return;
                }

                //Guardamos
                clsAlojamiento objEntidades = new clsAlojamiento();
                objEntidades.Descripcion = textBox1.Text;
                objEntidades.Ubicacion = textBox2.Text;
                objEntidades.Num_habitaciones = habitaciones;
                objEntidades.Max_huespedes = huespedes;
                objEntidades.Num_banos = banos;
                objEntidades.Id_administrador = idAdmin;

                objPuente.IngresarAlojamiento(objEntidades);

                MessageBox.Show("Alojamiento registrado correctamente en el sistema.", "Ingreso Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
                textBox1.Focus();
            }
            catch (FormatException)
            {
                MessageBox.Show("Asegúrese de ingresar solo valores numéricos enteros en las Habitaciones, Huéspedes y Baños.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al registrar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ProcesarIngreso();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Descripción: Permite texto libre
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox2.Focus(); }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Ubicación: Permite texto libre
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
            // Huéspedes admitidos: Solo números
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
            // Teléfono del encargado: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarIngreso();
            }
        }
    }
}