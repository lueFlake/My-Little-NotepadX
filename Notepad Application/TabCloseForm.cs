using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    public partial class TabCloseForm : Form {
        public TabCloseForm() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Yes;
            Close();
        }
        private void button2_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.No;
            Close();
        }

        private void TabCloseForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
        }
    }
}
