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
                MessageBox.Show("Por favor, complete todos los campos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsPuenteLogin puente = new clsPuenteLogin();
            if (puente.Ingresar(textBox1.Text, textBox2.Text))
            {
                // Si el login es exitoso, cerramos este form con OK para avisar a Form1
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas. Verifique su correo y contraseña.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}