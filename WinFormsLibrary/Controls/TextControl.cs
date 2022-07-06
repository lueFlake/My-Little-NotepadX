using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsLibrary.Controls {
    /// <summary>
    /// Объект для работы со вкладками в интерфейсе.
    /// </summary>
    public partial class TextControl : TabControl {

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public TextControl() : base() {
            TabPages = new TextPageCollection(this);

            DrawMode = TabDrawMode.OwnerDrawFixed;
            DrawItem += new System.Windows.Forms.DrawItemEventHandler(TextControlDrawItem);
        }

        /// <summary>
        /// Класс коллекции для хранения вкладок.
        /// </summary>
        public class TextPageCollection : TabPageCollection {
            /// <summary>
            /// Конструктор класса.
            /// </summary>
            /// <param name="owner">"Владелец" экземпляра.</param>
            public TextPageCollection(TabControl owner) : base(owner) { }

            /// <summary>
            /// Перегрузка индексирования по коллекции.
            /// </summary>
            /// <param name="index">Целочисленный индекс.</param>
            /// <returns>Значение, соответствущее индексу.</returns>
            public new TextPage this[int index] => (TextPage)base[index];

            /// <summary>
            /// Перегрузка индексирования по коллекции по ключям.
            /// </summary>
            /// <param name="index">Ключ в виде строки.</param>
            /// <returns>Значение, соответствущее ключу.</returns>
            public new TextPage this[string key] => (TextPage)base[key];

            /// <summary>
            /// Добавление вкладки в коллекцию.
            /// </summary>
            /// <param name="file">Файл, соответствующий вкладке.</param>
            /// <returns>Индекс, добавленного элемента.</returns>
            public int Add(FileInfo file = null) {
                try {
                    var textPage = new TextPage(file);

                    int index = 0;
                    foreach (var page in this) {
                        if (page.FileFullName == textPage.FileFullName)
                            break;
                        index++;
                    }

                    if (index == Count) {
                        Add(textPage);
                        return Count - 1;
                    }
                    else {
                        return index;
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

            /// <summary>
            /// Добавление пустой вкладки.
            /// </summary>
            /// <returns>Добавление пустой вкладки.</returns>
            public int Add() {
                var textPage = new TextPage();
                Add(textPage);
                return Count - 1;
            }

            /// <summary>
            /// Взятие счетчика для корректной работы с циклами foreach.
            /// </summary>
            /// <returns>Счетчик.</returns>
            public new IEnumerator<TextPage> GetEnumerator() {
                return this.OfType<TextPage>().GetEnumerator();
            }
        }

        /// <summary>
        /// Выбранная вкладка.
        /// </summary>
        public new TextPage SelectedTab => (TextPage)base.SelectedTab;

        /// <summary>
        /// Коллекция вкладок.
        /// </summary>
        public new TextPageCollection TabPages { get; }

        /// <summary>
        /// Изменение цветов кнопок вкладок на фиолетовый.
        /// </summary>
        /// <param name="sender">Издатель.</param>
        /// <param name="e">Аргументы события.</param>
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
    }
}
