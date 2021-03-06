using System.Drawing;
using System.Windows.Forms;
using WinFormsLibrary.Controls;

namespace WinFormsLibrary.Tools
{
    /// <summary>
    /// Класс контролирующий цветовую расцветку интерфейса.
    /// </summary>
    public class ColorStyle
    {
        
        /// <summary>
        /// Основной цвет (Задний фон).
        /// </summary>
        public Color MainBodyBackcolor { get; set; } = Color.FromName("Control");
        /// <summary>
        /// Второстепенный цвет (Цвет текста).
        /// </summary>
        public Color MainBodyForecolor { get; set; } = Color.FromName("Black");

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="color1">Название основного цвета.</param>
        /// <param name="color2">Название второстепенного цвета.</param>
        public ColorStyle(string color1, string color2) {
            MainBodyBackcolor = Color.FromName(color1);
            MainBodyForecolor = Color.FromName(color2);
        }
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="color1">Значения RGB основного цвета.</param>
        /// <param name="color2">Значения RGB второстепенного цвета.</param>
        public ColorStyle((int, int, int) color1, (int, int, int) color2) {
            MainBodyBackcolor = Color.FromArgb(color1.Item1, color1.Item2, color1.Item3);
            MainBodyForecolor = Color.FromArgb(color2.Item1, color2.Item2, color2.Item3);
        }
        /// <summary>
        /// Конструктор класса.
        /// </summary>
        /// <param name="color1">Значение argb+ основного цвета.</param>
        /// <param name="color2">Значение argb+ второстепенного цвета.</param>
        public ColorStyle(int color1, int color2) {
            MainBodyBackcolor = Color.FromArgb(color1);
            MainBodyForecolor = Color.FromArgb(color2);
        }

        /// <summary>
        /// Изменение цвета элемента интерфейса.
        /// </summary>
        /// <param name="style">Цветовая расцветка.</param>
        /// <param name="control">Элемент интерфейса.</param>
        public static void ChangeColorScheme(ColorStyle style, Control control) {
            if (control is TextPage && ((TextPage)control).FileExtension == ".rtf") {
                return;
            }
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

        /// <summary>
        /// Изменение цвета предмета ToolStrip.
        /// </summary>
        /// <param name="style">Цветовая расцветка.</param>
        /// <param name="control">Предмет ToolStrip.</param>
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
