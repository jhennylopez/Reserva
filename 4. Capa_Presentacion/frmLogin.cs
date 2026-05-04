using System;
using System.Windows.Forms;
using Capa_Logica;

namespace _4.Capa_Presentacion
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = true; // Ocultar contraseña
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "RommyEc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsPuenteLogin puente = new clsPuenteLogin();

            if (puente.Ingresar(textBox1.Text, textBox2.Text))
            {
                // === LA MAGIA ESTÁ AQUÍ ===
                // 1. Le decimos a Program.cs que todo salió bien (DialogResult.OK)
                this.DialogResult = DialogResult.OK;

                // 2. CERRAMOS y destruimos el Login por completo (Ya no usamos Hide)
                this.Close();
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas.", "RommyEc | Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Clear();
                textBox2.Focus();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) textBox2.Focus();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) button1.PerformClick();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            frmRegistroClientecs registro = new frmRegistroClientecs();
            registro.ShowDialog();
        }
    }
}