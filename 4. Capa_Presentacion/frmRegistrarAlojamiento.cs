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
            textBox3.MaxLength = 2; 
            textBox5.MaxLength = 2; 
            textBox6.MaxLength = 10; 
            textBox7.MaxLength = 8;  
        }

        private bool CamposEstanLlenos()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) || 
                string.IsNullOrWhiteSpace(textBox7.Text))
            {
                return false;
            }
            return true;
        }

        private void ProcesarIngreso()
        {
            if (!CamposEstanLlenos())
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. Validaciones de Rangos y Lógica
                int habitaciones = int.Parse(textBox3.Text);
                int huespedes = int.Parse(textBox4.Text);
                int banos = int.Parse(textBox5.Text);

                // Validación para Habitaciones (Min 1, Max 50 por ejemplo)
                if (habitaciones < 1 || habitaciones > 50)
                {
                    MessageBox.Show("El número de habitaciones debe estar entre 1 y 50.", "Rango Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox3.Focus();
                    return;
                }

                // Validación de Baños (No puede ser cero)
                if (banos <= 0)
                {
                    MessageBox.Show("El alojamiento debe tener al menos 1 baño.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox5.Focus();
                    return;
                }

                // Validación de Costo (Debe ser un número decimal válido y mayor a 0)
                if (!decimal.TryParse(textBox7.Text, out decimal costo) || costo <= 0)
                {
                    MessageBox.Show("Ingrese un costo de reservación válido y mayor a cero.", "Error de Costo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox7.Focus();
                    return;
                }

                clsPuenteAlojamiento objPuente = new clsPuenteAlojamiento();
                int idAdmin = objPuente.ObtenerIdAdminPorTelefono(textBox6.Text.Trim());

                if (idAdmin == 0)
                {
                    MessageBox.Show("El número de teléfono no coincide con ningún administrador registrado. Verifique el número e intente de nuevo.", "Administrador no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Mapeo de Entidades y Guardado
                clsAlojamiento objEntidades = new clsAlojamiento
                {
                    Descripcion = textBox1.Text.Trim(),
                    Ubicacion = textBox2.Text.Trim(),
                    Num_habitaciones = habitaciones,
                    Max_huespedes = huespedes,
                    Num_banos = banos,
                    Id_administrador = idAdmin,
                    Precio_por_noche = costo // Asegúrate que esta propiedad exista en tu Capa_Entidades
                };

                objPuente.IngresarAlojamiento(objEntidades);

                MessageBox.Show("Alojamiento registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar el registro: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            // Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;

            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                // VALIDACIÓN DE MÍNIMO Y MÁXIMO
                if (int.TryParse(textBox3.Text, out int cant) && cant >= 1 && cant <= 20)
                {
                    textBox4.Focus(); // Valor correcto, avanza
                }
                else
                {
                    MessageBox.Show("El número de habitaciones debe estar entre 1 y 20.", "Rango Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox3.SelectAll();
                }
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (char.IsDigit(e.KeyChar))
            {
                string textoSimulado = textBox4.Text.Remove(textBox4.SelectionStart, textBox4.SelectionLength)
                                                    .Insert(textBox4.SelectionStart, e.KeyChar.ToString());

                if (int.TryParse(textoSimulado, out int valorFuturo) && valorFuturo > 50)
                {
                    e.Handled = true;
                    return;
                }
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;

                if (int.TryParse(textBox4.Text, out int cant) && cant >= 1)
                {
                    textBox5.Focus();
                }
                else
                {
                    MessageBox.Show("Debe ingresar al menos 1 huésped.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox4.SelectAll();
                }
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;

            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                // VALIDACIÓN DE MÍNIMO (No puede ser cero)
                if (int.TryParse(textBox5.Text, out int cant) && cant > 0)
                {
                    textBox6.Focus(); // Valor correcto, avanza
                }
                else
                {
                    MessageBox.Show("El número de baños debe ser al menos 1.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox5.SelectAll();
                }
            }
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

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 1. Permitir teclas de control (como borrar)
            if (char.IsControl(e.KeyChar)) return;

            // 2. Definir separadores aceptados
            bool esSeparador = e.KeyChar == '.' || e.KeyChar == ',';

            // 3. Filtrar: solo números y un separador
            if (!char.IsDigit(e.KeyChar) && !esSeparador)
            {
                e.Handled = true;
                return;
            }

            // 4. VALIDACIÓN PREVENTIVA (Simulación de texto futuro)
            string textoActual = textBox7.Text;
            int cursorIndex = textBox7.SelectionStart;
            int selectionLength = textBox7.SelectionLength;

            // Construimos cómo quedaría el texto si aceptamos la tecla
            string textoSimulado = textoActual.Remove(cursorIndex, selectionLength)
                                              .Insert(cursorIndex, e.KeyChar.ToString());

            // Reemplazamos coma por punto para una validación numérica estándar
            string textoParaValidar = textoSimulado.Replace(',', '.');

            if (decimal.TryParse(textoParaValidar, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal valorFuturo))
            {
                // A. Validar Valor Máximo (999.99)
                if (valorFuturo > 999.99m)
                {
                    e.Handled = true;
                    return;
                }

                // B. Validar Máximo de 2 Decimales
                int indicePunto = textoParaValidar.IndexOf('.');
                if (indicePunto != -1)
                {
                    string parteDecimal = textoParaValidar.Substring(indicePunto + 1);
                    if (parteDecimal.Length > 2)
                    {
                        e.Handled = true;
                        return;
                    }
                }
            }
            else if (esSeparador && (textoActual.Contains(".") || textoActual.Contains(",")))
            {
                // Evitar doble punto o doble coma
                e.Handled = true;
                return;
            }

            // 5. LÓGICA AL PRESIONAR ENTER
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (decimal.TryParse(textBox7.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal precio) && precio > 0)
                {
                    ProcesarIngreso();
                }
                else
                {
                    MessageBox.Show("Ingrese un costo válido mayor a 0 (Máx: 999.99).", "Error de Precio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox7.SelectAll();
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
