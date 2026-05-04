using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Capa_Entidades;
using Capa_Logica;
using System.Drawing.Printing; // Necesario para la impresión
using System.Drawing;          // Necesario para fuentes y gráficos

namespace _4.Capa_Presentacion
{
    public partial class frmActualizarReserva : Form
    {
        private int idReservaActual = 0;
        clsPuenteReserva pRes = new clsPuenteReserva();
        clsPuenteAlojamiento pAloj = new clsPuenteAlojamiento();

        public frmActualizarReserva()
        {
            InitializeComponent();

            // Suscribir eventos para cálculo de precio en tiempo real
            this.dateTimePicker1.ValueChanged += ActualizarCalculoEnPantalla;
            this.dateTimePicker2.ValueChanged += ActualizarCalculoEnPantalla;
            this.textBox3.TextChanged += ActualizarCalculoEnPantalla;
            this.textBox1.TextChanged += ActualizarCalculoEnPantalla;
            this.comboBox2.SelectedIndexChanged += ActualizarCalculoEnPantalla;
        }

        private void frmActualizarReserva_Load(object sender, EventArgs e)
        {
            ConfigurarControles();
            ConfigurarDataGridView();
            EstadoInicial(); // Inicia bloqueado esperando el ID del Alojamiento
        }

        private void ConfigurarControles()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new string[] { "Completa", "Compartida", "Privada" });
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox2.MaxLength = 10; // Cédula
            textBox3.ReadOnly = false;
        }

        private void ConfigurarDataGridView()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToDeleteRows = false;
        }

        // 1. ESTADO INICIAL (Bloqueado)
        private void EstadoInicial()
        {
            // Habilitamos SOLO la búsqueda por ID de Alojamiento
            textBox3.Enabled = true;
            textBox3.ReadOnly = false; // Confirmamos que se pueda escribir
            textBox3.Clear();
            button1.Enabled = true; // Botón Buscar

            // Bloqueamos el resto de campos
            textBox2.Enabled = false; textBox2.Clear(); // Cédula
            comboBox2.Enabled = false; comboBox2.SelectedIndex = -1;
            dateTimePicker1.Enabled = false; dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Enabled = false; dateTimePicker2.Value = DateTime.Today.AddDays(1);
            textBox1.Enabled = false; textBox1.Clear(); // Número de Personas

            button2.Enabled = false; // Botón Actualizar
            idReservaActual = 0;

            dataGridView1.DataSource = null; // Limpiamos la tabla

            // Forzamos el cursor a titilar dentro del textBox3
            textBox3.Select();
            textBox3.Focus();
        }

        // 2. PROCESO DE BÚSQUEDA POR ID DE ALOJAMIENTO
        private void ProcesarBusqueda()
        {
            if (!int.TryParse(textBox3.Text, out int idAlojBuscar))
            {
                MessageBox.Show("Ingrese un ID de alojamiento válido (número entero).", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            try
            {
                // Buscamos todas las reservas en la BD
                DataTable dtCompleto = pRes.BuscarReservaPorId(0);

                if (dtCompleto != null && dtCompleto.Rows.Count > 0)
                {
                    // Filtramos usando LINQ para traer solo las reservas de ESTE alojamiento
                    var reservasDelAlojamiento = dtCompleto.AsEnumerable()
                                             .Where(r => r.Field<int>("Id_alojamiento") == idAlojBuscar)
                                             .ToList();

                    if (reservasDelAlojamiento.Any())
                    {
                        // Llenar el Grid con las reservas de este alojamiento
                        dataGridView1.DataSource = reservasDelAlojamiento.Select(a => new {
                            ID = a.Field<int>("Id_reserva"),
                            Alojamiento = a.Field<int>("Id_alojamiento"),
                            Cedula = a.Field<string>("ci"), // Agregamos la cédula a la vista
                            Entrada = a.Field<DateTime>("fecha_ingreso").ToShortDateString(),
                            Salida = a.Field<DateTime>("fecha_salida").ToShortDateString(),
                            Tipo = a.Field<string>("tipo"),
                            Personas = a.Field<int>("numero_personas")
                        }).ToList();

                        // Cargar automáticamente la primera reserva encontrada en los TextBox
                        var r = reservasDelAlojamiento.First();
                        idReservaActual = r.Field<int>("Id_reserva");
                        textBox2.Text = r.Field<string>("ci");
                        comboBox2.SelectedItem = r.Field<string>("tipo");
                        dateTimePicker1.Value = r.Field<DateTime>("fecha_ingreso");
                        dateTimePicker2.Value = r.Field<DateTime>("fecha_salida");
                        textBox1.Text = r.Field<int>("numero_personas").ToString();

                        // Desbloquear campos para edición
                        textBox3.Enabled = false; // Bloqueamos el ID del alojamiento tras buscar
                        button1.Enabled = false;  // Bloqueamos el botón buscar

                        textBox2.Enabled = true;  // Permitimos cambiar la cédula si hubo un error al registrar
                        comboBox2.Enabled = true;
                        dateTimePicker1.Enabled = true;
                        dateTimePicker2.Enabled = true;
                        textBox1.Enabled = true;
                        button2.Enabled = true;   // Botón Actualizar habilitado

                        textBox2.Focus();
                    }
                    else
                    {
                        MessageBox.Show($"No se encontraron reservas registradas para el Alojamiento con ID: {idAlojBuscar}.", "Búsqueda sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox3.SelectAll();
                        textBox3.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Actualmente no existen reservas registradas en el sistema.", "Base de datos vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al realizar la búsqueda: {ex.Message}", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 3. ACTUALIZACIÓN DINÁMICA DE PRECIOS
        private void ActualizarCalculoEnPantalla(object sender, EventArgs e)
        {
            if (int.TryParse(textBox3.Text, out int idAloj) && int.TryParse(textBox1.Text, out int numPersonas) && comboBox2.SelectedIndex != -1)
            {
                var aloj = pAloj.BuscarAlojamiento(idAloj);
                if (aloj != null && dateTimePicker2.Value.Date > dateTimePicker1.Value.Date)
                {
                    decimal precioFinalPorNoche = CalcularPrecioAjustado(aloj.Precio_por_noche, numPersonas, comboBox2.Text);
                    decimal[] valores = pRes.CalcularValoresFactura(dateTimePicker1.Value, dateTimePicker2.Value, precioFinalPorNoche);

                    // Descomentar si tienes un label para el total (ej. label8)
                    // label8.Text = valores[3].ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-EC"));
                }
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

        // 4. PROCESO DE ACTUALIZACIÓN
        private void ProcesarActualizacion()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox1.Text) || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Todos los campos habilitados son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateTimePicker2.Value.Date <= dateTimePicker1.Value.Date)
            {
                MessageBox.Show("La fecha de salida debe ser posterior a la de ingreso.", "Validación de Fechas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int numPer = int.Parse(textBox1.Text);
                int idAloj = int.Parse(textBox3.Text); // Aunque esté deshabilitado, tomamos su valor
                string cedula = textBox2.Text.Trim();

                if (numPer <= 0)
                {
                    MessageBox.Show("El número de personas debe ser mayor a cero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 1. Validar que el huésped (Cédula) exista
                clsPuenteHuesped puenteH = new clsPuenteHuesped();
                var huesped = puenteH.ObtenerHuespedes().FirstOrDefault(h => h.Ci == cedula);

                if (huesped == null)
                {
                    MessageBox.Show($"No se encontró ningún huésped registrado con la cédula: {cedula}", "Huésped Inválido", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    textBox2.Focus();
                    return;
                }

                // 2. Validar capacidad de alojamiento
                var aloj = pAloj.BuscarAlojamiento(idAloj);
                if (aloj != null && numPer > aloj.Max_huespedes)
                {
                    MessageBox.Show($"Capacidad excedida. Este alojamiento solo permite {aloj.Max_huespedes} personas.", "Capacidad", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // 3. Generar entidad y guardar en BD
                clsReserva ent = new clsReserva
                {
                    Id_reserva = idReservaActual,
                    Fecha_ingreso = dateTimePicker1.Value.Date,
                    Fecha_salida = dateTimePicker2.Value.Date,
                    Numero_personas = numPer,
                    Tipo = comboBox2.SelectedItem.ToString(),
                    Id_alojamiento = idAloj,
                    Id_huesped = huesped.Id_huesped // Asignamos el ID real del huésped encontrado
                };

                pRes.ActualizarReserva(ent);

                // 4. Calcular los nuevos valores para la factura de la reserva actualizada
                decimal precioFinal = CalcularPrecioAjustado(aloj.Precio_por_noche, numPer, ent.Tipo);
                decimal[] v = pRes.CalcularValoresFactura(ent.Fecha_ingreso, ent.Fecha_salida, precioFinal);

                clsFactura facturaActualizada = new clsFactura
                {
                    Dias_ocupacion = (int)v[0],
                    Subtotal = v[1],
                    Impuestos = v[2],
                    Total = v[3]
                };

                // --- INTEGRACIÓN DE FACTURA: VENTANA EMERGENTE ---
                DialogResult respuesta = MessageBox.Show(
                    "✔ La reserva ha sido actualizada correctamente.\n\n¿Desea visualizar la factura actualizada ahora para imprimirla o guardarla en PDF?",
                    "Actualización Exitosa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    VisualizarFactura(ent, facturaActualizada, aloj, huesped);
                }

                EstadoInicial(); // Reseteamos la ventana para una nueva búsqueda
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- EVENTOS CLIC ---
        private void button1_Click(object sender, EventArgs e) // Buscar
        {
            ProcesarBusqueda();
        }

        private void button2_Click(object sender, EventArgs e) // Actualizar
        {
            ProcesarActualizacion();
        }

        // --- EVENTOS TECLADO ---
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e) // Búsqueda por ID Alojamiento con Enter
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarBusqueda();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) // Actualización rápida de número de huéspedes
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ProcesarActualizacion();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) // Cédula: Solo números
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        // --- MANEJO DEL DATAGRID ---
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Cambiar dinámicamente de reserva a editar con solo hacer clic en la tabla
            if (e.RowIndex >= 0 && button2.Enabled == true)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                idReservaActual = Convert.ToInt32(row.Cells["ID"].Value);
                textBox2.Text = row.Cells["Cedula"].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(row.Cells["Entrada"].Value);
                dateTimePicker2.Value = Convert.ToDateTime(row.Cells["Salida"].Value);
                comboBox2.SelectedItem = row.Cells["Tipo"].Value.ToString();
                textBox1.Text = row.Cells["Personas"].Value.ToString();
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
                Text = "Vista Previa de Factura Actualizada - RommyEc"
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
            g.DrawString("FACTURA DE RESERVACIÓN ACTUALIZADA", fontSubtitulo, brocha, 220, y);
            y += 40;

            // Imprimimos la fecha de emisión (hoy) y los datos del cliente
            g.DrawString($"Fecha de Emisión: {DateTime.Now.ToShortDateString()}", fontTexto, brocha, margenIzq, y);
            y += 25;
            g.DrawString($"Cédula Cliente: {huesped.Ci}", fontTexto, brocha, margenIzq, y);
            y += 40;

            // 3. DETALLES DE LA RESERVA
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