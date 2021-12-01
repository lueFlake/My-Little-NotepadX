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
    public partial class AppCloseForm : Form {
        private List<TextPage> _textPages;

        public string fileLabel {
            get  => label2.Text; 
            set => label2.Text = value; 
        }

        public AppCloseForm(List<TextPage> textPages) {
            InitializeComponent();
            _textPages = textPages;
            listBox1.Items.AddRange(_textPages.Select(x => x.FileName).ToArray());
        }

        private void button3_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Ignore;
            Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            int index = listBox1.SelectedIndex;

            if (index == -1) {
                MessageBox.Show("Не выбран документ.");
                return;
            }

            FileInfo savePath;
            if (!File.Exists(_textPages[index].FileFullName)) {
                savePath = MessageTools.ShowFileDialog();
            }
            else {
                savePath = new FileInfo(_textPages[index].FileFullName);
            }

            if (savePath != null) {
                _textPages[index].SaveFile(savePath);
                _textPages.RemoveAt(index);
                listBox1.Items.RemoveAt(index);
            }
            if (listBox1.Items.Count == 0) {
                DialogResult = DialogResult.Ignore;
                Close();
            }
        }
    }
}
