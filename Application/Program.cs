using System;
using System.Collections.Generic;
using SteamLibrary;

namespace Application;

public class Program
{
    public static void Main(string[] args){
        List<Account> users = JSONUtils.ReadUsers(args[0]);
        foreach(var user in users){
            Console.WriteLine($"{user.Login} : { user.Password}");
        }
    }
}
