using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Capa_Entidades;
using Capa_Logica;
using System.Drawing.Printing; // Importación necesaria para imprimir
using System.Drawing; // Necesaria para fuentes y colores de la factura

namespace _4.Capa_Presentacion
{
    public partial class frmNuevaReserva : Form
    {
        clsPuenteReserva pRes = new clsPuenteReserva();
        clsPuenteAlojamiento pAloj = new clsPuenteAlojamiento();

        public frmNuevaReserva()
        {
            InitializeComponent();
            this.dateTimePicker1.ValueChanged += ActualizarCalculoEnPantalla;
            this.dateTimePicker2.ValueChanged += ActualizarCalculoEnPantalla;
            this.textBox3.TextChanged += ActualizarCalculoEnPantalla;
            this.textBox1.TextChanged += ActualizarCalculoEnPantalla;
            this.comboBox2.SelectedIndexChanged += ActualizarCalculoEnPantalla;
        }

        private void frmNuevaReserva_Load(object sender, EventArgs e)
        {
            ConfigurarControles();
            CargarDatosIniciales();
        }

        private void ConfigurarControles()
        {
            dateTimePicker1.MinDate = DateTime.Today;
            dateTimePicker2.MinDate = DateTime.Today.AddDays(1);
            textBox2.MaxLength = 10; // Cédula
            textBox1.MaxLength = 2;  // Huéspedes

            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new string[] { "Completa", "Compartida", "Privada" });
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.SelectedIndex = 0;

            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            textBox3.Clear();
            label8.Text = "$ 0.00";
        }

