﻿using System;
using System.Collections.Generic;
using System.Threading;
using SteamLibrary;
using SteamLibrary.JSONs;
using SteamLibrary.Services;

namespace Application;

public class Program
{
    public static void Main(string[] args){
        List<Client> clients = new List<Client>();
        foreach (Credentials cred in JSONUtils.ParseUsers(args[0])) {
            clients.Add(new Client(cred));
        }
        foreach (Client client in clients)
        {
            new Thread(client.Start).Start(); 
        }
        while (Console.ReadKey().KeyChar != 'p') { }
        Console.WriteLine();
        foreach (Client client in clients)
        {
            client.Stop();
        }
    }

    class Client {
        public SteamAccount Account { get; set; }
        public Logger Logger { get; set; }

        public List<IService> Services { get; set;}

        public Client(Credentials cred) {
            Account = new SteamAccount(cred);
            Logger = new Logger(Account);
            Services = new List<IService>() {
                new LoginService(Account, Logger),
                new ItemService(Account, Logger),
                new PlayService(Account, Logger),
            };
        }

        public void Start()
        {
            foreach (var service in Services)
            {
                service.Start();
            }
            Account.Connect();
        }

        public void Stop()
        {
            foreach(var service in Services)
            {
                service.Stop();
            }
            Account.Disconnect();
        }
    }
}
