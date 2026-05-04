using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Entidades;
using Capa_Logica;
using Microsoft.VisualBasic; 

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
                    // 1. Construimos y mostramos el mensaje de texto ordenado
                    StringBuilder mensajeEmergente = new StringBuilder();
                    string nombreHuesped = dtResultados.Rows[0]["Huésped"].ToString();
                    mensajeEmergente.AppendLine($"Reservas a nombre de: {nombreHuesped}\n");
                    mensajeEmergente.AppendLine("==================================\n");

                    foreach (DataRow fila in dtResultados.Rows)
                    {
                        mensajeEmergente.AppendLine($"📌 RESERVA ID: {fila["ID Reserva"]}");
                        mensajeEmergente.AppendLine($"🏠 Alojamiento: {fila["Alojamiento"]}");

                        DateTime fechaIn = Convert.ToDateTime(fila["Fecha Ingreso"]);
                        DateTime fechaOut = Convert.ToDateTime(fila["Fecha Salida"]);
                        mensajeEmergente.AppendLine($"📅 Fechas: {fechaIn.ToShortDateString()} al {fechaOut.ToShortDateString()}");

                        mensajeEmergente.AppendLine($"👥 Capacidad: {fila["N° Personas"]} personas ({fila["Tipo"]})");
                        mensajeEmergente.AppendLine("----------------------------------");
                    }

                    // 2. Mostramos el resumen de las reservas
                    MessageBox.Show(mensajeEmergente.ToString(), "Detalles de Reservas", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // --- 3. NUEVA LÓGICA DE IMPRESIÓN ---
                    DialogResult respuesta = MessageBox.Show(
                        "¿Desea visualizar e imprimir el comprobante/factura de alguna de estas reservas?",
                        "Generar Factura",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (respuesta == DialogResult.Yes)
                    {
                        if (dtResultados.Rows.Count == 1)
                        {
                            // Si solo hay una, ahorramos tiempo y la cargamos directo
                            int idUnica = Convert.ToInt32(dtResultados.Rows[0]["ID Reserva"]);
                            MostrarFacturaPorId(idUnica);
                        }
                        else
                        {
                            // Si hay varias, le pedimos que digite cuál quiere
                            string input = Interaction.InputBox(
                                "Se encontraron varias reservas.\nIngrese el número de 'RESERVA ID' que desea imprimir:",
                                "Seleccionar Reserva", "");

                            if (int.TryParse(input, out int idSeleccionado))
                            {
                                MostrarFacturaPorId(idSeleccionado);
                            }
                        }
                    }

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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                EjecutarConsulta();
            }
        }

        // ====================================================================
        // === MÉTODOS PARA RECONSTRUIR Y VISUALIZAR LA FACTURA ===
        // ====================================================================

        private void MostrarFacturaPorId(int idReserva)
        {
            try
            {
                // 1. Obtener la data cruda de la reserva seleccionada
                clsPuenteReserva pRes = new clsPuenteReserva();
                DataTable dt = pRes.BuscarReservaPorId(idReserva);

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No se pudo extraer la información detallada de esta reserva.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow r = dt.Rows[0];
                int idAloj = Convert.ToInt32(r["Id_alojamiento"]);
                string tipo = r["tipo"].ToString();
                int numPersonas = Convert.ToInt32(r["numero_personas"]);
                DateTime fIn = Convert.ToDateTime(r["fecha_ingreso"]);
                DateTime fOut = Convert.ToDateTime(r["fecha_salida"]);
                string cedula = r["ci"].ToString();

                // 2. Traer el Alojamiento
                clsPuenteAlojamiento pAloj = new clsPuenteAlojamiento();
                var aloj = pAloj.BuscarAlojamiento(idAloj);

                // 3. Traer al Huésped
                clsPuenteHuesped pHuesped = new clsPuenteHuesped();
                var huesped = pHuesped.ObtenerHuespedes().FirstOrDefault(h => h.Ci == cedula);

                if (aloj == null || huesped == null)
                {
                    MessageBox.Show("Falta información referencial (Alojamiento o Huésped) para generar la factura.", "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 4. Armar los objetos nuevamente
                clsReserva res = new clsReserva
                {
                    Id_reserva = idReserva,
                    Fecha_ingreso = fIn,
                    Fecha_salida = fOut,
                    Numero_personas = numPersonas,
                    Tipo = tipo,
                    Id_alojamiento = idAloj,
                    Id_huesped = huesped.Id_huesped
                };

                // 5. Recalcular valores monetarios
                decimal precioFinal = CalcularPrecioAjustado(aloj.Precio_por_noche, numPersonas, tipo);
                decimal[] v = pRes.CalcularValoresFactura(fIn, fOut, precioFinal);

                clsFactura fact = new clsFactura
                {
                    Dias_ocupacion = (int)v[0],
                    Subtotal = v[1],
                    Impuestos = v[2],
                    Total = v[3]
                };

                // 6. Enviar a pantalla
                VisualizarFactura(res, fact, aloj, huesped);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la vista de la factura: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal CalcularPrecioAjustado(decimal precioBase, int personas, string tipo)
        {
            decimal precioAjustado = precioBase;
            if (tipo == "Privada") precioAjustado *= 1.20m;
            if (tipo == "Compartida") precioAjustado *= 0.80m;
            if (personas > 1) precioAjustado += (personas - 1) * 10.00m;
            return precioAjustado;
        }

        private void VisualizarFactura(clsReserva res, clsFactura fact, clsAlojamiento aloj, clsHuesped huesped)
        {
            PrintDocument documento = new PrintDocument();
            documento.PrintPage += (s, ev) => DibujarFactura(ev, res, fact, aloj, huesped);

            PrintPreviewDialog visor = new PrintPreviewDialog
            {
                Document = documento,
                WindowState = FormWindowState.Maximized,
                ShowIcon = false,
                Text = "Copia de Factura - RommyEc"
            };
            visor.ShowDialog();
        }

        private void DibujarFactura(PrintPageEventArgs e, clsReserva res, clsFactura fact, clsAlojamiento aloj, clsHuesped huesped)
        {
            Graphics g = e.Graphics;
            Font fontTitulo = new Font("Arial", 18, FontStyle.Bold);
            Font fontSubtitulo = new Font("Arial", 14, FontStyle.Bold);
            Font fontTexto = new Font("Arial", 12, FontStyle.Regular);
            Font fontNegrita = new Font("Arial", 12, FontStyle.Bold);
            Brush brocha = Brushes.Black;

            int margenIzq = 50;
            int y = 50;

            g.DrawString("HOTEL Y ALOJAMIENTO ROMMYEC", fontTitulo, brocha, margenIzq, y);
            y += 30;
            g.DrawString("Riobamba, Ecuador", fontTexto, brocha, margenIzq, y);
            y += 40;

            g.DrawLine(new Pen(Color.Black, 2), margenIzq, y, 750, y);
            y += 20;

            g.DrawString("COMPROBANTE / COPIA DE FACTURA", fontSubtitulo, brocha, 220, y);
            y += 40;

            g.DrawString($"Fecha de Emisión: {DateTime.Now.ToShortDateString()}", fontTexto, brocha, margenIzq, y);
            y += 25;
            g.DrawString($"Cédula Cliente: {huesped.Ci}", fontTexto, brocha, margenIzq, y);
            y += 40;

            g.DrawString("DETALLES DEL SERVICIO:", fontSubtitulo, brocha, margenIzq, y);
            y += 30;
            g.DrawString($"- Reserva ID: {res.Id_reserva}", fontTexto, brocha, margenIzq, y);
            y += 25;
            g.DrawString($"- Alojamiento ID: {aloj.Id_alojamiento} ({aloj.Descripcion})", fontTexto, brocha, margenIzq, y);
            y += 25;
            g.DrawString($"- Tipo de Reserva: {res.Tipo}", fontTexto, brocha, margenIzq, y);
            y += 25;
            g.DrawString($"- Fechas: {res.Fecha_ingreso.ToShortDateString()} al {res.Fecha_salida.ToShortDateString()} ({fact.Dias_ocupacion} días)", fontTexto, brocha, margenIzq, y);
            y += 25;
            g.DrawString($"- Huéspedes admitidos: {res.Numero_personas}", fontTexto, brocha, margenIzq, y);
            y += 40;

            g.DrawLine(new Pen(Color.Gray, 1), margenIzq, y, 750, y);
            y += 20;

            int margenDerecho = 500;

            g.DrawString("Subtotal:", fontTexto, brocha, margenDerecho, y);
            g.DrawString(fact.Subtotal.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-EC")), fontTexto, brocha, margenDerecho + 120, y);
            y += 25;

            g.DrawString("IVA (15%):", fontTexto, brocha, margenDerecho, y);
            g.DrawString(fact.Impuestos.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-EC")), fontTexto, brocha, margenDerecho + 120, y);
            y += 25;

            g.DrawString("TOTAL:", fontNegrita, brocha, margenDerecho, y);
            g.DrawString(fact.Total.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-EC")), fontNegrita, brocha, margenDerecho + 120, y);

            y += 80;
            g.DrawString("¡Gracias por su preferencia!", fontSubtitulo, brocha, 280, y);
        }
    }
}