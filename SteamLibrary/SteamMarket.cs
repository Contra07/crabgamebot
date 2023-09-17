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
using static SteamKit2.GC.Dota.Internal.CMsgDOTASeasonPredictions.Prediction;
using static System.Net.WebRequestMethods;

namespace SteamLibrary
{
    public class SteamMarket
    {
        public static Dictionary<string, GameAsset> GetGameAssets(ulong appid) {
            Dictionary<string, GameAsset> itemList = new Dictionary<string, GameAsset>();
            string baseurl = "https://steamcommunity.com";
            string srevice = "https://steamcommunity.com/market/search/render/?query=appid%3A{0}&start={1}&count={2}&norender=1";
            int start = 0;
            int total = int.MaxValue;
            int assetsnumber = 100;
            while (start < total)
            {
                string req = string.Format(srevice, appid, start, assetsnumber);
                string answer = HttpGetResponse(baseurl, req);
                if (!(answer is null))
                {
                    GameAssetsPage list = JSONUtils.ParseGameAssetPage(answer);
                    total = list.total_count;
                    foreach (GameAsset result in list.results)
                    {
                        if (!itemList.ContainsKey(result.hash_name))
                        {
                            itemList.Add(result.hash_name, result);
                        }
                    }
                    start += list.pagesize;
                }
                else {
                    start = total;
                }            }

            return itemList;
        }
        public static Dictionary<string, GameItemDef> GetGameItemDefs(string appid, string digest) {
            Dictionary<string, GameItemDef> res = new Dictionary<string, GameItemDef>();
            List<GameItemDef> items = GetItemDefsArchive(appid, digest);
            if (!(items is null)) {
                foreach (var item in items) {
                    res.Add(item.itemdefid, item);
                }
            }
            return res;
        }

        public static List<GameItemDef> GetItemDefsArchive(string appid, string digest) {
            string baseadress = "https://api.steampowered.com";
            string reqteamplate = "https://api.steampowered.com/IGameInventory/GetItemDefArchive/v1?appid={0}&digest={1}";
            string req = string.Format(reqteamplate, appid, digest);
            string answer = HttpGetResponse(baseadress, req);
            if (!(answer is null))
            {
                return JSONUtils.ParseGameItemDefs(answer.Substring(0, answer.Length - 1));
            }
            else {
                return null;
            }
        }
        private static string HttpGetResponse(string baseurl, string req)
        {
            var http = new HttpClient()
            {
                BaseAddress = new Uri(baseurl),
            };
            try
            {
                Task<string> getAnswer = http.GetStringAsync(req);
                getAnswer.Wait();
                return getAnswer.Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
