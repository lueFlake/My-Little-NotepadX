using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSharpLibrary;
using WinFormsLibrary.Tools;

namespace NotepadApplication {
    public partial class StyleForm : Form {

        private static ColorDialog s_colorDialog = new ColorDialog();
        private static FontDialog s_fontDialog = new FontDialog();
        private ColorStyle _colorTheme = ConfigurationSetter.ColorTheme;

        public ColorStyle ColorStyleCallback { get; private set; }
        public Font FontCallback { get; private set; }

        public StyleForm() {
            InitializeComponent();
            FontCallback = ConfigurationSetter.MainFont;
            ColorStyleCallback = ConfigurationSetter.ColorTheme;
        }

        private void button1_Click(object sender, EventArgs e) {
            ColorStyleCallback = new ColorStyle((31, 31, 31), (240, 240, 240));
            Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            ColorStyleCallback = new ColorStyle("Control", "Black");
            Close();
        }

        private void StyleForm_Load(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
            button3.BackColor = button4.ForeColor = _colorTheme.MainBodyBackcolor;
            button3.ForeColor = button4.BackColor = _colorTheme.MainBodyForecolor;
            button4.BackColor = SyntaxHighlight.SyntaxTokenColors["keyword"];
            button5.BackColor = SyntaxHighlight.SyntaxTokenColors["class name"];
            button6.BackColor = SyntaxHighlight.SyntaxTokenColors["enum name"];
            button7.BackColor = SyntaxHighlight.SyntaxTokenColors["method name"];
            button8.BackColor = SyntaxHighlight.SyntaxTokenColors["local name"];
            button9.BackColor = SyntaxHighlight.SyntaxTokenColors["string"];
            button10.BackColor = SyntaxHighlight.SyntaxTokenColors["comment"];
            button11.BackColor = SyntaxHighlight.SyntaxTokenColors["other"];
        }

        private void button3_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            _colorTheme.MainBodyBackcolor = s_colorDialog.Color;
        }

        private void button4_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            _colorTheme.MainBodyForecolor = s_colorDialog.Color;
        }

        private void button5_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            SyntaxHighlight.SyntaxTokenColors["keywords"] = s_colorDialog.Color;
        }

        private void button6_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            SyntaxHighlight.SyntaxTokenColors["class name"] = s_colorDialog.Color;
            SyntaxHighlight.SyntaxTokenColors["record name"] = s_colorDialog.Color;
            SyntaxHighlight.SyntaxTokenColors["struct name"] = s_colorDialog.Color;
            SyntaxHighlight.SyntaxTokenColors["type identifiers"] = s_colorDialog.Color;
        }

        private void button7_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            SyntaxHighlight.SyntaxTokenColors["enum"] = s_colorDialog.Color;
        }

        private void button8_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            SyntaxHighlight.SyntaxTokenColors["method name"] = s_colorDialog.Color;
            SyntaxHighlight.SyntaxTokenColors["method identifier"] = s_colorDialog.Color;
        }

        private void button9_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            SyntaxHighlight.SyntaxTokenColors["local name"] = s_colorDialog.Color;
            SyntaxHighlight.SyntaxTokenColors["parameter name"] = s_colorDialog.Color;
        }

        private void button10_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            SyntaxHighlight.SyntaxTokenColors["string"] = s_colorDialog.Color;
        }

        private void button11_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            SyntaxHighlight.SyntaxTokenColors["comment"] = s_colorDialog.Color;
        }

        private void button12_Click(object sender, EventArgs e) {
            s_colorDialog.ShowDialog();
            SyntaxHighlight.SyntaxTokenColors["other"] = s_colorDialog.Color;
        }

        private void button13_Click(object sender, EventArgs e) {
            ColorStyle.ChangeColorScheme(ConfigurationSetter.ColorTheme, this);
            ColorStyleCallback = ConfigurationSetter.ColorTheme;
            ConfigurationSetter.SyntaxColors = SyntaxHighlight.SyntaxTokenColors;
        }

        private void button14_Click(object sender, EventArgs e) {
            s_fontDialog.ShowColor = false;
            s_fontDialog.ShowDialog();
            FontCallback = s_fontDialog.Font;
        }
    }
}
