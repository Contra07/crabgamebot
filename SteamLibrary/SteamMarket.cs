using SteamKit2;
using SteamKit2.Internal;
using SteamLibrary.JSONs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SteamLibrary
{
    public class SteamMarket
    {
        //https://steamcommunity.com/market/search/render/?query=appid%3A1782210&start=900&count=100&norender=1

        //https://steamcommunity.com/market/priceoverview/?market_hash_name={}&appid={}&currency=1

        private static Dictionary<string, string> _itemList = GetItems();

        private static object _lock = new object();

        public static Dictionary<string, string> GetItems() {
            _lock = new object();
            lock (_lock)
            {
                Dictionary<string, string> itemList = new Dictionary<string, string>();
                int start = 0;
                int total = 917;
                string bases = "https://steamcommunity.com/market/search/render/?query=appid%3A1782210&start=";

                while (start < total)
                {
                    string req = bases + start + "&count=100&norender=1";
                    var http = new HttpClient();
                    http.BaseAddress = new Uri("https://steamcommunity.com");
                    Task<string> getAnswer = http.GetStringAsync(req);
                    getAnswer.Wait();
                    string answer = getAnswer.Result;
                    ItemList list = JSONUtils.ParseItemList(answer);
                    List<Result> results = list.results;
                    total = list.total_count;
                    foreach (Result result in results)
                    {
                        if (!itemList.ContainsKey(result.hash_name))
                        {
                            itemList.Add(result.hash_name, result.sell_price_text);
                        }
                        else {
                            var a = itemList[result.hash_name];
                        }
                        
                    }
                    start += list.pagesize;
                }

                return itemList;
            }
        }

        public static string Test(CEconItem_Description desc) {

            lock (_lock)
            {
                if (_itemList.ContainsKey(desc.market_hash_name)) {
                    return _itemList[desc.market_hash_name];
                }
                string bases = "https://steamcommunity.com/market/priceoverview/?market_hash_name=";
                string req = bases + desc.market_hash_name + "&appid=" + desc.appid + "&currency=" + desc.currency;

                bool run = true;
                string answer = null;

                while (run)
                {
                    try
                    {
                        var http = new HttpClient();
                        http.BaseAddress = new Uri("https://steamcommunity.com");
                        Task<string> getAnswer = http.GetStringAsync(req);
                        getAnswer.Wait();
                        answer = getAnswer.Result;
                        run = false;
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                    catch (Exception ex)
                    {
                        run = false;
                        //Thread.Sleep(TimeSpan.FromSeconds(15));
                    }
                }

                if (answer is null)
                {
                    return "";
                }

                return JSONUtils.ParsePrice(answer).lowest_price;
            }

            
        }
    }
}
