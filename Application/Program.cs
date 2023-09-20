using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using SteamLibrary;
using SteamLibrary.DataBase;
using SteamLibrary.JSONs;
using SteamLibrary.Models;
using SteamLibrary.Services;
using FirebirdSql.Data;
using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.Common;
using FirebirdSql.Data.Services;

namespace Application
{

    public class Program
    {
        public static void Main(string[] args)
        {
           ;
            var db = new SteamDatabaseAccess(FirebirdClientFactory.Instance);
            List<Client> clients = new List<Client>();
            foreach (AccountCredentialsModel cred in db.GetAccountsCredentialsAsync().Result)
            {
                clients.Add(new Client(cred));
            }
            foreach (Client client in clients)
            {
                new Thread(client.Start).Start();
            }
            ////Aboba
            //new Thread(clients[0].Start).Start();
            //var run = true;
            //Thread th = new Thread(() =>
            //{
            //    while (run)
            //    {
            //        run = Console.ReadKey(false).KeyChar != 'p';
            //        Thread.Sleep(TimeSpan.FromMilliseconds(100));
            //    }
            //});
            //th.Start();
            //while (run)
            //{
            //    Thread.Sleep(TimeSpan.FromMilliseconds(100));
            //}
            //Console.WriteLine();
            //foreach (Client client in clients)
            //{
            //    client.Stop();
            //}
        }

        class Client
        {
            public SteamAccount Account { get; set; }
            public Logger Logger { get; set; }

            public List<IService> Services { get; set; }

            public Client(AccountCredentialsModel cred)
            {
                Account = new SteamAccount(cred);
                Logger = new Logger(Account);
                Services = new List<IService>() {
                new LoginService(Account, Logger),
                new ItemService(Account, Logger),
                new PlayService(Account, Logger),
                new EconService(Account, Logger),
                new EconomyService(Account, Logger)
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
                foreach (var service in Services)
                {
                    service.Stop();
                }
                Account.Disconnect();
            }
        }
    }

}
