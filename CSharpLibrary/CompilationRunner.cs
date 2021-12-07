using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSharpLibrary {
    public class CompilationRunner {

        private static Process s_compiler = new();

        public static string Build(string path, string source, string executable) {
            s_compiler.StartInfo.UseShellExecute = false;
            s_compiler.StartInfo.CreateNoWindow = true;
            s_compiler.StartInfo.RedirectStandardOutput = true;
            s_compiler.StartInfo.RedirectStandardError = true;
            s_compiler.StartInfo.FileName = path;
            s_compiler.StartInfo.Arguments = $"{source} /t:exe /out:{executable}";

            try {
                s_compiler.Start();
            }
            catch {
                return null;
            }

            return s_compiler.StandardOutput.ReadToEnd();
        }

        public static bool Run(string executable) {
            s_compiler.StartInfo.FileName = "CMD.EXE";
            s_compiler.StartInfo.Arguments = $"/K {executable}";
            s_compiler.StartInfo.RedirectStandardOutput = false;
            s_compiler.StartInfo.CreateNoWindow = false;


            try {
                s_compiler.Start();
            }
            catch { 
                return false; 
            }

            return true;
        }

        public static string CheckCompiler(string path) {
            s_compiler.StartInfo.FileName = path;
            s_compiler.StartInfo.UseShellExecute = false;
            s_compiler.StartInfo.RedirectStandardOutput = true;
            s_compiler.StartInfo.CreateNoWindow = true;
            try {
                s_compiler.Start();
            }
            catch {
                return "undefined";
            }
            string output = "";
            bool completed = ExecuteWithTimeLimit(TimeSpan.FromMinutes(1), () => {
                output = s_compiler.StandardOutput.ReadToEnd();
            });

            if (!completed)
                return "undefined";
            return output.Split(" ")[6];
        }

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

        private static string ChangeEncoding(string str, Encoding en1, Encoding en2) {
            byte[] bytes = en1.GetBytes(str);
            return en2.GetString(bytes);
        }
    }
}
