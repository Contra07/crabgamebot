using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamLibrary.Services
{
    public class EconomyService: Service
    {

        public EconomyService(SteamAccount account, Logger logger): base(account, logger, "Economy") {

        }

        protected override void Subscribe()
        {
            _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLogon);
        }

        private void OnLogon(SteamUser.LoggedOnCallback callback) {
            if (callback.Result == EResult.OK) {
                //SteamMarket.GetExportedAssetsForUser(_account);
            }
        }

    }
}
