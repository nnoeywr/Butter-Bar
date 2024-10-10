using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace bar
{
    public partial class Summ : Form
    {
        public Summ()
        {
            InitializeComponent();
        }

        private void Summ_Load(object sender, EventArgs e)
        {
            // เติมข้อมูลใน ComboBox สำหรับการเลือกวัน, เดือน, ปี
            for (int i = 1; i <= 31; i++)
            {
                day.Items.Add(i);
            }
            for (int i = 1; i <= 12; i++)
            {
                month.Items.Add(i);
            }
            for (int i = 2024; i <= 2030; i++)
            {
                year.Items.Add(i);
            }

            // เติมข้อมูลใน ComboBox สำหรับการเลือกประเภทการดูยอดขาย
            comboBox1.Items.Add("รายวัน");
            comboBox1.Items.Add("รายเดือน");
            comboBox1.Items.Add("รายปี");
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = comboBox1.SelectedItem.ToString();

            // แสดง/ซ่อน ComboBox วัน, เดือน, ปี ตามการเลือกประเภท
            switch (selectedType)
            {
                case "รายวัน":
                    day.Enabled = true;
                    month.Enabled = true;
                    year.Enabled = true;
                    break;

                case "รายเดือน":
                    day.Enabled = false;
                    month.Enabled = true;
                    year.Enabled = true;
                    break;

                case "รายปี":
                    day.Enabled = false;
                    month.Enabled = false;
                    year.Enabled = true;
                    break;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && year.SelectedIndex >= 0)
            {
                string selectedType = comboBox1.SelectedItem.ToString();
                int selectedDay = (day.Enabled && day.SelectedIndex >= 0) ? Convert.ToInt32(day.SelectedItem) : 0;
                int selectedMonth = (month.Enabled && month.SelectedIndex >= 0) ? Convert.ToInt32(month.SelectedItem) : 0;
                int selectedYear = Convert.ToInt32(year.SelectedItem);

                // เรียกใช้เมธอดเพื่อแสดงสรุปยอดขายตามประเภทที่เลือก
                ShowSalesSummary(selectedDay, selectedMonth, selectedYear, selectedType);
            }
            else
            {
                MessageBox.Show("กรุณาเลือกข้อมูลที่ถูกต้อง", "ข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ShowSalesSummary(int day, int month, int year, string type)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;"; // แทนที่ด้วย connection string ของคุณ
            string query = "";

            // สร้าง query ตามประเภทที่เลือก
            if (type == "รายวัน")
            {
                query = "SELECT name, quantity, total, date FROM history_order WHERE DAY(date) = @day AND MONTH(date) = @month AND YEAR(date) = @year";
            }
            else if (type == "รายเดือน")
            {
                query = "SELECT name, quantity, total, date FROM history_order WHERE MONTH(date) = @month AND YEAR(date) = @year";
            }
            else if (type == "รายปี")
            {
                query = "SELECT name, quantity, total, date FROM history_order WHERE YEAR(date) = @year";
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                if (type == "รายวัน")
                {
                    command.Parameters.AddWithValue("@day", day);
                }
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("ไม่มีข้อมูลในช่วงเวลาที่เลือก", "ข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // ตั้งค่า DataSource ของ DataGridView เป็น DataTable
                        dataGridView1.AutoGenerateColumns = true; // ตั้งค่าสร้างคอลัมน์อัตโนมัติ
                        dataGridView1.DataSource = dataTable;

                        // คำนวณยอดขายรวม
                        decimal totalSales = 0;
                        foreach (DataRow row in dataTable.Rows)
                        {
                            if (decimal.TryParse(row["total"].ToString(), out decimal saleAmount))
                            {
                                totalSales += saleAmount;
                            }
                            else
                            {
                                MessageBox.Show("ข้อมูลในคอลัมน์ 'total' ไม่สามารถแปลงเป็นจำนวนทศนิยมได้", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        // แสดงยอดขายรวม
                        label2.Text = $"ยอดขายทั้งหมด {totalSales:C}";
                        ShowTopItems(day, month, year, type);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void ShowTopItems(int day, int month, int year, string type)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;"; // แทนที่ด้วย connection string ของคุณ
            string query = "";

            // สร้าง query ตามประเภทที่เลือก
            if (type == "รายวัน")
            {
                query = @"SELECT name, SUM(quantity) AS total_quantity FROM history_order 
                          WHERE DAY(date) = @day AND MONTH(date) = @month AND YEAR(date) = @year
                          GROUP BY name ORDER BY total_quantity DESC LIMIT 3";
            }
            else if (type == "รายเดือน")
            {
                query = @"SELECT name, SUM(quantity) AS total_quantity FROM history_order 
                          WHERE MONTH(date) = @month AND YEAR(date) = @year
                          GROUP BY name ORDER BY total_quantity DESC LIMIT 3";
            }
            else if (type == "รายปี")
            {
                query = @"SELECT name, SUM(quantity) AS total_quantity FROM history_order 
                          WHERE YEAR(date) = @year
                          GROUP BY name ORDER BY total_quantity DESC LIMIT 3";
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                if (type == "รายวัน")
                {
                    command.Parameters.AddWithValue("@day", day);
                }
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);

                try
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    DataTable topItemsTable = new DataTable();
                    adapter.Fill(topItemsTable);

                    // ตรวจสอบว่ามีคอลัมน์ที่ต้องการ
                    if (!topItemsTable.Columns.Contains("name") || !topItemsTable.Columns.Contains("total_quantity"))
                    {
                        MessageBox.Show("คอลัมน์ 'name' หรือ 'total_quantity' ไม่พบในตารางข้อมูล", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

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
                        label1.Text = topItemsText.ToString();
                    }
                    else
                    {
                        label1.Text = "ไม่มีข้อมูลเมนูในวันที่เลือก";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void year_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void day_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click_1(object sender, EventArgs e)
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
