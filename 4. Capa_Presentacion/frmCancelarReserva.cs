using System;
using System.Data;
using System.Windows.Forms;
using Capa_Logica;

namespace _4.Capa_Presentacion
{
    public partial class frmCancelarReserva : Form
    {
        
        public frmCancelarReserva()
        {
            InitializeComponent();
            textBox1.MaxLength = 10;
        }

        private void frmCancelarReserva_Load(object sender, EventArgs e)
        {
            CargarListaReservas();
        }

        private void CargarListaReservas()
        {
            try
            {
                listBox1.Items.Clear();
                clsPuenteReserva pRes = new clsPuenteReserva();
                DataTable dt = pRes.ConsultarTodasLasReservas();

                if (dt.Rows.Count == 0)
                {
                    listBox1.Items.Add("No hay reservaciones activas.");
                    button2.Enabled = false;
                }
                else
                {
                    button2.Enabled = true;
                    foreach (DataRow fila in dt.Rows)
                    {
                        string info = $"ID: {fila["Id_reserva"]} | {fila["Huésped"]} | {fila["Alojamiento"]} | In: {Convert.ToDateTime(fila["fecha_ingreso"]).ToShortDateString()}";
                        listBox1.Items.Add(info);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista: " + ex.Message);
            }
        }

        private void EjecutarCancelacion()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Por favor, ingrese el ID de la reservación a cancelar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int idAEliminar = int.Parse(textBox1.Text);

                // Confirmación de seguridad 
                DialogResult confirm = MessageBox.Show($"¿Está seguro que desea cancelar la reservación número {idAEliminar}?",
                    "Confirmar Cancelación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    clsPuenteReserva pRes = new clsPuenteReserva();
                    pRes.EliminarReserva(idAEliminar);

                    MessageBox.Show("La reservación ha sido cancelada exitosamente.", "Proceso Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox1.Clear();
                    CargarListaReservas(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cancelar. Verifique que el ID sea correcto. Detalle: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Botón Eliminar
            EjecutarCancelacion();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo permitimos números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // Ejecutar con Enter
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                EjecutarCancelacion();
            }
        }
        private void textBox1_TextChanged(object sender, KeyPressEventArgs e)
        {
        }
    }
}