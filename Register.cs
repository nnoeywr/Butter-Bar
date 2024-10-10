using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace bar
{
    public partial class Form2 : Form
    {

        public MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtpass.UseSystemPasswordChar = false;
            }
            else
            {
                txtpass.UseSystemPasswordChar = true;
            }
        }

        private void txtpass_TextChanged(object sender, EventArgs e)
        {
            txtpass.UseSystemPasswordChar=true;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string name = txtname.Text;
            string phone = txttel.Text;
            string user = txtuser.Text;
            string password = txtpass.Text;

            // ตรวจสอบข้อมูลให้ครบถ้วน
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบถ้วน", "Butter");
                if (string.IsNullOrEmpty(name)) txtname.Focus();
                else if (string.IsNullOrEmpty(phone)) txttel.Focus();
                else if (string.IsNullOrEmpty(user)) txtuser.Focus();
                else txtpass.Focus();
                return;
            }

            // ตรวจสอบเบอร์โทรศัพท์
            if (!Regex.IsMatch(phone, @"^\d{10}$"))
            {
                MessageBox.Show("เบอร์โทรศัพท์ต้องเป็นตัวเลขและมีขนาด 10 ตัวเท่านั้น", "Butter");
                txttel.Focus();
                return;
            }

            // ตรวจสอบชื่อผู้ใช้
            if (!Regex.IsMatch(user, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("ชื่อผู้ใช้ต้องเป็นภาษาอังกฤษและตัวเลขเท่านั้น", "Butter");
                txtuser.Focus();
                return;
            }

            // ตรวจสอบรหัสผ่าน
            if (password.Length < 8 || !Regex.IsMatch(password, @"[A-Z]") || !Regex.IsMatch(password, @"[a-z]") || !Regex.IsMatch(password, @"[\d]") || !Regex.IsMatch(password, @"[\W_]"))
            {
                MessageBox.Show("รหัสผ่านต้องมีอย่างน้อย 8 ตัวอักษร ประกอบด้วยอักษรพิมพ์ใหญ่, พิมพ์เล็ก, ตัวเลข และอักษรพิเศษ", "Butter");
                txtpass.Focus();
                return;
            }

            // ตรวจสอบการใช้ชื่อผู้ใช้ซ้ำ
            using (MySqlConnection conn = new MySqlConnection("datasource=127.0.0.1;port=3306;username=root;password=;database=registers;"))
            {
                try
                {
                    conn.Open();

                    // ตรวจสอบชื่อผู้ใช้ซ้ำ
                    string checkUserQuery = "SELECT COUNT(*) FROM reg WHERE username = @username";
                    using (MySqlCommand checkUserCmd = new MySqlCommand(checkUserQuery, conn))
                    {
                        checkUserCmd.Parameters.AddWithValue("@username", user);
                        int userCount = Convert.ToInt32(checkUserCmd.ExecuteScalar());

                        if (userCount > 0)
                        {
                            MessageBox.Show("ชื่อผู้ใช้นี้ถูกใช้งานแล้ว", "Butter");
                            txtuser.Focus();
                            return;
                        }
                    }

                    // เพิ่มข้อมูลลงฐานข้อมูล
                    string sql = "INSERT INTO reg(name, tel, username, password) VALUES (@name, @tel, @username, @password)";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@tel", phone);
                        cmd.Parameters.AddWithValue("@username", user);
                        cmd.Parameters.AddWithValue("@password", password);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Welcome to Butter Bar!", "Butter");
                            this.Hide();
                            Login f = new Login();
                            f.Show();
                        }
                        else
                        {
                            MessageBox.Show("Failed to insert data.", "Butter");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Butter");
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login f = new Login();
            f.Show();
        }
    }
}
