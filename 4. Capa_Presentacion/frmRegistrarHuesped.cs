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
using Capa_Entidades;
using Capa_Logica;
namespace _4.Capa_Presentacion
{
    public partial class frmRegistrarHuesped : Form
    {
        public frmRegistrarHuesped()
        {
            InitializeComponent();
        }
        private void frmRegistrarHuesped_Load(object sender, EventArgs e)
        {
            // Establecemos los límites de longitud directamente en código
            textBox1.MaxLength = 10; // Cédula
            textBox6.MaxLength = 10; // Teléfono

            // Ocultamos la contraseña
            textBox5.PasswordChar = '*';
        }

        // MÉTODOS DE VALIDACIÓN

        private bool CamposEstanLlenos()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text))
            {
                return false;
            }
            return true;
        }

        private bool ValidarCorreo(string correo)
        {
            // Validamos que el formato sea texto@texto.texto
            string formato = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo, formato);
        }
        //Validadción de cédula ecuatoriana basada en el algoritmo oficial
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
                else 
                {
                    suma += digito;
                }
            }

            int digitoVerificador = int.Parse(cedula.Substring(9, 1));
            int decenaSuperior = ((suma / 10) + 1) * 10;
            int residuo = decenaSuperior - suma;

            if (residuo == 10) residuo = 0;

            return residuo == digitoVerificador;
        }

        // LÓGICA PRINCIPAL DE GUARDADO

        private void ProcesarIngreso()
        {
            if (!CamposEstanLlenos())
            {
                MessageBox.Show("Todos los campos son obligatorios. Por favor, llénelos todos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarCedulaEcuatoriana(textBox1.Text))
            {
                MessageBox.Show("La cédula ingresada no es válida para el territorio ecuatoriano.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            if (textBox6.Text.Length < 9)
            {
                MessageBox.Show("Ingrese un número de teléfono válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox6.Focus();
                return;
            }

            if (!ValidarCorreo(textBox4.Text))
            {
                MessageBox.Show("El formato del correo electrónico no es válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return;
            }

            if (textBox5.Text.Length < 4)
            {
                MessageBox.Show("La contraseña debe tener al menos 4 caracteres por seguridad.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Focus();
                return;
            }

            try
            {
                clsPuenteHuesped objPuente = new clsPuenteHuesped();

                // Validar duplicados en Base de Datos
                if (objPuente.ExisteCedulaDuplicada(textBox1.Text))
                {
                    MessageBox.Show("Ya existe un huésped registrado con ese número de cédula.", "Duplicado detectado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.SelectAll();
                    textBox1.Focus();
                    return;
                }

                clsHuesped objEntidades = new clsHuesped();
                objEntidades.Ci = textBox1.Text;
                objEntidades.Nombres = textBox2.Text;
                objEntidades.Apellidos = textBox3.Text;
                objEntidades.Correo = textBox4.Text;
                objEntidades.Telefono = textBox6.Text;
                objEntidades.Contrasena = textBox5.Text;

                objPuente.IngresarHuesped(objEntidades);

                MessageBox.Show("Huésped registrado correctamente en el sistema.", "Ingreso Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();

                textBox1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al ingresar el huésped: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // CI: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox2.Focus(); }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Nombres: Solo letras y espacios
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsSeparator(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox3.Focus(); }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Apellidos: Solo letras y espacios
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsSeparator(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox4.Focus(); }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Correo: No se permiten espacios en blanco
            if (char.IsSeparator(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox6.Focus(); }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Teléfono: Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; textBox5.Focus(); }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Contraseña: Evitamos espacios vacíos al principio o al final
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarIngreso();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox7_KeyPress(object sender, KeyPressEventArgs e) { }
    }
}
