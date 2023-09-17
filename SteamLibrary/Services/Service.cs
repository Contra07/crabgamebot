using SteamKit2;
using SteamKit2.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamLibrary.Services
{
    public abstract class Service: IService
    {
        protected SteamAccount _account;
        protected Logger _logger;
        protected List<IDisposable> _callbackFunctionPointers;
        protected bool _isRunning;
        protected string _serviceName;

        public Service(SteamAccount account, Logger logger, string serviceName = "NoName")
        {
            _account = account;
            _logger = logger;
            _serviceName = serviceName;
            _isRunning = false;
            _callbackFunctionPointers = null;
        }

        public void Restart() {
            Stop();
            Start();
        }

        public bool Start()
        {
            if (_isRunning) { return false; }
            if (!(_callbackFunctionPointers is null)) { return false; }
            Subscribe();
            _logger.Log(_serviceName + " service started");
            return true;
        }

        protected abstract void Subscribe();

        public void Stop()
        {
            if (!(_callbackFunctionPointers is null))
            {
                _callbackFunctionPointers.Clear();
                _callbackFunctionPointers = null;
            }
            _isRunning = false;
            _logger.Log(_serviceName + " stopped");
        }
    }
}
