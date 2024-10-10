using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace bar
{
    public partial class Mains : Form
    {

        public static class Globalvariables 
        {
            

            public static string username { get; set; }
        }
        
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        
        public Mains( string user)
        {
            InitializeComponent();
            Globalvariables.username = user;

            // แสดงชื่อผู้ใช้ใน label1
            label1.Text = Globalvariables.username;
            
            LoadData();
        }

        public void LoadData()
        {
            // โหลดข้อมูล เช่นจากฐานข้อมูล
            label1.Text = Globalvariables.username;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login f = new Login();
            f.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form7 f = new Form7();
            f.Show();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            label1.Text = Globalvariables.username;
            LoadData();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Table f1 = new Table(Globalvariables.username);
            f1.Show();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form11 f = new Form11(Globalvariables.username);
            f.Show();
        }

        private void guna2CircleButton1_Click_1(object sender, EventArgs e)
        {
            
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการออกจากระบบใช่หรือไม่", "Butter Bar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Hide();
                Login f = new Login();
                f.Show();
            }
            else
            {

            }
        }
    }
}
