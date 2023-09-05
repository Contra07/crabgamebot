using SteamAuth;
using SteamKit2.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamLibrary
{
    internal class SteamAuthAuthenticator : IAuthenticator
    {
        private SteamAccount _account;
        private Logger _logger;


        public SteamAuthAuthenticator(SteamAccount account, Logger logger)
        {
            _account = account;
            _logger = logger;
        }

        /// <inheritdoc />
        public Task<string> GetDeviceCodeAsync(bool previousCodeWasIncorrect)
        {
            if (previousCodeWasIncorrect)
            {
                _logger.Log("The previous 2-factor auth code you have provided is incorrect.");
            }

            string? code;

            do
            {
                _logger.Log("STEAM GUARD!");
                SteamGuardAccount guard;
                if (!string.IsNullOrEmpty(_account.Secret)) {
                    guard = new SteamGuardAccount()
                    {
                        SharedSecret = _account.Secret
                    };
                    code = guard.GenerateSteamGuardCode();
                    _logger.Log("Code: " + code);
                }
                else {
                    _logger.Log("Please enter your 2-factor auth code from your authenticator app: ");
                    code = Console.ReadLine();
                }

                if (code == null)
                {
                    break;
                }
            }
            while (string.IsNullOrEmpty(code));

            return Task.FromResult(code!);
        }

        /// <inheritdoc />
        public Task<string> GetEmailCodeAsync(string email, bool previousCodeWasIncorrect)
        {
            if (previousCodeWasIncorrect)
            {
                _logger.Log("The previous 2-factor auth code you have provided is incorrect.");
            }

            string? code;

            do
            {
                _logger.Log($"STEAM GUARD! Please enter the auth code sent to the email at {email}: ");
                code = Console.ReadLine()?.Trim();

                if (code == null)
                {
                    break;
                }
            }
            while (string.IsNullOrEmpty(code));

            return Task.FromResult(code!);
        }

        /// <inheritdoc />
        public Task<bool> AcceptDeviceConfirmationAsync()
        {
            _logger.Log("STEAM GUARD! Use the Steam Mobile App to confirm your sign in...");

            return Task.FromResult(false);
        }
    }
}
