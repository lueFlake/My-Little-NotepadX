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
using NotepadApplication;
using WinFormsLibrary.Tools;
using CSharpLibrary;

namespace NotepadApplication {
    public partial class CompilerForm : Form {
        public CompilerForm() {
            InitializeComponent();
            textBox1.Text = ConfigurationSetter.CompilerPath;
            label3.Text = CompilationRunner.CheckCompiler(textBox1.Text);
        }

        private void CompilleForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
        }

        private void button1_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog() {
                InitialDirectory = Path.GetDirectoryName(textBox1.Text),
                Filter = "Windows Executable (*.exe)|*.exe",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            if (File.Exists(dialog.FileName))
                textBox1.Text = dialog.FileName;
        }

        private void button2_Click(object sender, EventArgs e) {
            label3.Text = CompilationRunner.CheckCompiler(textBox1.Text);
            if (label3.Text != "undefined") {
                ConfigurationSetter.CompilerPath = textBox1.Text;
                Close();
            }
            else {
                MessageBox.Show("Выбран недействительный путь к csc.exe");
            }
        }
    }
}
