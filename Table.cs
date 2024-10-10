using MySql.Data.MySqlClient;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static bar.Mains;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace bar
{
    public partial class Table : Form
    {

        private string receivedUsername;
        private Button lastSelectedButton = null;

        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        public Table()
        {
            InitializeComponent();
            txtUser.Text = Globalvariables.username;
            
        }

        
        public Table(string username)
        {
            InitializeComponent();
            receivedUsername = username;
            txtUser.Text = receivedUsername;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            InitializeButtons();
            CheckTableStatus();
            UpdateLabels();
            
            
        }

        

        private void InitializeButtons()
        {
            button1.Text = "B1";
            button2.Text = "B2";
            button3.Text = "B3";
            button4.Text = "T1";
            button5.Text = "T2";
            button6.Text = "T3";
            button7.Text = "T4";
            button10.Text = "T5";
            button11.Text = "T6";

            SetAllButtonsColor(Color.LawnGreen);
        }

        private void SetAllButtonsColor(Color color)
        {
            button1.BackColor = color;
            button2.BackColor = color;
            button3.BackColor = color;
            button4.BackColor = color;
            button5.BackColor = color;
            button6.BackColor = color;
            button7.BackColor = color;
            button10.BackColor = color;
            button11.BackColor = color;

        }

        private void CheckTableStatus()
        {
            try
            {
                MySqlConnection conn = databaseConnection();
                conn.Open();
                string query = "SELECT numtable, status FROM history WHERE date = @date";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@date", date.Value.Date);
                MySqlDataReader reader = cmd.ExecuteReader();

                SetAllButtonsColor(Color.LawnGreen);

                while (reader.Read())
                {
                    string tableNumber = reader.GetString("numtable");
                    int status = reader.GetInt32("status");

                    if (status == 1)
                    {
                        SetButtonColor(tableNumber, Color.Red);
                    }
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetButtonColor(string tableNumber, Color color)
        {
            switch (tableNumber)
            {
                case "B1":
                    button1.BackColor = color;
                    break;
                case "B2":
                    button2.BackColor = color;
                    break;
                case "B3":
                    button3.BackColor = color;
                    break;
                case "T1":
                    button4.BackColor = color;
                    break;
                case "T2":
                    button5.BackColor = color;
                    break;
                case "T3":
                    button6.BackColor = color;
                    break;
                case "T4":
                    button7.BackColor = color;
                    break;
                case "T5":
                    button10.BackColor = color;
                    break;
                case "T6":
                    button11.BackColor = color;
                    break;


            }
        }

        private void UpdateLabels()
        {
            // Update your labels here
        }


        private void HandleButtonClick(Button button, string tableNumber, string capacity)
        {
            if (button.BackColor == Color.LawnGreen)
            {
                if (MessageBox.Show("ต้องการเลือกโต๊ะใช่หรือไม่", "Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // ถ้ามีปุ่มที่เลือกก่อนหน้า เปลี่ยนสีเป็นสีเขียว
                    if (lastSelectedButton != null)
                    {
                        lastSelectedButton.BackColor = Color.LawnGreen;
                    }

                    UpdateTableStatus(tableNumber, true); // จองโต๊ะ
                    button.BackColor = Color.Red; // เปลี่ยนสีปุ่มใหม่เป็นสีแดง
                    lastSelectedButton = button; // เก็บปุ่มที่ถูกเลือกล่าสุด

                    txtTableNumber.Text = tableNumber;
                    label5.Text = capacity;
                    txtUser.Text = receivedUsername;
                    UpdateLabels();
                }
            }
            else if (button.BackColor == Color.Red)
            {
                if (receivedUsername == "admin")
                {
                    DialogResult result = MessageBox.Show("ต้องการปลดล็อกโต๊ะใช่หรือไม่", "Message", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        button.BackColor = Color.LawnGreen;
                        lastSelectedButton = null; // รีเซ็ตตัวแปรปุ่มที่เลือกล่าสุด
                        UpdateLabels();

                        try
                        {
                            MySqlConnection conn = databaseConnection();
                            conn.Open();
                            string query = "UPDATE history SET status = 0 WHERE numtable = @table AND date = @date";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@table", tableNumber);
                            cmd.Parameters.AddWithValue("@date", date.Value.Date);
                            cmd.ExecuteNonQuery();
                            conn.Close();

                            MessageBox.Show("สถานะโต๊ะได้ถูกรีเซ็ตเรียบร้อยแล้ว", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    // ผู้ใช้ทั่วไปเห็นว่าโต๊ะถูกจองแล้ว
                    MessageBox.Show("โต๊ะนี้ถูกจองแล้ว", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void date_ValueChanged(object sender, EventArgs e)
        {
            // รับค่าวันที่ปัจจุบัน
            DateTime currentDate = DateTime.Now.Date;

            // คำนวณค่าวันที่สูงสุด (2 สัปดาห์จากวันนี้)
            DateTime maxDate = currentDate.AddDays(14);

            // กำหนดวันที่ขั้นต่ำและสูงสุดให้กับ DateTimePicker
            if (receivedUsername == "admin")
            {
                // If the user is admin, allow selection of past dates
                date.MinDate = DateTime.MinValue; // No minimum date restriction
                date.MaxDate = maxDate; // Set max date as 2 weeks from today
            }
            else
            {
                // For regular users, restrict to today as minimum date
                date.MinDate = currentDate;
                date.MaxDate = maxDate;
            }

            DateTime selectedDate = date.Value.Date;

            // ตรวจสอบความถูกต้องของวันที่ที่เลือก
            if (receivedUsername != "admin")
            {
                if (selectedDate < currentDate)
                {
                    MessageBox.Show("ไม่สามารถเลือกวันที่ย้อนหลังได้", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    date.Value = currentDate; // รีเซ็ตเป็นวันที่ปัจจุบัน
                    return;
                }
            }

            CheckTableStatus();
        }


        private void UpdateTableStatus(string tableNumber, bool isReserved)
        {
            try
            {
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open();

                    string query;
                    if (isReserved)
                    {
                        // จองโต๊ะ
                        query = "UPDATE booked SET status = 1, username = @username, date = @date, time = CURTIME() WHERE numtable = @table AND date = @date";
                    }
                    else
                    {
                        // ปลดล็อกโต๊ะ
                        query = "UPDATE history SET status = 0, username = NULL, date = NULL, time = NULL WHERE numtable = @table AND date = @date";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", isReserved ? txtUser.Text : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@table", tableNumber);
                        cmd.Parameters.AddWithValue("@date", date.Value.Date);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            HandleButtonClick(button1, "B1", "ไม่เกิน 2 คน");

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            HandleButtonClick(button2, "B2", "ไม่เกิน 2 คน");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HandleButtonClick(button3, "B3", "ไม่เกิน 2 คน");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            HandleButtonClick(button5, "T2", "ไม่เกิน 4 คน");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            HandleButtonClick(button7, "T4", "ไม่เกิน 4 คน");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            HandleButtonClick(button6, "T3", "ไม่เกิน 4 คน");
        }
        private void button10_Click_2(object sender, EventArgs e)
        {
            HandleButtonClick(button10, "T5", "ไม่เกิน 8 คน");
        }
        private void button11_Click_2(object sender, EventArgs e)
        {
            HandleButtonClick(button11, "T6", "ไม่เกิน 8 คน");
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void textus_TextChanged(object sender, EventArgs e)
        {

        }

        

        

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = date.Value.Date;

            // รับค่าวันที่ปัจจุบัน
            DateTime currentDate = DateTime.Now.Date;

            // คำนวณค่าวันที่สูงสุด (2 สัปดาห์จากวันนี้)
            DateTime maxDate = currentDate.AddDays(14);

            // ตรวจสอบว่าค่าวันที่ที่เลือกมีความถูกต้องหรือไม่
            if (selectedDate < currentDate)
            {
                MessageBox.Show("ไม่สามารถจองวันที่ย้อนหลังได้", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedDate > maxDate)
            {
                MessageBox.Show("สามารถจองล่วงหน้าได้ไม่เกิน 2 สัปดาห์", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(txtUser.Text) && !string.IsNullOrEmpty(txtTableNumber.Text))
            {
                try
                {
                    MySqlConnection conn = databaseConnection();
                    conn.Open();
                    string sql = "INSERT INTO booked (numtable, username, status, date, time) VALUES (@table, @user, @status, @date, CURTIME())";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@table", txtTableNumber.Text);
                    cmd.Parameters.AddWithValue("@user", txtUser.Text);
                    cmd.Parameters.AddWithValue("@status", 1);
                    cmd.Parameters.AddWithValue("@date", selectedDate);

                    cmd.ExecuteNonQuery();
                    conn.Close();

                    string tableNumber = txtTableNumber.Text;
                    string username = txtUser.Text;

                    this.Hide(); // ซ่อน Form ปัจจุบัน

                    // ส่งหมายเลขโต๊ะและผู้ใช้ไปยัง Form7
                    Form7 f7 = new Form7(tableNumber, username);
                    f7.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void button4_Click(object sender, EventArgs e)
        {
            HandleButtonClick(button4, "T1", "ไม่เกิน 4 คน");
        }

        private void guna2CircleButton2_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text;

            // ตรวจสอบว่ามีการกำหนดค่า receivedUsername หรือไม่
            if (string.IsNullOrEmpty(receivedUsername))
            {
                MessageBox.Show("Username ไม่ถูกต้อง", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (receivedUsername == "admin")
            {
                // ส่งค่า username เมื่อเปิด Form8
                this.Hide();
                Form8 form8 = new Form8(receivedUsername); // ส่งค่า receivedUsername
                form8.Show();
            }
            else
            {
                // ส่งค่า username เมื่อเปิด Form3
                this.Hide();
                Mains form3 = new Mains(username); // ส่งค่า username ไปยัง Form3
                form3.Show();
            }
        }



    }
}
