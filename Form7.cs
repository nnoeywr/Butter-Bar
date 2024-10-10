using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace bar
{
    public partial class Form7 : Form
    {
        
        function fn = new function();
        
        public MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            
            return conn;
        }
        private string tableNumber;
        private string username;

        public Form7(string tableNumber, string username)
        {
            InitializeComponent();
            this.tableNumber = tableNumber;
            this.username = username;
            txttable.Text = tableNumber;
            textBox1.Text = username;
            // ทำให้แน่ใจว่าได้ใช้ข้อมูลที่ได้รับ
            LoadData();
        }

        private void LoadData()
        {
           
        }

        public Form7(Table form4) : this()
        {
            
            this.form4 = form4;
        }
        public Form7()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form7_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
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

        private void ตะกร้าสินค้า_Enter(object sender, EventArgs e)
        { 

        }
        protected int n, total  = 0;
        private Table form4;
        private int GetAvailableQuantity(string name)//ดึงข้อมูลจำนวนสินค้า
        {
            int availableQuantity = 0;
            string connectionString = "Server=127.0.0.1;Port=3306;Database=registers;Uid=root;Pwd=;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    string query = "SELECT quantity FROM menu WHERE name = @name";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        connection.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                availableQuantity = Convert.ToInt32(reader["quantity"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาดในการเชื่อมต่อฐานข้อมูล: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return availableQuantity;
        }

        private void button1_Click(object sender, EventArgs e)//ปุ่มเพิ่มสินค้า
        {
            
            
            if (string.IsNullOrEmpty(txttotal.Text) || txttotal.Text == "0")
            {
                MessageBox.Show("กรุณาใส่จำนวนให้ถูกต้อง", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Check if the quantity entered is valid
            if (!int.TryParse(txtnum.Text, out int quantityToAdd) || quantityToAdd <= 0)
            {
                MessageBox.Show("กรุณาใส่จำนวนสินค้าที่ถูกต้อง", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Get available quantity from the database
            int availableQuantity = GetAvailableQuantity(txtname.Text);

            // Check if quantity exceeds available stock
            if (availableQuantity < quantityToAdd)
            {
                MessageBox.Show($"สินค้ามีจำนวนจำกัด (สูงสุด {availableQuantity} หน่วย)", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool found = false;
            int rowIndex = -1;

            // Loop to check if the item already exists in the basket
            for (int i = 0; i < basketGridView1.Rows.Count; i++)
            {
                if (basketGridView1.Rows[i].Cells[1].Value != null &&
                    basketGridView1.Rows[i].Cells[1].Value.ToString() == txtname.Text)
                {
                    found = true;
                    rowIndex = i;
                    break;
                }
            }

            if (found)
            {
                DataGridViewCell quantityCell = basketGridView1.Rows[rowIndex].Cells[3];
                DataGridViewCell totalCell = basketGridView1.Rows[rowIndex].Cells[4];

                if (quantityCell?.Value != null && totalCell?.Value != null)
                {
                    int currentQuantity = int.Parse(quantityCell.Value.ToString());
                    int currentTotal = int.Parse(totalCell.Value.ToString());

                    int newQuantity = currentQuantity + quantityToAdd;
                    int newTotal = currentTotal + int.Parse(txttotal.Text);

                    // Check again if the new quantity exceeds the available quantity
                    if (newQuantity <= availableQuantity)
                    {
                        quantityCell.Value = newQuantity.ToString();
                        totalCell.Value = newTotal.ToString();
                    }
                    else
                    {
                        MessageBox.Show($"สินค้ามีจำนวนจำกัด (สูงสุด {availableQuantity} หน่วย)", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Error: Quantity or total value is null.");
                }
            }
            else
            {
                // Add a new row to the basket grid view
                int n = basketGridView1.Rows.Add();
                basketGridView1.Rows[n].Cells[0].Value = label13.Text; // Product ID
                basketGridView1.Rows[n].Cells[1].Value = txtname.Text;  // Product name
                basketGridView1.Rows[n].Cells[2].Value = txtprice.Text; // Price
                basketGridView1.Rows[n].Cells[3].Value = txtnum.Text;   // Quantity
                basketGridView1.Rows[n].Cells[4].Value = txttotal.Text; // Total price
            }

            // Update total price in the basket
            UpdateTotal();
            UpdateStockCount(txtname.Text); // Update the remaining stock count in the database
        }


        private void UpdateTotal()
        {
            int sum = 0;
            foreach (DataGridViewRow row in basketGridView1.Rows)
            {
                if (row.Cells[4].Value != null)
                {
                    sum += int.Parse(row.Cells[4].Value.ToString());
                }
            }
            label8.Text =  sum.ToString("N2") +  "   บาท";
        }
        private void UpdateStockCount(string itemName)
        {
            int availableQuantity = GetAvailableQuantity(itemName);
            label2.Text = $"คงเหลือ: {availableQuantity}"; // อัปเดต label2 ให้แสดงจำนวนที่เหลือ
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)//combobox categories
        {
            listBox1.Items.Clear(); // เคลียร์รายการที่อยู่ใน listBox1 เพื่อไม่ให้รายการซ้ำกัน

            try
            {
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string categories = comboBox1.Text;
                        string query = "SELECT name FROM menu WHERE categories='" + categories + "'";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd); // เรียกใช้ตัวแปร da แต่ไม่ได้ใช้งาน
                        DataSet ds = new DataSet();
                        da.Fill(ds);

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            listBox1.Items.Add(row["name"].ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อกับฐานข้อมูลได้");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) //ลิสต์บ็อกแสดงสินค้า
        {
            listBox1.Items.Clear();

            try
            {
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        string categories = comboBox1.Text;
                        string query = "SELECT name FROM menu WHERE categories='" + categories + "' and name like '%" + textSearch.Text + "%'";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            listBox1.Items.Add(reader.GetString("name"));
                        }

                        reader.Close();
                    }
                    else
                    {
                        MessageBox.Show("ไม่สามารถเชื่อมต่อกับฐานข้อมูลได้");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
            }


        }
        private void showItemlist(String query)
        {
            listBox1.Items.Clear();
            DataSet ds = fn.getData(query);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listBox1.Items.Add(ds.Tables[0].Rows[i][0].ToString());
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)//ดึงข้อมูลสินค้าที่เลืก
        {
            txtnum.ResetText();
            txttotal.Clear();

            if (listBox1.SelectedItem != null)
            {
                string text = listBox1.GetItemText(listBox1.SelectedItem);
                txtname.Text = text;

                try
                {
                    using (MySqlConnection conn = databaseConnection())
                    {
                        conn.Open();
                        if (conn.State == ConnectionState.Open)
                        {
                            // ดึงราคาและ ID จากฐานข้อมูล
                            string query = "SELECT price, id, description FROM menu WHERE name=@name";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@name", text);
                            MySqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                // อ่านราคาและ ID จากผลลัพธ์ที่ได้จากฐานข้อมูล
                                txtprice.Text = reader["price"].ToString();
                                label13.Text = reader["id"].ToString();
                                label11.Text = reader["description"].ToString();
                            }
                            else
                            {
                                // หากไม่พบข้อมูลในฐานข้อมูล
                                txtprice.Text = "ไม่พบข้อมูลราคา";
                                textBox1.Text = "ไม่พบข้อมูล ID";
                            }

                            reader.Close();

                            // ดึงรูปภาพจากฐานข้อมูล
                            string imageQuery = "SELECT picture FROM menu WHERE name=@name";
                            MySqlCommand imageCmd = new MySqlCommand(imageQuery, conn);
                            imageCmd.Parameters.AddWithValue("@name", text);
                            byte[] imageData = (byte[])imageCmd.ExecuteScalar();
                            if (imageData != null)
                            {
                                using (MemoryStream ms = new MemoryStream(imageData))
                                {
                                    pictureBox1.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                pictureBox1.Image = null; // หากรูปภาพไม่พบ
                            }

                            // อัปเดตจำนวนสินค้าใน Label
                            UpdateStockCount(txtname.Text);
                        }
                        else
                        {
                            MessageBox.Show("ไม่สามารถเชื่อมต่อกับฐานข้อมูลได้");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการดึงข้อมูลราคาหรือรูปภาพ: " + ex.Message);
                }
            }
        }

        private void basketGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void basketGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected row
                DataGridViewRow selectedRow = basketGridView1.Rows[e.RowIndex];

                // Display data from the selected row in TextBoxes
                label13.Text = selectedRow.Cells[0].Value.ToString();
                txtname.Text = selectedRow.Cells[1].Value.ToString();
                txtprice.Text = selectedRow.Cells[2].Value.ToString();
                txtnum.Value = int.Parse(selectedRow.Cells[3].Value.ToString());
                txttotal.Text = selectedRow.Cells[4].Value.ToString();

                // อัปเดตจำนวนสินค้าใน Label
                UpdateStockCount(txtname.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (basketGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row index
                int rowIndex = basketGridView1.SelectedRows[0].Index;

                // Get the quantity and price from the selected row
                int quantity = int.Parse(basketGridView1.Rows[rowIndex].Cells[3].Value.ToString());
                int price = int.Parse(basketGridView1.Rows[rowIndex].Cells[2].Value.ToString());

                // Get the quantity to be subtracted
                int subtractedQuantity = (int)txtnum.Value;

                // Check if the quantity to be subtracted is greater than the current quantity
                if (subtractedQuantity > quantity)
                {
                    MessageBox.Show("Cannot subtract more than the current quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update the total and quantity based on the numeric control values
                quantity -= subtractedQuantity;
                int total = quantity * price;

                // Update the quantity and total in the DataGridView
                basketGridView1.Rows[rowIndex].Cells[3].Value = quantity;
                basketGridView1.Rows[rowIndex].Cells[4].Value = total;

                // Remove the item from the basket if quantity becomes zero
                if (quantity == 0)
                {
                    basketGridView1.Rows.RemoveAt(rowIndex);
                }

                // Update the total displayed
                UpdateTotal();
                UpdateStockCount(txtname.Text); // อัปเดตจำนวนที่เหลือ
            }
            else
            {
                MessageBox.Show("กรุณากรอกจำนวนสินค้าให้ถูกต้อง.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void txtnum_ValueChanged(object sender, EventArgs e)
        {
            Int64 num = Int64.Parse(txtnum.Value.ToString());
            Int64 price = Int64.Parse(txtprice.Text);
            txttotal.Text = (num * price).ToString();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            
        }

        private void button5_Click_1(object sender, EventArgs e)
        {

        }

        private void txtname_TextChanged(object sender, EventArgs e)
        {

        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Table f = new Table(username); // ส่งค่า username กลับไปยัง Table
            f.Show();
        }

        private void textSearch_Enter(object sender, EventArgs e)
        {
            if (textSearch.Text == "Search")
            {
                textSearch.Text = "";
                textSearch.ForeColor = Color.Black;
            }
        }

        private void textSearch_Leave(object sender, EventArgs e)
        {
            if (textSearch.Text == "")
            {
                textSearch.Text = "Search";
                textSearch.ForeColor = Color.Silver;

            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void txttable_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e) //จ่ายเงิน
        {
            try
            {
                using (MySqlConnection connection = databaseConnection())
                {
                    connection.Open();

                    int totalPrice = 0;

                    foreach (DataGridViewRow row in basketGridView1.Rows)
                    {
                        if (row.Cells[4].Value != null)
                        {
                            int id = int.Parse(row.Cells[0].Value.ToString()); // ตรวจสอบ ID ของสินค้า
                            string name = row.Cells[1].Value.ToString();
                            int price = int.Parse(row.Cells[2].Value.ToString());
                            int quantity = int.Parse(row.Cells[3].Value.ToString());
                            int total = int.Parse(row.Cells[4].Value.ToString());

                            string query = "INSERT INTO `order` (id, name, price, quantity, total, numtable, username) VALUES (@id, @name, @price, @quantity, @total, @numtable, @username)";
                            using (MySqlCommand command = new MySqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@id", id); // แทรก ID ที่ถูกต้อง
                                command.Parameters.AddWithValue("@name", name);
                                command.Parameters.AddWithValue("@price", price);
                                command.Parameters.AddWithValue("@quantity", quantity);
                                command.Parameters.AddWithValue("@total", total);
                                command.Parameters.AddWithValue("@numtable", tableNumber);
                                command.Parameters.AddWithValue("@username", username);

                                command.ExecuteNonQuery();
                            }


                            string updateQuery = "UPDATE menu SET quantity = quantity - @quantity WHERE name = @name";
                            using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@quantity", quantity);
                                updateCommand.Parameters.AddWithValue("@name", name);
                                updateCommand.ExecuteNonQuery();
                            }

                            totalPrice += total;
                        }
                    }

                    txttotal.Text = totalPrice.ToString();
                    string imageUrl = "https://promptpay.io/0981965003/" + totalPrice.ToString() + ".png";
                    this.Hide();
                    Form9 f = new Form9(this, imageUrl, username, tableNumber, totalPrice); // ส่ง totalPrice ไปยัง Form9
                    f.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

    }




}

   
        


        
    
