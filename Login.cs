using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static bar.Mains;


namespace bar
{
    public partial class Login : Form
    {
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        private void showmewreg()
        {
           

        }
        public Login()
        {
            InitializeComponent();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtPass.UseSystemPasswordChar = false;
            }
            else
            {
                txtPass.UseSystemPasswordChar = true;
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = true; 

        }

        private void txtUser_Enter(object sender, EventArgs e)
        {
            if (txtUser.Text == "Username")
            {
                txtUser.Text = "";
                txtUser.ForeColor = Color.Black;
            }
        }

        private void txtUser_Leave(object sender, EventArgs e)
        {
            if (txtUser.Text == "") 
            {
                txtUser.Text = "Username";
                txtUser.ForeColor = Color.Silver;

            }
        }

        private void txtPass_Enter(object sender, EventArgs e)
        {
            if (txtPass.Text == "Password")
            {
                txtPass.Text = "";
                txtPass.ForeColor = Color.Black;
            }
        }

        private void txtPass_Leave(object sender, EventArgs e)
        {
            if (txtPass.Text == "")
            {
                txtPass.Text = "Password";
                txtPass.ForeColor = Color.Silver;

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("คุณต้องการออกจากโปรแกรมใช่หรือไม่", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                this.Show();
            }
        }

        private void guna2Chip1_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2Chip2_Click(object sender, EventArgs e)
        {
           
        }
        //log in
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string user = txtUser.Text;
            string password = txtPass.Text;
            

            if (user == "admin" && password == "123456")
            {
                MessageBox.Show("ยินดีต้อนรับค่ะ:)", "Message");
                this.Hide();
                Form8 f = new Form8(user);
                f.Show();
            }
            else if (txtUser.Text == "" && txtPass.Text == "")
            {
                MessageBox.Show("กรุณากรอกชื่อผู้ใช้และรหัสผ่านให้ถูกต้อง", "Message");
                txtUser.Focus();
            }
            else if (txtUser.Text == "")
            {
                MessageBox.Show("กรุณากรอกชื่อผู้ใช้", "Message");
                txtUser.Focus();
            }
            else if (txtPass.Text == "")
            {
                MessageBox.Show("กรุณากรอกรหัสผ่าน", "Message");
                txtPass.Focus();
            }
            else
            {
                string sql = "SELECT * FROM reg WHERE username='" +txtUser.Text+ "' AND password='" +txtPass.Text+ "'";


                MySqlConnection conn = new MySqlConnection("datasource=127.0.0.1;port=3306;username=root;password=;database=registers;");
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    MessageBox.Show("ยินดีต้อนรับค่ะ", "Message");


                    this.Hide();
                    Globalvariables.username = user;

                    // ใช้คอนสตรัคเตอร์ใหม่ที่รับ username
                    Mains f = new Mains(user);
                    f.Show();
                }
                else
                {
                    MessageBox.Show("กรุณากรอกชื่อผู้ใช้และรหัสผ่านให้ถูกต้อง", "Message");
                    txtUser.Focus();
                }
            }
        }


        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("คุณต้องการสมัครสมาชิกใช่หรือไม่", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Hide();
                Form2 f = new Form2();
                f.Show();
            }
            else
            {
                this.Show();
            }
        }
    }
}
