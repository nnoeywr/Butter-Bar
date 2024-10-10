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

namespace bar
{
    public partial class Form10 : Form
    {
        public Form10()
        {
            InitializeComponent();
        }
        private void Form10_Load(object sender, EventArgs e)
        {
            comboBox.Items.Add("Daily");
            comboBox.Items.Add("Monthly");
            comboBox.Items.Add("Annual");
            for (int i = 1; i <= 31; i++)
            {
                comboBox1.Items.Add(i);
            }

            for (int i = 1; i <= 12; i++)
            {
                comboBox2.Items.Add(i);
            }
            for (int i = 2024; i <= 2030; i++)
            {
                comboBox3.Items.Add(i);
            }
        }
        private void ShowSalesSummary(int day, int month, int year)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            string query = "SELECT * FROM history_order WHERE DAY(date)=@day AND MONTH(date) = @month AND YEAR(date) = @year";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@day", day);
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable originalTable = new DataTable();
                    adapter.Fill(originalTable);

                    // ตรวจสอบว่าคอลัมน์ที่ต้องการมีอยู่ใน DataTable
                    if (!originalTable.Columns.Contains("name") || !originalTable.Columns.Contains("quantity") || !originalTable.Columns.Contains("total"))
                    {
                        MessageBox.Show("คอลัมน์ 'name' หรือ 'total' ไม่พบในตารางข้อมูล", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // สร้าง DataTable ใหม่ที่มีเฉพาะคอลัมน์ที่ต้องการ
                    DataTable filteredTable = new DataTable();
                    filteredTable.Columns.Add("name", typeof(string));
                    filteredTable.Columns.Add("quantity", typeof(int));
                    filteredTable.Columns.Add("total", typeof(decimal));

                    // คัดลอกข้อมูลจาก DataTable เดิมไปยัง DataTable ใหม่
                    foreach (DataRow row in originalTable.Rows)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        newRow["name"] = row["name"];
                        newRow["quantity"] = row["quantity"];
                        newRow["total"] = row["total"];
                        filteredTable.Rows.Add(newRow);
                    }

                    // ตั้งค่า DataSource ของ DataGridView เป็น DataTable ใหม่
                    dataGridView1.DataSource = filteredTable;

                    // คำนวณยอดขายรวม
                    decimal totalSales = 0;
                    foreach (DataRow row in filteredTable.Rows)
                    {
                        // ตรวจสอบค่าว่างและแปลงข้อมูลเป็น decimal
                        if (!DBNull.Value.Equals(row["total"]))
                        {
                            if (decimal.TryParse(row["total"].ToString(), out decimal saletotal))
                            {
                                totalSales += saletotal;
                            }
                            else
                            {
                                MessageBox.Show("ข้อมูลในคอลัมน์ 'total' ไม่สามารถแปลงเป็น decimal ได้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    // แสดงยอดขายรวม
                    label1.Text = $"Total income {day}/{month}/{year} : {totalSales:C} Baht ";
                    ShowTopItems(day, month, year);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowTopItems(int day, int month, int year)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            string query = @"                    SELECT name, SUM(quantity) AS total_quantity 
                            FROM history_order 
                            WHERE DAY(date)=@day AND MONTH(date) = @month AND YEAR(date) = @year
                            GROUP BY name 
                            ORDER BY total_quantity DESC 
                            LIMIT 3";
        

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@day", day);
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable topItemsTable = new DataTable();
                    adapter.Fill(topItemsTable);

                    // ล้างข้อมูลก่อนหน้า
                    label2.Text = "สินค้าขายดี:\n";

                    // ตรวจสอบว่ามีแถวในผลลัพธ์หรือไม่
                    if (topItemsTable.Rows.Count > 0)
                    {
                        StringBuilder topItemsText = new StringBuilder();
                        topItemsText.AppendLine("Best Seller :");
                        foreach (DataRow row in topItemsTable.Rows)
                        {
                            string name = row["name"].ToString();
                            int quantity = Convert.ToInt32(row["total_quantity"]);
                            topItemsText.AppendLine($"{name}: {quantity}");
                        }
                        label2.Text = topItemsText.ToString();
                    }
                    else
                    {
                        label2.Text = "ไม่มีข้อมูลเมนูในเดือนที่เลือก";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ข้อผิดพลาด: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ShowMonthlySalesSummary(int month, int year)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            string query = "SELECT * FROM history_order WHERE MONTH(date) = @month AND YEAR(date) = @year";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable originalTable = new DataTable();
                    adapter.Fill(originalTable);

                    // ตรวจสอบว่าคอลัมน์ที่ต้องการมีอยู่ใน DataTable
                    if (!originalTable.Columns.Contains("name") || !originalTable.Columns.Contains("quantity") || !originalTable.Columns.Contains("total"))
                    {
                        MessageBox.Show("คอลัมน์ 'name' หรือ 'total' ไม่พบในตารางข้อมูล", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // สร้าง DataTable ใหม่ที่มีเฉพาะคอลัมน์ที่ต้องการ
                    DataTable filteredTable = new DataTable();
                    filteredTable.Columns.Add("name", typeof(string));
                    filteredTable.Columns.Add("quantity", typeof(int));
                    filteredTable.Columns.Add("total", typeof(decimal));

                    // คัดลอกข้อมูลจาก DataTable เดิมไปยัง DataTable ใหม่
                    foreach (DataRow row in originalTable.Rows)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        newRow["name"] = row["name"];
                        newRow["quantity"] = row["quantity"];
                        newRow["total"] = row["total"];
                        filteredTable.Rows.Add(newRow);
                    }

                    // ตั้งค่า DataSource ของ DataGridView เป็น DataTable ใหม่
                    dataGridView1.DataSource = filteredTable;

                    // คำนวณยอดขายรวม
                    decimal totalSales = 0;
                    foreach (DataRow row in filteredTable.Rows)
                    {
                        // ตรวจสอบค่าว่างและแปลงข้อมูลเป็น decimal
                        if (!DBNull.Value.Equals(row["total"]))
                        {
                            if (decimal.TryParse(row["total"].ToString(), out decimal saletotal))
                            {
                                totalSales += saletotal;
                            }
                            else
                            {
                                MessageBox.Show("ข้อมูลในคอลัมน์ 'total' ไม่สามารถแปลงเป็น decimal ได้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    // แสดงยอดขายรวม
                    label1.Text = $"Total income {month}/{year} : {totalSales:C} Baht ";
                    ShowTopItemsByMonth(month, year);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowTopItemsByMonth(int month, int year)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            string query = @"   SELECT name, SUM(quantity) AS total_quantity 
                                FROM history_order 
                                WHERE MONTH(date) = @month AND YEAR(date) = @year
                                GROUP BY name 
                                ORDER BY total_quantity DESC 
                                LIMIT 3";
        

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable topItemsTable = new DataTable();
                    adapter.Fill(topItemsTable);

                    // แสดงข้อมูล 3 อันดับเมนูที่ขายดีที่สุด
                    if (topItemsTable.Rows.Count > 0)
                    {
                        StringBuilder topItemsText = new StringBuilder();
                        topItemsText.AppendLine("Best Seller :");
                        foreach (DataRow row in topItemsTable.Rows)
                        {
                            string name = row["name"].ToString();
                            int quantity = Convert.ToInt32(row["total_quantity"]);
                            topItemsText.AppendLine($"{name}: {quantity}");
                        }
                        label2.Text = topItemsText.ToString();
                    }
                    else
                    {
                        label2.Text = "ไม่มีข้อมูลเมนูในเดือนที่เลือก";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ข้อผิดพลาด: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowSalesSummary(int year)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            string query = "SELECT name, quantity, total FROM history_order WHERE YEAR(date) = @year";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@year", year);

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable originalTable = new DataTable();
                    adapter.Fill(originalTable);

                    // ตรวจสอบว่าคอลัมน์ที่ต้องการมีอยู่ใน DataTable
                    if (!originalTable.Columns.Contains("name") || !originalTable.Columns.Contains("quantity") || !originalTable.Columns.Contains("total"))
                    {
                        MessageBox.Show("คอลัมน์ 'name' หรือ 'total' ไม่พบในตารางข้อมูล", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // สร้าง DataTable ใหม่ที่มีเฉพาะคอลัมน์ที่ต้องการ
                    DataTable filteredTable = new DataTable();
                    filteredTable.Columns.Add("name", typeof(string));
                    filteredTable.Columns.Add("quantity", typeof(int));
                    filteredTable.Columns.Add("total", typeof(decimal));

                    // คัดลอกข้อมูลจาก DataTable เดิมไปยัง DataTable ใหม่
                    foreach (DataRow row in originalTable.Rows)
                    {
                        DataRow newRow = filteredTable.NewRow();
                        newRow["name"] = row["name"];
                        newRow["quantity"] = row["quantity"];
                        newRow["total"] = row["total"];
                        filteredTable.Rows.Add(newRow);
                    }

                    // ตั้งค่า DataSource ของ DataGridView เป็น DataTable ใหม่
                    dataGridView1.DataSource = filteredTable;

                    // คำนวณยอดขายรวม
                    decimal totalSales = 0;
                    foreach (DataRow row in filteredTable.Rows)
                    {
                        // ตรวจสอบค่าว่างและแปลงข้อมูลเป็น decimal
                        if (!DBNull.Value.Equals(row["total"]))
                        {
                            if (decimal.TryParse(row["total"].ToString(), out decimal saletotal))
                            {
                                totalSales += saletotal;
                            }
                            else
                            {
                                MessageBox.Show("ข้อมูลในคอลัมน์ 'total' ไม่สามารถแปลงเป็น decimal ได้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    // แสดงยอดขายรวม
                    label1.Text = $"รายได้ทั้งหมดของปี {year} คือ {totalSales:C} บาท";

                    // เรียกใช้ฟังก์ชันเพื่อแสดง 3 อันดับเมนูที่ขายดีที่สุด
                    ShowTopItems(year);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowTopItems(int year)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            string query = @"SELECT name, SUM(quantity) AS total_quantity 
                            FROM history_order 
                            WHERE YEAR(date) = @year
                            GROUP BY name 
                            ORDER BY total_quantity DESC 
                            LIMIT 3";
   
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@year", year);

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable topItemsTable = new DataTable();
                    adapter.Fill(topItemsTable);

                    // Clear previous content
                    label2.Text = "Best Seller:\n";

                    // Display each item in a new line
                    if (topItemsTable.Rows.Count > 0)
                    {
                        StringBuilder topItemsText = new StringBuilder();
                        topItemsText.AppendLine("Best Seller :");
                        foreach (DataRow row in topItemsTable.Rows)
                        {
                            string name = row["name"].ToString();
                            int quantity = Convert.ToInt32(row["total_quantity"]);
                            topItemsText.AppendLine($"{name}: {quantity}");
                        }
                        label2.Text = topItemsText.ToString();
                    }
                    else
                    {
                        label2.Text = "ไม่มีข้อมูลเมนูในเดือนที่เลือก";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


       
        

        

        private void button4_Click(object sender, EventArgs e)
        {
            
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (comboBox.SelectedItem == null)
            {
                MessageBox.Show("กรุณาเลือกประเภทการแสดงผล ", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedType = comboBox.SelectedItem.ToString();

            if (selectedType == "Daily")
            {
                if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || comboBox3.SelectedItem == null)
                {
                    MessageBox.Show("กรุณาเลือกวัน/เดือน/ปี ให้ครบถ้วน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int day = Convert.ToInt32(comboBox1.SelectedItem);
                int month = Convert.ToInt32(comboBox2.SelectedItem);
                int year = Convert.ToInt32(comboBox3.SelectedItem);
                ShowSalesSummary(day, month, year);
            }
            else if (selectedType == "Monthly")
            {
                if (comboBox2.SelectedItem == null || comboBox3.SelectedItem == null)
                {
                    MessageBox.Show("กรุณาเลือกเดือน/ปี ให้ครบถ้วน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int month = Convert.ToInt32(comboBox2.SelectedItem);
                int year = Convert.ToInt32(comboBox3.SelectedItem);
                ShowMonthlySalesSummary(month, year);
            }
            else if (selectedType == "Annual")
            {
                if ( comboBox3.SelectedItem == null)
                {
                    MessageBox.Show("กรุณาเลือกเดือน/ปี ให้ครบถ้วน", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int year = Convert.ToInt32(comboBox3.SelectedItem);
                ShowSalesSummary(year);
            }
        }
    

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form8 f = new Form8();
            f.Show();
        }
    }
}
