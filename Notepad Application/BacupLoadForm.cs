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
using WinFormsLibrary.Controls;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    public partial class BacupLoadForm : Form {
        private List<FileInfo> _backups;
        public FileInfo Callback { get; set; } 

        public BacupLoadForm(TextPage textPage) {
            InitializeComponent();
            string usedDirectory = textPage.FileFullName.GetHashCode().ToString();
            _backups = Directory.GetFiles($"../../../Backups/{usedDirectory}").
                Select(x => new FileInfo(x)).
                ToList();
            foreach (var backup in _backups)
                listBox1.Items.Add(backup.Name);
            DialogResult = DialogResult.Cancel;
        }

        private void BacupLoadForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
        }

        private void button1_Click(object sender, EventArgs e) {
            int index = listBox1.SelectedIndex;

            if (index == -1) {
                MessageBox.Show("Не выбран документ.");
                return;
            }

            DialogResult = DialogResult.OK;
            Callback = _backups[index];
        }

        private void button2_Click(object sender, EventArgs e) {
            int index = listBox1.SelectedIndex;

            if (index == -1) {
                MessageBox.Show("Не выбран документ.");
                return;
            }
            
            _backups[index].Delete();
            listBox1.Items.RemoveAt(index);
        }

        private void button3_Click(object sender, EventArgs e) {
            foreach (var backup in _backups)
                backup.Delete();
            listBox1.Items.Clear();
        }
    }
}
