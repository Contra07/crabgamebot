using SteamKit2.Authentication;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SteamKit2.Internal.CMsgBluetoothDevicesData;

namespace SteamLibrary.Services
{
    public class LoginService
    {
        private SteamAccount _account;
        public LoginService(SteamAccount account) {
            _account = account;
            _account.CallbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
            _account.CallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
            _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            _account.CallbackManager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
        }

        async void OnConnected(SteamClient.ConnectedCallback callback)
        {
            Log("Connected to Steam! Logging in ...");

            // Begin authenticating via credentials
            CredentialsAuthSession authSession = await _account.SteamClient.Authentication.BeginAuthSessionViaCredentialsAsync(new AuthSessionDetails
            {
                Username = _account.Username,
                Password = _account.Password,
                IsPersistentSession = true,
                ClientOSType = EOSType.Windows10,
                Authenticator = new UserConsoleAuthenticator(),
            });

            // Starting polling Steam for authentication response
            AuthPollResult pollResponse = await authSession.PollingWaitForResultAsync();

            // Logon to Steam with the access token we have received
            // Note that we are using RefreshToken for logging on here
            _account.SteamClient.GetHandler<SteamUser>()?.LogOn(new SteamUser.LogOnDetails
            {
                Username = pollResponse.AccountName,
                AccessToken = pollResponse.RefreshToken,
            });
        }

        void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            _account.Disconnect();
            Log("Disconnected from Steam");
        }

        void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                Log($"Unable to logon to Steam: {callback.Result} / {callback.ExtendedResult}");
                _account.Disconnect();
                return;
            }
            Log("Successfully logged on!");
        }

        void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            Log($"Logged off of Steam: {callback.Result}");
        }

        private void Log(string log) { 
            Console.WriteLine($"[{_account.Username,15}]: {log}");
        }
    }
}
