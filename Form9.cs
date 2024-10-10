
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Net;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using ZXing;
using System.Net.Mail;
using System.Diagnostics;



namespace bar
{
    public partial class Form9 : Form
    {
        
        private string imageUrl;
        private DataTable basketDataTable; 
        private string numtable;      
        private string username;
        private int totalPrice;



        public Form9()
        {
            InitializeComponent();

        }
       
        
        public Form9(DataTable basketDataTable) : this()
        {
            this.basketDataTable = basketDataTable;

        }
        private MySqlConnection databaseConnection()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=registers;";
            MySqlConnection conn = new MySqlConnection(connectionString);
            return conn;
        }

        private Form7 form7;
        

        public Form9(Form7 form7, string imageUrl, string username, string numtable,int totalPrice)
        {
            InitializeComponent();
            this.form7 = form7 ?? throw new ArgumentNullException(nameof(form7));
            this.imageUrl = imageUrl;
            this.username = username;
            this.numtable = numtable;
            this.totalPrice = totalPrice;
        }
        public Form9(Form7 form7, DataTable basketDataTable) : this()
        {
            this.form7 = form7 ?? throw new ArgumentNullException(nameof(form7));
            this.basketDataTable = basketDataTable;
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            displayImage(imageUrl);
            LoadDataFromDatabase();
            label1.Text = $"รวมทั้งหมด {totalPrice.ToString("F2")} บาท";
        }
        private void displayImage(string imageUrl)
        {
            try
            {
                // Download the image
                WebClient webClient = new WebClient();
                byte[] imageData = webClient.DownloadData(imageUrl);
                webClient.Dispose();

                // Load image from byte array
                MemoryStream memoryStream = new MemoryStream(imageData);
                System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);


                // Resize image
                int newWidth = (int)(image.Width * 0.5);
                int newHeight = (int)(image.Height * 0.5);
                image = new Bitmap(image, newWidth, newHeight);

                // Display image in PictureBox
                pictureQr.Image = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        

        private void pictureQr_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }


