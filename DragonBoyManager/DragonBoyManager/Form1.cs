using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace DragonBoyManager
{
    public partial class Form1 : Form
    {
        private int selectedAccountId = -1;
        private int selectedAccountStt = -1;
        private static bool IsBusy = false;
        public static string GameExecutablePath = Application.StartupPath + "\\Dragonboy_vn_v222.exe";
        public Form1()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            loadData();
            dataGridView1.Columns["Password"].Visible = false;
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns["limit149tr"].Visible = false;
            dataGridView1.Columns["limit1ty49"].Visible = false;
            dataGridView1.Columns["autoCoden"].Visible = false;
            dataGridView1.AllowUserToAddRows = false;
            checkBox1.Checked = true;
        }


        private void loadData()
        {
            // Lấy dữ liệu từ database
            List<Account> accounts = DatabaseHelper.GetAllAccounts();

            // Xóa dữ liệu cũ trong DataGridView
            dataGridView1.Rows.Clear();

            // Thêm từng hàng vào DataGridView
            foreach (var acc in accounts)
            {
                dataGridView1.Rows.Add(dataGridView1.Rows.Count, acc.Username, acc.Server, acc.Type, acc.Map, acc.Khu, acc.ToaDoX, acc.ToaDoY, acc.Password, acc.Id, acc.GioiHan149tr, acc.GioiHan1ty49, acc.AutoCoden);
            }
            if (dataGridView1.Columns.Contains("Password")) dataGridView1.Columns["Password"].Visible = false;
        }



        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(comboBox1.Text) ||
                string.IsNullOrWhiteSpace(comboBox2.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            Account acc = new Account
            {
                Username = textBox1.Text,
                Password = AesEncryptionHelper.Encrypt(textBox3.Text),
                Server = comboBox1.Text,
                Type = comboBox2.Text,
                ToaDoX = textBox2.Text,
                ToaDoY = textBox4.Text,
                Map = textBox5.Text,
                Khu = textBox6.Text,
                GioiHan149tr = checkBox2.Checked,
                GioiHan1ty49 = checkBox3.Checked,
                AutoCoden = checkBox4.Checked
            };

            DatabaseHelper.AddAccount(acc);
            MessageBox.Show("Đã lưu tài khoản!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            loadData();
        }

        // Khi click vào DataGridView, lưu lại ID và hiển thị các giá trị
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                selectedAccountId = Convert.ToInt32(row.Cells["Id"].Value); // lưu ID
                selectedAccountStt = e.RowIndex;
                textBox1.Text = row.Cells["Username"].Value.ToString();
                comboBox1.Text = row.Cells["Server"].Value.ToString();
                comboBox2.Text = row.Cells["Type"].Value.ToString();
                textBox2.Text = row.Cells["ToaDoX"].Value.ToString();
                textBox4.Text = row.Cells["ToaDoY"].Value.ToString();
                textBox5.Text = row.Cells["Map"].Value.ToString();
                textBox6.Text = row.Cells["Khu"].Value.ToString();

                // Cập nhật trạng thái các CheckBox
                checkBox2.Checked = Convert.ToBoolean(row.Cells["limit149tr"].Value);
                checkBox3.Checked = Convert.ToBoolean(row.Cells["limit1ty49"].Value);
                checkBox4.Checked = Convert.ToBoolean(row.Cells["autoCoden"].Value);
            }
        }


        // Sửa button update để dùng ID đã lưu
        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedAccountId == -1)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần cập nhật!");
                return;
            }

            Account acc = new Account
            {
                Id = selectedAccountId,
                Username = textBox1.Text,
                Password = AesEncryptionHelper.Encrypt(textBox3.Text),
                Server = comboBox1.Text,
                Type = comboBox2.Text,
                ToaDoX = textBox2.Text,
                ToaDoY = textBox4.Text,
                Map = textBox5.Text,
                Khu = textBox6.Text,
                GioiHan149tr = checkBox2.Checked,
                GioiHan1ty49 = checkBox3.Checked,
                AutoCoden = checkBox4.Checked
            };

            DatabaseHelper.UpdateAccount(acc);
            MessageBox.Show("Cập nhật thành công!");
            loadData();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedAccountId == -1)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa!");
                return;
            }

            DatabaseHelper.DeleteAccount(selectedAccountId);
            MessageBox.Show("Xóa thành công!");
            selectedAccountId = -1;
            loadData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.UseSystemPasswordChar = checkBox1.Checked;
        }

        [DllImport("user32.dll", SetLastError = true)] private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")] public static extern bool SetWindowText(IntPtr hWnd, string text);
        [DllImport("user32.dll", SetLastError = true)] public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);


        //thêm truyền argument
        private async Task LaunchGameClientAsync(int index)
        {
            if (!IsBusy)
            {
                IsBusy = true;

                Account account = DatabaseHelper.GetAccountById(selectedAccountId);
                if (account == null)
                {
                    MessageBox.Show("Không tìm thấy tài khoản!");
                    IsBusy = false;
                    return;
                }

                string username = account.Username;
                string password = AesEncryptionHelper.Decrypt(account.Password);
                string server = account.Server;

                string arguments = $"--username \"{username}\" --password \"{password}\" --server \"{server}\"";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = GameExecutablePath,
                    Arguments = arguments,
                    UseShellExecute = false
                };

                Process process = Process.Start(startInfo);

                await Task.Delay(500); // Đợi game khởi động sơ bộ

                // Chờ đến khi cửa sổ game xuất hiện
                while (!IsGameRunning("Dragonboy222"))
                    await Task.Delay(100);

                // Đặt lại tiêu đề cửa sổ
                SetWindowText(FindWindow(null, "Dragonboy222"), "TNL" + (index + 1));

                await Task.Delay(1000); // Đợi một chút để đảm bảo game ổn định
                IsBusy = false;
            }
        }


        public static bool IsGameRunning(string windowTitle) => FindWindow(null, windowTitle) != IntPtr.Zero;

        private async void button4_Click(object sender, EventArgs e)
        {
            await LaunchGameClientAsync(selectedAccountStt);
        }

        private async void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            await LaunchGameClientAsync(selectedAccountStt);
        }

    }
}

