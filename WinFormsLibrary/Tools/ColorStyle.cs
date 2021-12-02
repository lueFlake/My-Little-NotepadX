using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsLibrary.Tools
{
    public class ColorStyle
    {
        
        public Color MainBodyBackcolor { get; set; } = Color.FromName("Control");
        public Color MainBodyForecolor { get; set; } = Color.FromName("Black");

        public ColorStyle(string color1, string color2) {
            MainBodyBackcolor = Color.FromName(color1);
            MainBodyForecolor = Color.FromName(color2);
        }

        public ColorStyle((int, int, int) color1, (int, int, int) color2) {
            MainBodyBackcolor = Color.FromArgb(color1.Item1, color1.Item2, color1.Item3);
            MainBodyForecolor = Color.FromArgb(color2.Item1, color2.Item2, color2.Item3);
        }

        public ColorStyle(int color1, int color2) {
            MainBodyBackcolor = Color.FromArgb(color1);
            MainBodyForecolor = Color.FromArgb(color2);
        }

        [JsonConstructor]
        public ColorStyle(Color MainBodyBackcolor, Color MainBodyForecolor) {
            this.MainBodyBackcolor = MainBodyBackcolor;
            this.MainBodyForecolor = MainBodyForecolor;
        }

        public static void ChangeColorScheme(ColorStyle style, Control control) {
            control.BackColor = style.MainBodyBackcolor;
            control.ForeColor = style.MainBodyForecolor;
            if (control is MenuStrip) {
                foreach (var item in ((MenuStrip)control).Items) {
                    if (item is ToolStripMenuItem)
                        ChangeColorScheme(style, (ToolStripMenuItem)item);
                }
            }
            if (control.ContextMenuStrip != null) {
                foreach (var item in control.ContextMenuStrip.Items) {
                    if (item is ToolStripMenuItem)
                        ChangeColorScheme(style, (ToolStripMenuItem)item);
                }
            }
            foreach (Control subcontrol in control.Controls) {
                ChangeColorScheme(style, subcontrol);
            }
        }

        public static void ChangeColorScheme(ColorStyle style, ToolStripMenuItem item) {
            item.BackColor = style.MainBodyBackcolor;
            item.ForeColor = style.MainBodyForecolor;
            foreach (var subitem in item.DropDownItems) {
                if (subitem is ToolStripMenuItem) {
                    ChangeColorScheme(style, (ToolStripMenuItem)subitem);
                }
                if (subitem is ToolStripSeparator) {
                    ToolStripSeparator stripSeparator = (ToolStripSeparator)subitem;
                    stripSeparator.BackColor = style.MainBodyBackcolor;
                    stripSeparator.ForeColor = style.MainBodyForecolor;
                }
            }
        }
    }
}
