using System;
using SteamKit2;
using SteamKit2.Authentication;

namespace SteamLibrary;

public class Session
{
    private Credentials _credentials;
    // steamclient instance
    private SteamClient _steamClient;
    // callback manager routes callbacks to function calls
    private CallbackManager _manager;
    // steamuser handler, which is used for logging on after successfully connecting
    private SteamUser _steamUser;
    // connection
    private bool _isRunning;

    public Session(Credentials account){
        _credentials = account;
        // create our steamclient instance
        _steamClient = new SteamClient();
        // create the callback manager which will route callbacks to function calls
        _manager = new CallbackManager(_steamClient);
        // get the steamuser handler, which is used for logging on after successfully connecting
        _steamUser = _steamClient.GetHandler<SteamUser>();
        _isRunning = false;
        // register a few callbacks we're interested in
        // these are registered upon creation to a callback manager, which will then route the callbacks
        // to the functions specified
        _manager.Subscribe<SteamClient.ConnectedCallback>( OnConnected );
        _manager.Subscribe<SteamClient.DisconnectedCallback>( OnDisconnected );
        _manager.Subscribe<SteamUser.LoggedOnCallback>( OnLoggedOn );
        _manager.Subscribe<SteamUser.LoggedOffCallback>( OnLoggedOff );
    }

    public void Connect(){
        // initiate the connection
        _steamClient.Connect();
        // create our callback handling loop
        _isRunning = true;
        while ( _isRunning )
        {
            // in order for the callbacks to get routed, they need to be handled by the manager
            _manager.RunWaitCallbacks( TimeSpan.FromSeconds( 1 ) );
        }
    }

    async void OnConnected( SteamClient.ConnectedCallback callback )
    {
        Console.WriteLine( "Connected to Steam! Logging in '{0}'...", _credentials.Login );

        // Begin authenticating via credentials
        CredentialsAuthSession authSession = await _steamClient.Authentication.BeginAuthSessionViaCredentialsAsync( new AuthSessionDetails
        {
            Username = _credentials.Login,
            Password = _credentials.Password,
            IsPersistentSession = false,
            Authenticator = new UserConsoleAuthenticator(),
        } );

        // Starting polling Steam for authentication response
        AuthPollResult pollResponse = await authSession.PollingWaitForResultAsync();

        // Logon to Steam with the access token we have received
        // Note that we are using RefreshToken for logging on here
        _steamUser.LogOn( new SteamUser.LogOnDetails
        {
            Username = pollResponse.AccountName,
            AccessToken = pollResponse.RefreshToken,
        } );       
    }

    void OnDisconnected( SteamClient.DisconnectedCallback callback )
    {
        Console.WriteLine( "Disconnected from Steam" );
        _isRunning = false;
    }

    void OnLoggedOn( SteamUser.LoggedOnCallback callback )
    {
        if ( callback.Result != EResult.OK )
        {
            Console.WriteLine( "Unable to logon to Steam: {0} / {1}", callback.Result, callback.ExtendedResult );
            _isRunning = false;
            return;
        }
        Console.WriteLine( "Successfully logged on!" );

        // at this point, we'd be able to perform actions on Steam
        
        // ...

        // for this sample we'll just log off
        _steamUser.LogOff();
    }

    void OnLoggedOff( SteamUser.LoggedOffCallback callback )
    {
        Console.WriteLine( "Logged off of Steam: {0}", callback.Result );
    }
}
