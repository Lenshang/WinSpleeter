using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinSpleeter
{
    public partial class Form1 : Form
    {
        RunnerModule runner;
        String openPath;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new TextBoxWriter(this.textBox1));
        }

        private void StartProcess(string filePath,Action callback,string stems="2stems")
        {
            Console.WriteLine("Spleeter start!");
            this.runner = new RunnerModule("miniconda3/Scripts/activate.bat", $"./miniconda3 && python -m spleeter separate -i \"{filePath}\" -p spleeter:{stems} -o output", false);
            this.runner.OnMessageReceive = (string msg) => {
                Console.WriteLine(msg);
            };
            this.runner.OnProcessExit = () =>
            {
                Console.WriteLine("Spleeter exited");
                callback();
            };
            this.runner.Run();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";
            dialog.Filter = "MP3文件(*.mp3)|*.mp3";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                this.tbFilePath.Text = file;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbFilePath.Text))
            {
                MessageBox.Show("尚未选择文件!");
                return;
            }

            if (!File.Exists(this.tbFilePath.Text))
            {
                MessageBox.Show("选择的文件不存在!");
                return;
            }

            (sender as Button).Enabled = false;
            string selectStems = "2stems";
            if (this.rb4stems.Checked)
            {
                selectStems = "4stems";
            }
            else if (this.rb5stems.Checked)
            {
                selectStems = "5stems";
            }

            Action callback = () => {
                this.Invoke(new Action(() => {
                    MessageBox.Show("处理完成");
                    (sender as Button).Enabled = true;
                    this.btOpen.Enabled = true;
                }));
            };

            this.StartProcess(this.tbFilePath.Text, callback, selectStems);
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("./output"))
            {
                System.Diagnostics.Process.Start("explorer", Path.Combine(System.Environment.CurrentDirectory, "output"));
            }
        }
    }
}
