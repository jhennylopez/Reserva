using Capa_Entidades;
using Capa_Logica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4.Capa_Presentacion
{
    public partial class frmActualizarHuesped : Form
    {

        // Variable global para almacenar el ID interno del huésped que estamos editando
        private int idHuespedActual = 0;

        public frmActualizarHuesped()
        {
            InitializeComponent();
            estadoInicial();
        }

        private void frmActualizarHuesped_Load(object sender, EventArgs e)
        {
            textBox1.MaxLength = 10; // Cédula
            textBox6.MaxLength = 10; // Teléfono
            textBox5.PasswordChar = '*'; // Ocultar contraseña 
        }


        private void estadoInicial()
        {
            // Habilitamos la cédula y el botón Buscar
            textBox1.Enabled = true;
            textBox1.Clear();
            button2.Enabled = true; // Botón Buscar

            // Deshabilitamos el resto de campos y el botón Actualizar hasta encontrar al huésped
            textBox2.Enabled = false; textBox2.Clear();
            textBox3.Enabled = false; textBox3.Clear();
            textBox4.Enabled = false; textBox4.Clear();
            textBox5.Enabled = false; textBox5.Clear();
            textBox6.Enabled = false; textBox6.Clear();

            button1.Enabled = false; // Botón Actualizar

            idHuespedActual = 0;
            textBox1.Focus();
        }

        private bool CamposEstanLlenos()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) || string.IsNullOrWhiteSpace(textBox5.Text))
            {
                return false;
            }
            return true;
        }

        private bool ValidarCorreo(string correo)
        {
            string formato = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, formato);
        }

        private bool ValidarCedulaEcuatoriana(string cedula)
        {
            if (cedula.Length != 10) return false;
            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || (provincia > 24 && provincia != 30)) return false;
            int tercerDigito = int.Parse(cedula.Substring(2, 1));
            if (tercerDigito >= 6) return false;

            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int digito = int.Parse(cedula.Substring(i, 1));
                if (i % 2 == 0)
                {
                    int producto = digito * 2;
                    suma += (producto > 9) ? producto - 9 : producto;
                }
                else { suma += digito; }
            }

            int digitoVerificador = int.Parse(cedula.Substring(9, 1));
            int decenaSuperior = ((suma / 10) + 1) * 10;
            int residuo = decenaSuperior - suma;
            if (residuo == 10) residuo = 0;

            return residuo == digitoVerificador;
        }
        private void ProcesarBusqueda()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Ingrese una cédula para buscar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            try
            {
                clsPuenteHuesped objPuente = new clsPuenteHuesped();
                List<clsHuesped> listaHuespedes = objPuente.ObtenerHuespedes();
                clsHuesped huespedEncontrado = listaHuespedes.FirstOrDefault(h => h.Ci == textBox1.Text);

                if (huespedEncontrado != null)
                {
                    // Guardamos el ID y llenamos los campos con los datos de la base
                    idHuespedActual = huespedEncontrado.Id_huesped;
                    textBox2.Text = huespedEncontrado.Nombres;
                    textBox3.Text = huespedEncontrado.Apellidos;
                    textBox4.Text = huespedEncontrado.Correo;
                    textBox5.Text = huespedEncontrado.Contrasena;
                    textBox6.Text = huespedEncontrado.Telefono;

                    // Bloqueamos la búsqueda y habilitamos la edición
                    textBox1.Enabled = false;
                    button2.Enabled = false; // Desactivar botón buscar

                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    textBox4.Enabled = true;
                    textBox5.Enabled = true;
                    textBox6.Enabled = true;
                    button1.Enabled = true; // Activar botón Actualizar

                    textBox2.Focus();
                }
                else
                {
                    MessageBox.Show("No se encontró ningún huésped con esa cédula.", "Error de Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            if (textBox6.Text.Length < 9)
            {
                MessageBox.Show("Ingrese un número de teléfono válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox6.Focus(); return;
            }

            if (!ValidarCorreo(textBox4.Text))
            {
                MessageBox.Show("Formato de correo inválido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus(); return;
            }

            if (textBox5.Text.Length < 4)
            {
                MessageBox.Show("La contraseña debe tener al menos 4 caracteres por seguridad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Focus(); return;
            }

            try
            {
                clsPuenteHuesped objPuente = new clsPuenteHuesped();

                clsHuesped objEntidades = new clsHuesped();
                objEntidades.Id_huesped = idHuespedActual;
                objEntidades.Ci = textBox1.Text;
                objEntidades.Nombres = textBox2.Text;
                objEntidades.Apellidos = textBox3.Text;
                objEntidades.Correo = textBox4.Text;
                objEntidades.Telefono = textBox6.Text;
                objEntidades.Contrasena = textBox5.Text;

                objPuente.ActualizarHuesped(objEntidades);

                MessageBox.Show("Datos actualizados correctamente en el sistema.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                estadoInicial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar los datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cédula: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarBusqueda();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsSeparator(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox3.Focus(); }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsSeparator(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox4.Focus(); }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsSeparator(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox5.Focus(); }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Contraseña
            if (char.IsSeparator(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox6.Focus(); }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Teléfono: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarActualizacion();
            }
        }
    }
}