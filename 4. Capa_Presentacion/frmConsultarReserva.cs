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
    public partial class frmConsultarReserva : Form
    {
        public frmConsultarReserva()
        {
            InitializeComponent();
        }

        private void frmConsultarReserva_Load(object sender, EventArgs e)
        {
            
            textBox1.MaxLength = 10;
        }

        private void EjecutarConsulta()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor, ingrese un número de cédula para consultar.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return;
            }

            try
            {
                clsPuenteReserva objPuente = new clsPuenteReserva();
                DataTable dtResultados = objPuente.ConsultarReservasPorCedula(textBox1.Text);

               
                if (dtResultados.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontraron reservas registradas para la cédula ingresada.", "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.SelectAll();
                    textBox1.Focus();
                }
                else
                {
                    // Utilizamos StringBuilder para construir un mensaje de texto ordenado
                    StringBuilder mensajeEmergente = new StringBuilder();

                    // Extraemos el nombre del huésped de la primera fila para el título
                    string nombreHuesped = dtResultados.Rows[0]["Huésped"].ToString();
                    mensajeEmergente.AppendLine($"Reservas a nombre de: {nombreHuesped}\n");
                    mensajeEmergente.AppendLine("==================================\n");

                    // Recorremos todas las reservas encontradas y las agregamos al mensaje
                    foreach (DataRow fila in dtResultados.Rows)
                    {
                        mensajeEmergente.AppendLine($"📌 RESERVA ID: {fila["ID Reserva"]}");
                        mensajeEmergente.AppendLine($"🏠 Alojamiento: {fila["Alojamiento"]}");

                        // Formateamos las fechas para que no muestren la hora (00:00:00)
                        DateTime fechaIn = Convert.ToDateTime(fila["Fecha Ingreso"]);
                        DateTime fechaOut = Convert.ToDateTime(fila["Fecha Salida"]);
                        mensajeEmergente.AppendLine($"📅 Fechas: {fechaIn.ToShortDateString()} al {fechaOut.ToShortDateString()}");

                        mensajeEmergente.AppendLine($"👥 Capacidad: {fila["N° Personas"]} personas ({fila["Tipo"]})");
                        mensajeEmergente.AppendLine("----------------------------------");
                    }

                    // Mostramos la ventana emergente con toda la información
                    MessageBox.Show(mensajeEmergente.ToString(), "Detalles de Reservas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpiamos la caja de texto para una nueva consulta
                    textBox1.Clear();
                    textBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al realizar la consulta: " + ex.Message, "Error Interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EjecutarConsulta();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Validamos que SOLO se puedan ingresar números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // Si presiona "Enter", buscar automáticamente
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                EjecutarConsulta();
            }
        }
    }
}