        private async void button3_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("กรุณากรอกอีเมลล์ให้ถูกต้อง", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("ท่านทำการชำระเงินแล้วใช่หรือไม่", "Butter Bar", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await CreatePdf();  // เรียกใช้ await
                InsertHistoryAndDeleteBooked();

                try
                {
                    using (MySqlConnection conn = databaseConnection())
                    {
                        conn.Open();
                        string loadQuery = "SELECT * FROM `order`";
                        MySqlCommand loadCmd = new MySqlCommand(loadQuery, conn);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(loadCmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;

                        foreach (DataRow row in dt.Rows)
                        {
                            string insertQuery = "INSERT INTO `history_order` (name, price, quantity, total, numtable, username, date) VALUES (@name, @price, @quantity, @total, @numtable, @username, @date)";
                            MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                            insertCmd.Parameters.AddWithValue("@name", row["name"]);
                            insertCmd.Parameters.AddWithValue("@price", row["price"]);
                            insertCmd.Parameters.AddWithValue("@quantity", row["quantity"]);
                            insertCmd.Parameters.AddWithValue("@total", row["total"]);
                            insertCmd.Parameters.AddWithValue("@numtable", row["numtable"]);
                            insertCmd.Parameters.AddWithValue("@username", row["username"]);
                            insertCmd.Parameters.AddWithValue("@date", DateTime.Now);
                            insertCmd.ExecuteNonQuery();
                        }

                        string clearQuery = "DELETE FROM `order`";
                        MySqlCommand clearCmd = new MySqlCommand(clearQuery, conn);
                        clearCmd.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการเชื่อมต่อฐานข้อมูล: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Optional action for No response
            }
        }


        private void InsertHistoryAndDeleteBooked()
        {
            try
            {
                MySqlConnection conn = databaseConnection();
                conn.Open();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        string insertQuery = "INSERT INTO history (username,numtable,status, date) VALUES ( @username,@numtable,@status ,@date)";
                        MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@username", username);
                        insertCmd.Parameters.AddWithValue("@numtable", numtable);
                        insertCmd.Parameters.AddWithValue("@status", 1);
                        insertCmd.Parameters.AddWithValue("@date", DateTime.Now);

                        insertCmd.ExecuteNonQuery();
                    }
                }
                string deleteQuery = "DELETE FROM booked WHERE username = @username";
                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@username", username);
                    deleteCmd.ExecuteNonQuery();
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable LoadDataFromDatabase()
        {
            try
            {
                using (MySqlConnection conn = databaseConnection())
                {
                    conn.Open();

                    // Load data from `order` table
                    string loadQuery = "SELECT * FROM `order`";
                    MySqlCommand loadCmd = new MySqlCommand(loadQuery, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(loadCmd);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.DataSource = dt;

                    return dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return null;
            }
        }


        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async Task CreatePdf()
        {

            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string receiptFolder = Path.Combine(documentsFolder, "Receipt");
            
            if (!Directory.Exists(receiptFolder))
            {
                Directory.CreateDirectory(receiptFolder);
            }

            // Create the file path for the PDF
            string filePath = Path.Combine(receiptFolder, $"Receipt_{DateTime.Now:yyyyMMddHHmmss}.pdf");

            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    Document doc = new Document(PageSize.B5, 25, 25, 20, 20);
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                    doc.Open();
                    

                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                    // เพิ่มหัวข้อ
                    Paragraph title = new Paragraph("Receipt\nButter Bar🥂", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    doc.Add(title);
                    doc.Add(new Paragraph("\n"));
                    doc.Add(new Paragraph("                 105 Sukhumvit Rd, Khwaeng Bang Na, Khet Bang Na, Bankok 10270 "));
                    doc.Add(new Paragraph("                                      TAX ID : 0653050334200 (VAT Included)"));
                    doc.Add(new Paragraph("                                                 Tel : 0981965003"));
                    doc.Add(new Paragraph("         -------------------------------------------------------------------------------------------------"));
                    doc.Add(new Paragraph($"        Date: {DateTime.Now}"));
                    doc.Add(new Paragraph("         -------------------------------------------------------------------------------------------------"));


                    doc.Add(new Paragraph("\n"));
                    
                    PdfContentByte cb = writer.DirectContent;

                    float yPosition = 485; // จุดเริ่มต้นสำหรับรายการสินค้า
                    float leftMargin = 50;
                    float rightMargin = doc.PageSize.Width - 50;

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            string name = row.Cells["name"].Value?.ToString() ?? string.Empty;
                            string quantity = row.Cells["quantity"].Value?.ToString() ?? string.Empty;
                            string total = row.Cells["total"].Value != null ? String.Format("{0:N2}", Convert.ToDecimal(row.Cells["total"].Value)) : "0.00";

                            cb.BeginText();
                            cb.SetFontAndSize(cellFont.BaseFont, 12);
                            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, quantity, leftMargin, yPosition, 0);
                            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, name, leftMargin + 15, yPosition, 0); // ปรับตามต้องการ
                            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, total, rightMargin, yPosition, 0);
                            cb.EndText();

                            yPosition -= 20; // ปรับการเว้นบรรทัดตามต้องการ
                        }
                    }

                    doc.Add(new Paragraph("\n"));
                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "-----------------------------------------------------------------------------------------------------", leftMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;
                    decimal totalAmount = CalculateTotalAmount();
                    decimal vat = CalculateVAT(totalAmount);
                    decimal beforeVAT = CalculateBeforeVAT(totalAmount);


                    // Total Baht
                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Total Baht", leftMargin, yPosition, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, totalAmount.ToString("C"), rightMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;

                    // NET Baht
                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "NET Baht", leftMargin, yPosition, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, totalAmount.ToString("C"), rightMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;
                   
                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "*************************************************************************************", leftMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;
                    
                    // Before VAT
                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Before VAT", leftMargin, yPosition, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, beforeVAT.ToString("C"), rightMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;
                    // VAT 7%
                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "VAT 7%", leftMargin, yPosition, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, vat.ToString("C"), rightMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;
                    // VATAble
                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "VATable", leftMargin, yPosition, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, totalAmount.ToString("C"), rightMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;

                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "-----------------------------------------------------------------------------------------------------", leftMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;
                    cb.BeginText();
                    cb.SetFontAndSize(headerFont.BaseFont, 12);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Thank you <3", leftMargin, yPosition, 0);
                    cb.EndText();
                    yPosition -= 20;

                    doc.Close();
                    /*MessageBox.Show("PDF created and saved at: " + filePath);*/



                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                    await SendEmailWithPDFAsync(filePath);
                    if (MessageBox.Show("การจองสำเร็จค่ะ\nคุณต้องการทำรายการต่อใช่หรือไม่", "Butter Bar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        this.Hide();
                        Mains f = new Mains(username);
                        f.Show();
                    }
                    else
                    {
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการสร้าง PDF: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private decimal CalculateTotalAmount()
        {
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["total"].Value != null)
                {
                    totalAmount += Convert.ToDecimal(row.Cells["total"].Value);
                }
            }

            return totalAmount;
        }

        private decimal CalculateVAT(decimal totalAmount)
        {
            // Calculate VAT (7%)
            decimal vat = totalAmount * 0.07m;
            
            return vat;
        }
        private decimal CalculateBeforeVAT(decimal totalAmount)
        {
            // Before VAT = Total Amount / (1 + VAT rate)
            decimal beforeVAT = totalAmount / 1.07m;
            return beforeVAT;
        }
        private async Task SendEmailWithPDFAsync(string filePath)
        {
            try
            {
                string fromMail = "noeywarit2546@gmail.com";
                string fromPassword = "bpvu asis iatv mrow"; // Consider storing passwords more securely

                // Retrieve email from TextBox
                string email = textBox1.Text.Trim();

                if (string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("กรุณากรอกอีเมลล์ให้ถูกต้อง", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Retrieve reservation details from the database
                (string reservationName, DateTime reservationDate, string tableNumber) = GetReservationDetails();

                // Construct the email body with reservation details
                string body = $"ขอบคุณที่ใช้บริการ Butter Bar \n\n" +
                              $"รายละเอียดการจอง\n" +
                              $"ชื่อผู้จอง: {reservationName}\n" +
                              $"วันที่จอง: {reservationDate.ToString("dd/MM/yyyy")}\n" +
                              $"โต๊ะที่จอง: {tableNumber}\n\n" +
                              $"กรุณามารับโต๊ะก่อน 20:30 น. หากไม่มาภายในเวลาที่กำหนดปล่อยโต๊ะทันที\n\n"+
                              $"หากต้องการแจ้งปัญหาสามารถติดต่อได้ที่ 0981965003 (เนย)";

                // Create the email message
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(fromMail),
                    Subject = "RECEIPT Butter Bar",
                    Body = body,
                    IsBodyHtml = false
                };
                message.To.Add(new MailAddress(email));

                // Attach the PDF receipt
                Attachment attachment = new Attachment(filePath);
                message.Attachments.Add(attachment);

                // Send the email using SmtpClient
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential(fromMail, fromPassword);
                    smtpClient.EnableSsl = true;

                    await smtpClient.SendMailAsync(message);
                }

                
            }
            catch (Exception ex)
            {
                // Handle exceptions and notify the user
                MessageBox.Show("ส่งใบเสร็จชำระเงินไม่สำเร็จค่ะ: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private (string name, DateTime date, string tableNumber) GetReservationDetails()
        {
            string name = string.Empty;
            DateTime date = DateTime.MinValue;
            string tableNumber = string.Empty;

            // Database connection
            using (MySqlConnection conn = databaseConnection())
            {
                conn.Open();
                // Assuming the `booked` table has fields like `username`, `date`, and `numtable`
                string query = "SELECT username, date, numtable FROM booked WHERE username = @username";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            name = reader["username"].ToString();
                            date = Convert.ToDateTime(reader["date"]);
                            tableNumber = reader["numtable"].ToString();
                        }
                    }
                }
            }

            return (name, date, tableNumber);
        }





        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
            form7.Show();
        }
    }
}


    

