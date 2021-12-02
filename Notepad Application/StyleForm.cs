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
    public partial class StyleForm : Form {

        public ColorStyle Callback {
            get; set;
        }

        public StyleForm() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Callback = new ColorStyle((31, 31, 31), (240, 240, 240));
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            Callback = new ColorStyle("Control", "Black");
            this.Close();
        }

        private void StyleForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
            Callback = ConfigurationSetter.ColorTheme;
        }
    }
}
