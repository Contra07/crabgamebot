using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamLibrary.Services
{
    public interface IService
    {
        bool Start();
        void Restart();
        void Stop();
    }
}
