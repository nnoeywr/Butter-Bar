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
    public partial class Form8 : Form
    {
        private string receivedUsername;
        // คอนสตรัคเตอร์ที่รับค่า username
        public Form8(string username)
        {
            InitializeComponent();
            receivedUsername = username;
        }
       
        public Form8()
        {
            InitializeComponent();
        }
        private void buttonSomeAction_Click(object sender, EventArgs e)
        {
            OpenForm8IfAdmin();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenForm4IfAdmin();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
        private void OpenForm8IfAdmin()
        {
            if (receivedUsername == "admin")
            {
                // ส่งค่า receivedUsername เมื่อเปิด Form8 อีกครั้ง
                Form8 form8 = new Form8(receivedUsername);
                form8.Show();
            }
            else
            {
                MessageBox.Show("คุณไม่มีสิทธิ์เข้าถึงฟังก์ชันนี้", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void OpenForm4IfAdmin()
        {
            if (receivedUsername == "admin")
            {
                // ซ่อนหน้าปัจจุบันแทนการปิดเพื่อให้กลับมาเปิดใหม่ได้
                this.Hide();

                // ตรวจสอบและส่ง username ไปยัง Table (Form4)
                Table form4 = new Table(receivedUsername);
                form4.FormClosed += (s, args) => this.Show(); // เมื่อ Form4 ถูกปิด ให้แสดง Form8 อีกครั้ง
                form4.Show();
            }
            else
            {
                MessageBox.Show("คุณไม่มีสิทธิ์เข้าถึงฟังก์ชันนี้", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        private void Form8_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)//ปุ่มเปดจัดการโต๊ะ
        {
            OpenForm4IfAdmin();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form6 f = new Form6();
            f.Show();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Summ f = new Summ ();
            f.Show();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            History f = new History();
            f.Show();
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {
            this.Hide ();
            Login f = new Login ();
            f.Show();
        }
    }
}
