using SteamKit2.Authentication;
using SteamKit2;
using System;
using System.Collections.Generic;

namespace SteamLibrary.Services
{
    public class LoginService : Service
    {
        public LoginService(SteamAccount account, Logger logger) : base(account, logger, "Login") { }

        protected override void Subscribe()
        {
            _callbackFunctionPointers = new List<IDisposable> {
                _account.CallbackManager.Subscribe<SteamClient.ConnectedCallback>(OnConnected),
                _account.CallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected),
                _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn),
                _account.CallbackManager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff),
            };
        }

        private async void OnConnected(SteamClient.ConnectedCallback callback)
        {
            _logger.Log("Connected to Steam! Logging in ...");

            // Begin authenticating via credentials
            CredentialsAuthSession authSession = await _account.SteamClient.Authentication.BeginAuthSessionViaCredentialsAsync(new AuthSessionDetails
            {
                Username = _account.Username,
                Password = _account.Password,
                IsPersistentSession = true,
                ClientOSType = EOSType.Windows10,
                Authenticator = new SteamAuthAuthenticator(_account, _logger),
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

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {
            _account.Disconnect();
            _logger.Log("Disconnected from Steam");
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result != EResult.OK)
            {
                _logger.Log($"Unable to logon to Steam: {callback.Result} / {callback.ExtendedResult}");
                _account.Disconnect();
                return;
            }
            _logger.Log("Successfully logged on!");
        }

        private void OnLoggedOff(SteamUser.LoggedOffCallback callback)
        {
            _logger.Log($"Logged off of Steam: {callback.Result}");
        }
    }
}
