using SteamKit2;
using SteamLibrary.JSONs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamLibrary
{
    public class SteamAccount
    {
        private Credentials _credentials;
        private SteamClient _steamClient;
        private CallbackManager _callbackManager;
        private Thread _proccess;
        private bool _isRunning;

        public string Username { get { return _credentials.Login; } }
        public string Password { get { return _credentials.Password; } }
        public SteamClient SteamClient { get { return _steamClient; } }
        public CallbackManager CallbackManager { get { return _callbackManager; } }

        public SteamAccount(Credentials credentials)
        {
            _credentials = credentials;
            _steamClient = new SteamClient();
            _callbackManager = new CallbackManager(_steamClient);
            _isRunning = false;
            _proccess = new Thread(Proccess);
        }

        public void Connect() {
            _isRunning = true;
            _proccess.Start();
            _steamClient.Connect();
        }

        public void Disconnect() {
            _isRunning = false;
            _steamClient.Disconnect();
        }

        private void Proccess()
        {
            while (_isRunning)
            {
                _callbackManager.RunWaitAllCallbacks(TimeSpan.FromSeconds(1));
            }
        }
    }
}
