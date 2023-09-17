using SteamKit2;
using SteamKit2.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SteamLibrary.Services
{
    public class PlayService: Service
    {
        private Thread _thread;
        private int _appid;
        public PlayService(SteamAccount account, Logger logger): base (account, logger, "Play") {
            _thread = new Thread(GameLoop);
            _appid = 1782210;
        }

        protected override void Subscribe()
        {
            _callbackFunctionPointers = new List<IDisposable> {
                _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLogon),
                _account.CallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnect)
            };
        }

        private void OnDisconnect(SteamClient.DisconnectedCallback callback) {
            _thread.Interrupt();
            _logger.Log("Stopping playing game");
        }

        private void OnLogon(SteamUser.LoggedOnCallback callback) {

            if (callback.Result == EResult.OK) {
                PlayGame();
                _thread.Start();
            }
        }

        private void PlayGame() {
            ClientMsgProtobuf<CMsgClientGamesPlayed> request = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayedWithDataBlob)
            {
                Body = {
				        // Underflow here is to be expected, this is Steam's logic
				        client_os_type = unchecked((uint) EOSType.Windows10)
                    }
            };
            request.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed { game_id = new GameID(_appid) });
            _account.SteamClient.Send(request);
            _logger.Log($"Start playing game");
        }

        private void GameLoop() {
            try
            {
                TimeSpan time = TimeSpan.FromHours(12);
                while (true)
                {
                    _logger.Log($"Started play game for {time}");
                    Thread.Sleep(time);
                }
            }
            catch(ThreadInterruptedException ex) {
                _logger.Log($"Stoped playing game");
            }
            
        }

    }
}
