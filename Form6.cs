using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace bar
{

    public partial class Form6 : Form
    {
        public MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            
            return conn;
        }
        public Form6()
        {
            InitializeComponent();



        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // If it has, don't open file dialog again
                return;
            }

            // If PictureBox doesn't have an image, proceed with opening file dialog
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (.jpg, *.jpeg, *.png, *.gif, *.bmp)|.jpg; *.jpeg; *.png; *.gif; *.bmp";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Read the selected image from the file
                    string imagePath = openFileDialog1.FileName;
                    Image selectedImage = Image.FromFile(imagePath);

                    // Display the image in PictureBox (pic1)
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage; // Adjust the image size to fit the PictureBox
                    pictureBox1.Image = selectedImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void showdataGridView1()
        {
            MySqlConnection conn = databaseConnection();
            DataSet ds = new DataSet();

            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM menu";

            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(ds);

            dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            showdataGridView1();

            comboBox1.Items.Add("อาหาร");
            comboBox1.Items.Add("เครื่องดื่ม");
            comboBox1.Items.Add("Cocktail");
            comboBox1.Items.Add("อื่น ๆ");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["ID"].FormattedValue.ToString();
            txtnm.Text = dataGridView1.Rows[e.RowIndex].Cells["name"].FormattedValue.ToString();
            comboBox1.SelectedItem = dataGridView1.Rows[e.RowIndex].Cells["categories"].FormattedValue.ToString();
            txtp.Text = dataGridView1.Rows[e.RowIndex].Cells["price"].FormattedValue.ToString();
            txtdesc.Text = dataGridView1.Rows[e.RowIndex].Cells["Description"].FormattedValue.ToString();   

            if (dataGridView1.Rows[e.RowIndex].Cells["picture"].Value != DBNull.Value)
            {
                byte[] imgData = (byte[])dataGridView1.Rows[e.RowIndex].Cells["picture"].Value;
                MemoryStream ms = new MemoryStream(imgData);
                pictureBox1.Image = Image.FromStream(ms);
            }
            else
            {
                // ให้ทำอย่างไรก็ตามหากไม่มีข้อมูลภาพในฐานข้อมูล
                // เช่น กำหนดภาพเริ่มต้นหรือลบภาพที่มีอยู่
                pictureBox1.Image = null; // เพื่อล้างภาพที่อาจจะมีอยู่
            }

            txtq.Text = dataGridView1.Rows[e.RowIndex].Cells["quantity"].FormattedValue.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form8 f = new Form8();
            f.Show();
        }

        private void txtdesc_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่าราคาติดลบหรือไม่
            if (decimal.TryParse(txtp.Text, out decimal price) && price < 0)
            {
                MessageBox.Show("ราคาไม่สามารถเป็นค่าติดลบได้", "ข้อผิดพลาด");
                return;
            }

            // ตรวจสอบว่าจำนวนติดลบหรือไม่
            if (int.TryParse(txtq.Text, out int quantity) && quantity < 0)
            {
                MessageBox.Show("จำนวนไม่สามารถเป็นค่าติดลบได้", "ข้อผิดพลาด");
                return;
            }

            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            int editId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);

            // สร้างคำสั่ง SQL ที่ใช้ในการแก้ไขข้อมูลรวมถึงรูปภาพ
            String sql = "UPDATE menu SET name = @name, categories = @categories, price = @price, description = @description, quantity = @quantity, picture = @picture WHERE id = @id";

            // เชื่อมต่อฐานข้อมูล
            using (MySqlConnection conn = databaseConnection())
            {
                // สร้าง MySqlCommand และกำหนดค่าพารามิเตอร์
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", editId);
                    cmd.Parameters.AddWithValue("@name", txtnm.Text);
                    cmd.Parameters.AddWithValue("@categories", comboBox1.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@price", price); // ใช้ค่าที่ตรวจสอบแล้ว
                    cmd.Parameters.AddWithValue("@description", txtdesc.Text);
                    cmd.Parameters.AddWithValue("@quantity", quantity); // ใช้ค่าที่ตรวจสอบแล้ว

                    // ตรวจสอบว่ามีการอัปเดตรูปภาพหรือไม่
                    if (pictureBox1.Image != null)
                    {
                        // แปลงรูปภาพใน PictureBox ให้เป็น byte[] เพื่ออัปเดตในฐานข้อมูล
                        byte[] photoBytes;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                            photoBytes = ms.ToArray();
                        }

                        // กำหนดพารามิเตอร์สำหรับรูปภาพ
                        cmd.Parameters.AddWithValue("@picture", photoBytes);
                    }
                    else
                    {
                        // กรณีไม่มีการเปลี่ยนรูปภาพ ก็สามารถเก็บค่ารูปเดิมไว้ได้
                        byte[] currentImage = (byte[])dataGridView1.Rows[selectedRow].Cells["picture"].Value;
                        cmd.Parameters.AddWithValue("@picture", currentImage);
                    }

                    // เปิดการเชื่อมต่อฐานข้อมูลและ execute คำสั่ง SQL
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    conn.Close();

                    // ตรวจสอบการแก้ไขข้อมูลและแสดงข้อความแจ้งเตือนให้ผู้ใช้ทราบ
                    if (rows > 0)
                    {
                        MessageBox.Show("แก้ไขข้อมูลสำเร็จ");
                        showdataGridView1(); // เรียกใช้ฟังก์ชันเพื่อโหลดข้อมูลใหม่ลงในตาราง
                    }
                }
            }
        }



        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // ตรวจสอบว่าผู้ใช้ได้เลือกหมวดหมู่หรือไม่
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("กรุณาเลือกหมวดหมู่", "ข้อผิดพลาด");
                return;
            }

            // ตรวจสอบค่าราคาและจำนวนว่าติดลบหรือไม่
            if (decimal.TryParse(txtp.Text, out decimal price) && price < 0)
            {
                MessageBox.Show("ราคาไม่สามารถเป็นค่าติดลบได้", "ข้อผิดพลาด");
                return;
            }

            if (int.TryParse(txtq.Text, out int quantity) && quantity < 0)
            {
                MessageBox.Show("จำนวนไม่สามารถเป็นค่าติดลบได้", "ข้อผิดพลาด");
                return;
            }

            // สร้างคำสั่ง SQL สำหรับการเพิ่มข้อมูล
            string sql = "INSERT INTO menu(name, categories, price, description, picture, quantity) VALUES (@name, @categories, @price, @description, @picture, @quantity)";

            using (MySqlConnection con = new MySqlConnection("datasource=127.0.0.1;port=3306;username=root;password=;database=registers;"))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    // เพิ่มพารามิเตอร์และกำหนดค่า
                    cmd.Parameters.AddWithValue("@name", txtnm.Text);
                    cmd.Parameters.AddWithValue("@categories", comboBox1.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@price", price); // ใช้ค่าที่ตรวจสอบแล้ว
                    cmd.Parameters.AddWithValue("@description", txtdesc.Text);

                    // แปลงภาพใน PictureBox เป็น byte[] เพื่อเพิ่มในฐานข้อมูล
                    byte[] photoBytes;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pictureBox1.Image.Save(ms, ImageFormat.Jpeg);
                        photoBytes = ms.ToArray();
                    }
                    cmd.Parameters.AddWithValue("@picture", photoBytes);
                    cmd.Parameters.AddWithValue("@quantity", quantity); // ใช้ค่าที่ตรวจสอบแล้ว

                    // เปิดการเชื่อมต่อและรันคำสั่ง
                    con.Open();
                    int rowAffected = cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            // แสดงข้อความว่าการเพิ่มข้อมูลสำเร็จ
            MessageBox.Show("เพิ่มข้อมูลสำเร็จ", "Butter");
            showdataGridView1(); // โหลดข้อมูลใหม่เพื่อแสดงใน DataGridView
        }


        private void guna2Button4_Click(object sender, EventArgs e)
        {
            int selectedRow = dataGridView1.CurrentCell.RowIndex;
            int deleteId = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells["id"].Value);

            MySqlConnection conn = databaseConnection();
            String sql = "DELETE FROM menu WHERE id = '" + deleteId + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            conn.Close();
            if (rows > 0)
            {
                MessageBox.Show("ลบข้อมูลสำเร็จ");
                showdataGridView1();
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (.jpg, *.jpeg, *.png, *.gif, *.bmp)|.jpg; *.jpeg; *.png; *.gif; *.bmp";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // อ่านรูปภาพจากไฟล์ที่เลือก
                    string imagePath = openFileDialog1.FileName;
                    Image selectedImage = Image.FromFile(imagePath);

                    // แสดงรูปภาพใน PictureBox (pic1)
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // ปรับขนาดรูปภาพให้เต็มพื้นที่ PictureBox
                    pictureBox1.Image = selectedImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void textBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form8 f = new Form8();
            f.Show();

        }
    }
}
