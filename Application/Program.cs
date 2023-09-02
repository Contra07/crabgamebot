using System;
using System.Collections.Generic;
using System.Threading;
using SteamKit2;
using SteamLibrary;

namespace Application;

public class Program
{
    public static void Main(string[] args){
        List<Credentials> users = JSONUtils.ParseUsers(args[0]);
        List<Session> sessions = new List<Session>();
        foreach(var user in users){
            var session = new Session(user);
            new ItemHandler(session);
            sessions.Add(session);
        }
        foreach(var session in sessions){
            new Thread(new ThreadStart(session.Connect)).Start();
        }
    }
}
