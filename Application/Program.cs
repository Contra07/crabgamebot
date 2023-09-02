using System;
using System.Collections.Generic;
using System.Threading;
using SteamLibrary;

namespace Application;

public class Program
{
    public static void Main(string[] args){
        List<Credentials> users = JSONUtils.ReadUsers(args[0]);
        List<Thread> sessions = new List<Thread>();
        foreach(var user in users){
            sessions.Add(new Thread(new ThreadStart((new Session(user)).Connect)));
        }
        foreach(var session in sessions){
            session.Start();
        }
    }
}