        private void CargarDatosIniciales()
        {
            try
            {
                DataTable dt = pRes.BuscarReservaPorId(0);
                if (dt != null && dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt.AsEnumerable().Select(a => new {
                        ID = a.Field<int>("Id_reserva"),
                        Entrada = a.Field<DateTime>("fecha_ingreso").ToShortDateString(),
                        Salida = a.Field<DateTime>("fecha_salida").ToShortDateString(),
                        Personas = a.Field<int>("numero_personas"),
                        Tipo = a.Field<string>("tipo"),
                        Cedula = a.Field<string>("ci"),
                        Alojamiento = a.Field<int>("Id_alojamiento")
                    }).ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar datos: " + ex.Message); }
        }

        private void ActualizarCalculoEnPantalla(object sender, EventArgs e)
        {
            // Validamos que tengamos ID de alojamiento y número de personas válido para calcular
            if (int.TryParse(textBox3.Text, out int idAloj) && int.TryParse(textBox1.Text, out int numPersonas))
            {
                var aloj = pAloj.BuscarAlojamiento(idAloj);
                if (aloj != null && dateTimePicker2.Value.Date > dateTimePicker1.Value.Date)
                {
                    decimal precioFinalPorNoche = CalcularPrecioAjustado(aloj.Precio_por_noche, numPersonas, comboBox2.Text);

                    decimal[] valores = pRes.CalcularValoresFactura(dateTimePicker1.Value, dateTimePicker2.Value, precioFinalPorNoche);

                    label8.Text = valores[1].ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-EC"));
                }
                else { label8.Text = "$ 0.00"; }
            }
            else { label8.Text = "$ 0.00"; }
        }

        private decimal CalcularPrecioAjustado(decimal precioBase, int personas, string tipo)
        {
            decimal precioAjustado = precioBase;

            // 1. Ajuste por Tipo de Reserva
            if (tipo == "Privada") precioAjustado *= 1.20m;
            if (tipo == "Compartida") precioAjustado *= 0.80m;

            // 2. Ajuste por Huéspedes (Ejemplo: $10 adicionales por persona después de la primera)
            if (personas > 1)
            {
                precioAjustado += (personas - 1) * 10.00m;
            }

            return precioAjustado;
        }

        private void ProcesarReserva()
        {
            if (!ValidarCampos()) return;

            try
            {
                int idAlojamiento = int.Parse(textBox3.Text);
                string cedula = textBox2.Text.Trim();
                int numHuespedes = int.Parse(textBox1.Text);
                string tipoReserva = comboBox2.Text;

                // 1. Obtener datos del alojamiento
                var aloj = pAloj.BuscarAlojamiento(idAlojamiento);
                if (aloj == null) { MessageBox.Show("Alojamiento no válido."); return; }

                // 2. Traer el historial completo de reservas
                DataTable dtExistentes = pRes.BuscarReservaPorId(0);

                if (dtExistentes != null && dtExistentes.Rows.Count > 0)
                {
                    // Filtramos SOLO las reservaciones que chocan con las fechas elegidas
                    var superpuestas = dtExistentes.AsEnumerable().Where(r =>
                        r.Field<int>("Id_alojamiento") == idAlojamiento &&
                        r.Field<DateTime>("fecha_ingreso").Date < dateTimePicker2.Value.Date && // Entrada Existente < Salida Nueva
                        r.Field<DateTime>("fecha_salida").Date > dateTimePicker1.Value.Date     // Salida Existente > Entrada Nueva
                    ).ToList();

                    if (superpuestas.Any())
                    {
                        // A) REGLA ANTI-DUPLICADOS EXACTOS
                        bool existeDuplicado = superpuestas.Any(r =>
                            r.Field<string>("tipo") == tipoReserva &&
                            r.Field<DateTime>("fecha_ingreso").Date == dateTimePicker1.Value.Date &&
                            r.Field<DateTime>("fecha_salida").Date == dateTimePicker2.Value.Date &&
                            r.Field<string>("ci") == cedula
                        );

                        if (existeDuplicado)
                        {
                            MessageBox.Show("Este huésped ya tiene una reservación idéntica registrada para estas fechas.", "Reserva Duplicada", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }

                        // B) REGLA "COMPLETA"
                        if (superpuestas.Any(r => r.Field<string>("tipo") == "Completa"))
                        {
                            MessageBox.Show("El alojamiento ya está rentado de forma Completa en estas fechas. No se admiten más reservaciones.", "Alojamiento Lleno", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (tipoReserva == "Completa")
                        {
                            MessageBox.Show("No se puede hacer una reserva Completa porque ya hay habitaciones ocupadas por otros huéspedes en estas fechas.", "Conflicto de Disponibilidad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // C) REGLA "PRIVADA"
                        int cuartosPrivadosOcupados = superpuestas.Count(r => r.Field<string>("tipo") == "Privada");
                        bool hayCompartidas = superpuestas.Any(r => r.Field<string>("tipo") == "Compartida");
                        int cuartosDisponibles = aloj.Num_habitaciones - cuartosPrivadosOcupados - (hayCompartidas ? 1 : 0);

                        if (tipoReserva == "Privada" && cuartosDisponibles <= 0)
                        {
                            MessageBox.Show("No hay suficientes habitaciones disponibles para una reserva Privada en estas fechas.", "Sin Cuartos Libres", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // D) REGLA "COMPARTIDA" / CAPACIDAD TOTAL
                        int personasOcupadas = superpuestas.Sum(r => r.Field<int>("numero_personas"));
                        if ((personasOcupadas + numHuespedes) > aloj.Max_huespedes)
                        {
                            MessageBox.Show($"Capacidad excedida. Solo quedan {aloj.Max_huespedes - personasOcupadas} espacios/camas disponibles en este rango de fechas.", "Capacidad Llena", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }

                // Validación de capacidad máxima base
                if (numHuespedes > aloj.Max_huespedes)
                {
                    MessageBox.Show($"Este alojamiento solo permite un máximo de {aloj.Max_huespedes} personas en total.", "Capacidad Excedida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                    return;
                }

                // 3. Validar Huésped
                clsPuenteHuesped puenteH = new clsPuenteHuesped();
                var huesped = puenteH.ObtenerHuespedes().FirstOrDefault(h => h.Ci == cedula);

                if (huesped == null)
                {
                    MessageBox.Show("Huésped no encontrado en la base de datos.", "Error de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 4. Preparar Entidades
                clsReserva nuevaRes = new clsReserva
                {
                    Fecha_ingreso = dateTimePicker1.Value.Date,
                    Fecha_salida = dateTimePicker2.Value.Date,
                    Numero_personas = numHuespedes,
                    Tipo = tipoReserva,
                    Id_huesped = huesped.Id_huesped,
                    Id_alojamiento = idAlojamiento
                };

                // 5. Cálculo y Factura
                decimal precioFinal = CalcularPrecioAjustado(aloj.Precio_por_noche, numHuespedes, tipoReserva);
                decimal[] v = pRes.CalcularValoresFactura(nuevaRes.Fecha_ingreso, nuevaRes.Fecha_salida, precioFinal);

                clsFactura nuevaFactura = new clsFactura
                {
                    Dias_ocupacion = (int)v[0],
                    Subtotal = v[1],
                    Impuestos = v[2],
                    Total = v[3]
                };

                // 6. Guardar en Base de Datos
                pRes.IngresarReservaConFactura(nuevaRes, nuevaFactura);

                // --- INTEGRACIÓN DE FACTURA: VENTANA EMERGENTE ---
                DialogResult respuesta = MessageBox.Show(
                    "✔ La reserva y su factura han sido generadas correctamente.\n\n¿Desea visualizar la factura ahora para imprimirla o guardarla en PDF?",
                    "Confirmación Exitosa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    VisualizarFactura(nuevaRes, nuevaFactura, aloj, huesped);
                }

                LimpiarFormulario();
                CargarDatosIniciales();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox1.Text) ||
                comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor complete todos los campos obligatorios.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void LimpiarFormulario()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            comboBox2.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today.AddDays(1);
            label8.Text = "$ 0.00";
        }

        private void button2_Click(object sender, EventArgs e) { ProcesarReserva(); }

        private void button1_Click(object sender, EventArgs e) { this.Close(); }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // --- MÉTODOS PARA VISUALIZACIÓN E IMPRESIÓN DE LA FACTURA ---

        private void VisualizarFactura(clsReserva res, clsFactura fact, clsAlojamiento aloj, clsHuesped huesped)
        {
            PrintDocument documento = new PrintDocument();

            // Suscribimos el evento que se encarga de "dibujar" la página
            documento.PrintPage += (s, ev) => DibujarFactura(ev, res, fact, aloj, huesped);

            // Configuramos el visor predeterminado de Windows
            PrintPreviewDialog visor = new PrintPreviewDialog
            {
                Document = documento,
                WindowState = FormWindowState.Maximized,
                ShowIcon = false,
                Text = "Vista Previa de Factura - RommyEc"
            };

            visor.ShowDialog(); // Abre la ventana para visualizar, imprimir o exportar a PDF
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
            int y = 50; // Posición vertical inicial

            // 1. ENCABEZADO DE LA EMPRESA
            g.DrawString("HOTEL Y ALOJAMIENTO ROMMYEC", fontTitulo, brocha, margenIzq, y);
            y += 30;
            g.DrawString("Riobamba, Ecuador", fontTexto, brocha, margenIzq, y);
            y += 40;

            // Línea separadora
            g.DrawLine(new Pen(Color.Black, 2), margenIzq, y, 750, y);
            y += 20;

            // 2. DETALLES DE LA FACTURA
            g.DrawString("FACTURA DE RESERVACIÓN", fontSubtitulo, brocha, 280, y);
            y += 40;

            // Imprimimos la fecha de emisión (hoy) y los datos del cliente
            g.DrawString($"Fecha de Emisión: {DateTime.Now.ToShortDateString()}", fontTexto, brocha, margenIzq, y);
            y += 25;
            g.DrawString($"Cédula Cliente: {huesped.Ci}", fontTexto, brocha, margenIzq, y);
            y += 40;

            // 3. DETALLES DE LA RESERVA
            g.DrawString("DETALLES DEL SERVICIO:", fontSubtitulo, brocha, margenIzq, y);
            y += 30;
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

            // 4. DESGLOSE DE VALORES (Alineados a la derecha)
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