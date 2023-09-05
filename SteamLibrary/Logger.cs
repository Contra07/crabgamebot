using System;

namespace SteamLibrary
{
    public class Logger
    {
        private SteamAccount _account;

        public Logger(SteamAccount account)
        {
            _account = account;
        }

        public void Log(string log)
        {
            Console.WriteLine($"[{_account.Username,25}]: {log}");
        }
    }
}
