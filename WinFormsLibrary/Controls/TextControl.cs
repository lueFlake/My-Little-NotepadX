using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsLibrary;

namespace WinFormsLibrary.Controls {
    public partial class TextControl : TabControl {
        public TextControl() : base() {
            TabPages = new TextPageCollection(this);
        }

        public class TextPageCollection : TabPageCollection {
            public TextPageCollection(TabControl owner) : base(owner) { }

            public new TextPage this[int index] => (TextPage)base[index];

            public new TextPage this[string key] => (TextPage)base[key];

            public int Add(System.IO.FileInfo file = null) {
                try {
                    var textPage = new TextPage(file);

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
                var textPage = new TextPage();
                Add(textPage);
                return Count - 1;
            }

            public new IEnumerator<TextPage> GetEnumerator() {
                return this.OfType<TextPage>().GetEnumerator();
            }
        }

        public new TextPage SelectedTab => (TextPage)base.SelectedTab;

        public new TextPageCollection TabPages { get; }

        public Color TabTextBackColor {
            get;
        }
    }
}
