using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static bar.Mains;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace bar
{
    public partial class Form11 : Form
    {
        private string receivedUsername;
        public Form11()
        {
            InitializeComponent();
        }
        public Form11(string username)
        {
            InitializeComponent();
            receivedUsername = username;
            label7.Text = receivedUsername;
        }
        public MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string name = txtname.Text;
            string phone = txttel.Text;
            string password = txtpass.Text;

            // ตรวจสอบข้อมูลให้ครบถ้วน
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบถ้วน", "Butter");
                if (string.IsNullOrEmpty(name)) txtname.Focus();
                else if (string.IsNullOrEmpty(phone)) txttel.Focus();
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

            // ตรวจสอบรหัสผ่าน
            if (password.Length < 8 || !Regex.IsMatch(password, @"[A-Z]") || !Regex.IsMatch(password, @"[a-z]") || !Regex.IsMatch(password, @"[\d]") || !Regex.IsMatch(password, @"[\W_]"))
            {
                MessageBox.Show("รหัสผ่านต้องมีอย่างน้อย 8 ตัวอักษร ประกอบด้วยอักษรพิมพ์ใหญ่, พิมพ์เล็ก, ตัวเลข และอักษรพิเศษ", "Butter");
                txtpass.Focus();
                return;
            }

            // เชื่อมต่อฐานข้อมูลเพื่ออัปเดตข้อมูล
            String sql = "UPDATE reg SET name = @name, tel = @tel, password = @password WHERE username = @originalUsername";

            using (MySqlConnection conn = databaseConnection())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        // กำหนดค่าพารามิเตอร์สำหรับการอัปเดต
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@tel", phone);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@originalUsername", label7.Text); // ใช้ label7.Text สำหรับ username

                        conn.Open();
                        int rows = cmd.ExecuteNonQuery();
                        conn.Close();

                        // ตรวจสอบการแก้ไขข้อมูลและแสดงข้อความแจ้งเตือนให้ผู้ใช้ทราบ
                        if (rows > 0)
                        {
                            MessageBox.Show("แก้ไขข้อมูลสำเร็จ", "Butter");
                        }
                        else
                        {
                            MessageBox.Show("ไม่พบข้อมูลที่จะอัปเดต หรือไม่มีการเปลี่ยนแปลง", "Butter");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message, "Butter");
                }
            }
        }

        private void Form11_Load(object sender, EventArgs e)
        {
            txtpass.UseSystemPasswordChar = true; //เปิดมาซ่อนpassword
            label6.Enabled = false;
            label7.Enabled = false;

            label7.Text = receivedUsername;  // กำหนดค่าให้ label7 เป็น username ที่รับมา
            String sql = "SELECT id,name, tel, username, password FROM reg WHERE username = @username";

            using (MySqlConnection conn = databaseConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    // เพิ่มพารามิเตอร์สำหรับคำสั่ง SQL
                    cmd.Parameters.AddWithValue("@username", receivedUsername);

                    conn.Open();

                    // รันคำสั่งและอ่านข้อมูล
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            label6.Text = reader["id"].ToString();
                            // ดึงข้อมูลจาก reader และตั้งค่าให้กับคอนโทรลต่างๆ
                            txtname.Text = reader["name"].ToString();
                            txttel.Text = reader["tel"].ToString();
                            label7.Text = reader["username"].ToString();  // กำหนดให้ label7 เป็น username
                            txtpass.Text = reader["password"].ToString();
                        }
                    }

                    conn.Close();
                }
            }
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

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            foreach (Form form in Application.OpenForms)
            {
                if (form is Mains)
                {
                    form.Show();  // แสดง Form3 ที่ถูกซ่อนไว้
                    ((Mains)form).LoadData();  // เรียกฟังก์ชันโหลดข้อมูลใน Form3
                    break;
                }
            }
        }
    }
}