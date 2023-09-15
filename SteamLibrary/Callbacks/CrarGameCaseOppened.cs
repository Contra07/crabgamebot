using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamLibrary.Callbacks
{
    public class CrarGameCaseOppened : CallbackMsg
    {
        public SteamUnifiedMessages.ServiceMethodResponse ParentCallback;
        internal CrarGameCaseOppened(SteamUnifiedMessages.ServiceMethodResponse callback)
        {
            JobID = callback.JobID;
            ParentCallback = callback;
        }
    }
}
