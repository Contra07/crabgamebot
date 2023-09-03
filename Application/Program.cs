using System;
using System.Collections.Generic;
using System.Threading;
using SteamKit2;
using SteamLibrary;
using SteamLibrary.JSONs;
using SteamLibrary.Services;

namespace Application;

public class Program
{
    public static void Main(string[] args){
        List<SteamAccount> accounts = new List<SteamAccount>();
        foreach (Credentials cred in JSONUtils.ParseUsers(args[0])) {
            var account = new SteamAccount(cred);
            new LoginService(account);
            new ItemService(account);
            new PlayService(account);
            accounts.Add(account);
        }
        foreach (SteamAccount account in accounts){
            account.Connect();
        }

        //List<Session> sessions = new List<Session>();
        //foreach(var user in accounts){
        //    var session = new Session(user);
        //    var item = new ItemHandler(session);
        //    new Thread(new ThreadStart(item.Start)).Start();
        //    sessions.Add(session);
        //}
        //foreach(var session in sessions){
        //    new Thread(new ThreadStart(session.Start)).Start();
        //}
        //foreach (var session in sessions)
        //{
        //    session.Connect();
        //}
    }
}
