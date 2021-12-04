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
using WinFormsLibrary;

namespace WinFormsLibrary.Controls {
    public partial class TextControl : TabControl {

        public TextControl() : base() {
            TabPages = new TextPageCollection(this);

            DrawMode = TabDrawMode.OwnerDrawFixed;
            DrawItem += new System.Windows.Forms.DrawItemEventHandler(TextControlDrawItem);
        }

        public class TextPageCollection : TabPageCollection {
            public TextPageCollection(TabControl owner) : base(owner) { }

            public new TextPage this[int index] => (TextPage)base[index];

            public new TextPage this[string key] => (TextPage)base[key];

            public int Add(FileInfo file = null) {
                try {
                    var textPage = CreateInstance(file);

                    if (!Contains(textPage)) {
                        Add(textPage);
                        return Count - 1;
                    }
                    else {
                        return IndexOf(textPage);
                    }
                }
                catch (System.IO.IOException ex) {
                    Tools.MessageTools.ShowException("Ошибка ввода/вывода во время попытки открытия файла:", ex);
                }
                catch (ArgumentException ex) {
                    Tools.MessageTools.ShowException("Выбран некорректный файл:", ex);
                }
                return -1;
            }

            public int Add() {
                var textPage = CreateInstance();
                Add(textPage);
                return Count - 1;
            }

            public new IEnumerator<TextPage> GetEnumerator() {
                return this.OfType<TextPage>().GetEnumerator();
            }
        }

        public new TextPage SelectedTab => (TextPage)base.SelectedTab;

        public new TextPageCollection TabPages { get; }

        private void TextControlDrawItem(object sender, DrawItemEventArgs e) {

            Color backColor = Color.Magenta;
            if (e.Index == SelectedIndex) 
                backColor = Color.DarkMagenta;

            Rectangle rect = e.Bounds;
            e.Graphics.FillRectangle(new SolidBrush(backColor), (rect.X) + 4, rect.Y, (rect.Width) - 4, rect.Height);

            SizeF sz = e.Graphics.MeasureString(TabPages[e.Index].Text, e.Font);
            e.Graphics.DrawString(TabPages[e.Index].Text, 
                e.Font,
                e.Index == SelectedIndex ? Brushes.WhiteSmoke : Brushes.White,
                e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2,
                e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1
            );
        }

        private static TextPage CreateInstance(FileInfo? fileInfo = null) {
            if (fileInfo == null) {
                return new AnyFileTextPage(fileInfo);
            }
            return fileInfo.Extension switch {
                ".rtf" => new RTFTextPage(fileInfo),
                ".cs" => new CSharpTextPage(fileInfo),
                _ => new AnyFileTextPage(fileInfo)
            };
        }

        public static TextPage CreateInstance(FileInfo bufferedFile, string outputFile, string text, bool isSaved, bool empty) {
            return bufferedFile.Extension switch {
                ".rtf" => new RTFTextPage(bufferedFile, outputFile, text, isSaved, empty),
                ".cs" => new CSharpTextPage(bufferedFile, outputFile, text, isSaved, empty),
                _ => new AnyFileTextPage(bufferedFile, outputFile, text, isSaved, empty)
            };
        }
    }
}
