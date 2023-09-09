using System;
using System.IO;

namespace SteamLibrary
{
    public class Logger
    {
        private static ConsoleColor _baseColor = (ConsoleColor)0;
        private SteamAccount _account;
        private ConsoleColor _color;
        private string _path;
        

        public Logger(SteamAccount account)
        {
            _account = account;
            _path = "./logs/" + account.Username + ".txt";
            _color = _baseColor++;
            if (_color == Console.BackgroundColor) {
                _baseColor++;
                _color++;
            }
        }

        public void Log(string log)
        {
            string template = $"[{DateTime.Now.TimeOfDay}][{_account.Username,25}]: {log}";
            ConsoleLog(template);
            //FileLog(template);
        }

        private void SaveLogs(){
            var dir = Directory.CreateDirectory("./logs");
            if (File.Exists(_path))
            {
                using (var file = File.Open(_path, FileMode.Open)) {
                    using (var fileNew = File.Open(dir.FullName + "/" + _account.Username + "-old.txt", FileMode.OpenOrCreate))
                    {
                        file.CopyTo(fileNew);
                    }
                }
                using (var nf = File.CreateText(_path)) { }
            }
        }

        private void FileLog(string log) {
            lock(_path){
                if (!File.Exists(_path))
                {
                    using (var f = File.CreateText(_path)) { }
                }
                using (var file = File.Open(_path, FileMode.Open))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        file.Position = file.Length;
                        writer.WriteLine(log);
                    }
                }
            }
        }

        private void ConsoleLog(string log) {
            lock (Console.Out)
            {
                Console.ForegroundColor = _color;
                Console.WriteLine(log);
                Console.ResetColor();
            }
        }
    }
}
