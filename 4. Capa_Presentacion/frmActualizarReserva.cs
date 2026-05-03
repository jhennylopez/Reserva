using System;
using System.Collections.Generic;
using System.Data;
using System.Linq; 
using System.Windows.Forms;
using Capa_Entidades;
using Capa_Logica;
using Microsoft.VisualBasic;

namespace _4.Capa_Presentacion
{
    public partial class frmActualizarReserva : Form
    {
        private int idReservaActual = 0;

        public frmActualizarReserva()
        {
            InitializeComponent();
        }

        private void frmActualizarReserva_Load(object sender, EventArgs e)
        {
            ConfigurarControles();
            CargarListasReferenciales();
            EstadoInicial(true);
        }

        private void ConfigurarControles()
        {
            comboBox2.Items.AddRange(new string[] { "Completa", "Compartida", "Privada" });
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            textBox2.ReadOnly = true; 
        }

        private void EstadoInicial(bool bloqueado)
        {
           
            comboBox1.Enabled = !bloqueado;
            comboBox2.Enabled = !bloqueado;
            dateTimePicker1.Enabled = !bloqueado;
            dateTimePicker2.Enabled = !bloqueado;
            textBox1.Enabled = !bloqueado;
            button2.Enabled = !bloqueado; 

            
            button1.Enabled = bloqueado; 
        }

        private void CargarListasReferenciales()
        {
            try
            {
                
                clsPuenteAlojamiento pAloj = new clsPuenteAlojamiento();
                var lista = pAloj.BuscarAlojamientosPorHuespedes(0);

                comboBox1.Items.Clear();
                listBox1.Items.Clear();
                foreach (var a in lista)
                {
                    comboBox1.Items.Add(a.Id_alojamiento);
                    listBox1.Items.Add($"ID: {a.Id_alojamiento} | Máx: {a.Max_huespedes} | {a.Descripcion}");
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar datos: " + ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string input = Microsoft.VisualBasic.Interaction.InputBox("Ingrese el ID de la Reserva:", "Actualizar");
            if (int.TryParse(input, out int id))
            {
                clsPuenteReserva pRes = new clsPuenteReserva();
                DataTable dt = pRes.BuscarReservaPorId(id);

                if (dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    idReservaActual = id;
                    comboBox1.SelectedItem = Convert.ToInt32(r["Id_alojamiento"]);
                    comboBox2.SelectedItem = r["tipo"].ToString();
                    dateTimePicker1.Value = Convert.ToDateTime(r["fecha_ingreso"]);
                    dateTimePicker2.Value = Convert.ToDateTime(r["fecha_salida"]);
                    textBox1.Text = r["numero_personas"].ToString();
                    textBox2.Text = r["ci"].ToString();

                    EstadoInicial(false);
                }
                else { MessageBox.Show("Reserva no encontrada."); }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Acción Actualizar
            try
            {
                if (dateTimePicker2.Value <= dateTimePicker1.Value)
                {
                    MessageBox.Show("La fecha de salida debe ser posterior a la de ingreso.");
                    return;
                }

                int numPer = int.Parse(textBox1.Text);
                int idAloj = int.Parse(comboBox1.SelectedItem.ToString());

                // Validar capacidad
                clsPuenteAlojamiento pAloj = new clsPuenteAlojamiento();
                var aloj = pAloj.BuscarAlojamientosPorHuespedes(0).FirstOrDefault(a => a.Id_alojamiento == idAloj);

                if (aloj != null && numPer > aloj.Max_huespedes)
                {
                    MessageBox.Show($"Capacidad excedida. Este alojamiento solo permite {aloj.Max_huespedes} personas.");
                    return;
                }

                clsReserva ent = new clsReserva
                {
                    Id_reserva = idReservaActual,
                    Fecha_ingreso = dateTimePicker1.Value,
                    Fecha_salida = dateTimePicker2.Value,
                    Numero_personas = numPer,
                    Tipo = comboBox2.SelectedItem.ToString(),
                    Id_alojamiento = idAloj
                };

                clsPuenteReserva pRes = new clsPuenteReserva();
                pRes.ActualizarReserva(ent);

                MessageBox.Show("Reserva actualizada correctamente.");
                EstadoInicial(true);
                textBox1.Clear(); textBox2.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }
    }
}