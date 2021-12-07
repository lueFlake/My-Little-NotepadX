using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSharpLibrary {
    /// <summary>
    /// Класс осуществляющий компиляцию и выполнение CSharp кода.
    /// </summary>
    public class PromptRunner {

        /// <summary>
        /// Процесс для компилятора и скомпилированной программы.
        /// </summary>
        private static Process s_process = new();

        /// <summary>
        /// Компиляция открытого .cs файла.
        /// </summary>
        /// <param name="path">Путь к компилятору.</param>
        /// <param name="source">Путь к исходному коду.</param>
        /// <param name="executable">Путь к скомпилированой программе.</param>
        /// <returns>Вывод компилятора.</returns>
        public static string Build(string path, string source, string executable) {
            s_process.StartInfo.UseShellExecute = false;
            s_process.StartInfo.CreateNoWindow = true;
            s_process.StartInfo.RedirectStandardOutput = true;
            s_process.StartInfo.RedirectStandardError = true;
            s_process.StartInfo.FileName = path;
            s_process.StartInfo.Arguments = $"\"{source}\" /t:exe /out:\"{executable}\"";

            try {
                s_process.Start();
            }
            catch {
                return null;
            }

            return s_process.StandardOutput.ReadToEnd();
        }

        /// <summary>
        /// Запуск исполняемого файла скомпилированой программы.
        /// </summary>
        /// <param name="executable">Путь к скомпилированой программе.</param>
        /// <returns>Статус выполнения процесса.</returns>
        public static bool Run(string executable) {
            s_process.StartInfo.FileName = "CMD.EXE";
            s_process.StartInfo.Arguments = $"/K \"{executable}\"";
            s_process.StartInfo.RedirectStandardOutput = false;
            s_process.StartInfo.CreateNoWindow = false;

            try {
                s_process.Start();
            }
            catch { 
                return false; 
            }

            return true;
        }

        /// <summary>
        /// Проверка легитимности выбранного компилятора.
        /// </summary>
        /// <param name="path">Путь к компилятору.</param>
        /// <returns>Версию компилятора или "undefined" в случае ошибки.</returns>
        public static string CheckCompiler(string path) {
            s_process.StartInfo.FileName = path;
            s_process.StartInfo.UseShellExecute = false;
            s_process.StartInfo.RedirectStandardOutput = true;
            s_process.StartInfo.CreateNoWindow = true;
            try {
                s_process.Start();
            }
            catch {
                return "undefined";
            }
            string output = "";
            bool completed = ExecuteWithTimeLimit(TimeSpan.FromMinutes(1), () => {
                output = s_process.StandardOutput.ReadToEnd();
            });

            if (!completed)
                return "undefined";
            return output.Split(" ")[6];
        }

        /// <summary>
        /// Таймер выполнения программы.
        /// </summary>
        /// <param name="timeSpan">Длительность таймера.</param>
        /// <param name="codeBlock">Выполняемпя функция.</param>
        /// <returns>true если программа успела выполнится в срок, иначе false</returns>
        private static bool ExecuteWithTimeLimit(TimeSpan timeSpan, Action codeBlock) {
            try {
                Task task = Task.Factory.StartNew(() => codeBlock());
                task.Wait(timeSpan);
                return task.IsCompleted;
            }
            catch (AggregateException ae) {
                throw ae.InnerExceptions[0];
            }
        }
    }
}
