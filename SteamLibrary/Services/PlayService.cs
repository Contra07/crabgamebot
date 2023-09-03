using SteamKit2;
using SteamKit2.Authentication;
using SteamKit2.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SteamKit2.GC.Dota.Internal.CMsgDOTABotDebugInfo;

namespace SteamLibrary.Services
{
    public class PlayService
    {
        private SteamAccount _account;

        public PlayService(SteamAccount account)
        {
            _account = account;
            _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLogon);
        }

        public void OnLogon(SteamUser.LoggedOnCallback callback) {

            if (callback.Result == EResult.OK) {
                new Thread(PlayGame).Start();
            }
        }

        public void PlayGame() {
            ClientMsgProtobuf<CMsgClientGamesPlayed> request = new(EMsg.ClientGamesPlayedWithDataBlob)
            {
                Body = {
				        // Underflow here is to be expected, this is Steam's logic
				        client_os_type = unchecked((uint) EOSType.Windows10)
                    }
            };
            request.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed { game_id = new GameID(1782210) });
            _account.SteamClient.Send(request);
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(300));
            }
        }
    }
}
