using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace bar
{
    public partial class History : Form
    {
        public History()
        {
            InitializeComponent();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            guna2ComboBox1.Items.Add("ข้อมูลการจองโต๊ะ");
            guna2ComboBox1.Items.Add("ข้อมูลการสั่งอาหาร");
            guna2ComboBox1.Items.Add("ข้อมูลUser");
            guna2ComboBox1.SelectedIndexChanged += guna2ComboBox1_SelectedIndexChanged;
            
        }
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = guna2ComboBox1.SelectedItem.ToString();

            if (selectedItem == "ข้อมูลการจองโต๊ะ")
            {
                LoadTableBookingData();
            }
            else if (selectedItem == "ข้อมูลการสั่งอาหาร")
            {
                LoadOrderData();
            }
            else if (selectedItem == "ข้อมูลUser")
            {
                LoadUserData();
            }
        }

        private void LoadTableBookingData()
        {
            try
            {
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM history";
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable; // Assuming you have a DataGridView named dataGridView1
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadOrderData()
        {
            try
            {
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT name, quantity, total, numtable,username, date FROM history_order";
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable; // Assuming you have a DataGridView named dataGridView1
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUserData()
        {
            try
            {
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM reg";
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable; // Assuming you have a DataGridView named dataGridView1
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void guna2ComboBox1_SelectedIndexChanged1(object sender, EventArgs e)
        {
            // เรียกใช้ guna2Button1_Click เพื่อโหลดข้อมูลตามการเลือกใน ComboBox และกรองตาม username
            guna2Button1_Click(sender, e);
        }


        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim(); // Assuming you have a TextBox named txtSearch for input
            string selectedItem = guna2ComboBox1.SelectedItem.ToString();
            string query = "";

            try
            {
                string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // กำหนด SQL query ตามประเภทของข้อมูลที่เลือก
                    if (selectedItem == "ข้อมูลการจองโต๊ะ")
                    {
                        query = string.IsNullOrEmpty(searchText)
                            ? "SELECT * FROM history"
                            : "SELECT * FROM history WHERE username LIKE @searchText";
                    }
                    else if (selectedItem == "ข้อมูลการสั่งอาหาร")
                    {
                        query = string.IsNullOrEmpty(searchText)
                            ? "SELECT name, quantity, total, numtable,username, date FROM history_order"
                            : "SELECT name, quantity, total, numtable,username, date FROM history_order WHERE username LIKE @searchText";
                    }
                    else if (selectedItem == "ข้อมูลUser")
                    {
                        query = string.IsNullOrEmpty(searchText)
                            ? "SELECT * FROM reg"
                            : "SELECT * FROM reg WHERE username LIKE @searchText";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(searchText))
                        {
                            cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
                        }

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            dataGridView1.DataSource = dataTable;

                            // สำหรับข้อมูลการสั่งอาหาร คำนวณยอดขายรวม
                            if (selectedItem == "ข้อมูลการสั่งอาหาร")
                            {
                                decimal totalSales = 0;
                                foreach (DataRow row in dataTable.Rows)
                                {
                                    if (row["total"] != DBNull.Value) // ตรวจสอบว่าค่าที่ได้มาไม่เป็น null
                                    {
                                        totalSales += Convert.ToDecimal(row["total"]);
                                    }
                                }
                                labelTotalSales.Text = "ยอดขายรวม: " + totalSales.ToString("C2");
                            }
                            else
                            {
                                labelTotalSales.Text = ""; // Clear total sales label if not showing order data
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form8 f = new Form8();
            f.Show();
        }
    }
}

