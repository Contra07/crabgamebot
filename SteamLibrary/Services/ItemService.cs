using SteamKit2.Internal;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using SteamLibrary.JSONs;
using System.Threading;
using SteamLibrary.Core;
using static SteamKit2.GC.Dota.Internal.CSerializedCombatLog;
using System.Threading.Tasks;

namespace SteamLibrary.Services
{
    public class ItemService: Service 
    {
        private const int _appid = 1782210;
        private const int _timeExchanger = 1200;
        private SteamUnifiedMessages.UnifiedService<IInventory> _inventoryService;
        private bool _runCaseLoop;
        private int _tries;
        private int _caseMinutesDelay;
        private int _dayHoursDelay;

        public ItemService(SteamAccount account, Logger logger): base(account, logger, "Item")
        {
            var steamUnifiedMessages = _account.SteamClient.GetHandler<SteamUnifiedMessages>();
            _inventoryService = steamUnifiedMessages.CreateService<IInventory>();
            _runCaseLoop = false;
            _tries = 3;
            _caseMinutesDelay = 30;
            _dayHoursDelay = 12;
        }

        protected override void Subscribe() {
            _callbackFunctionPointers = new List<IDisposable> {
                _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn),
                _account.CallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected)
            };
        }

        private async void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                _runCaseLoop = true;
                await CaseLoopScript(_tries, _caseMinutesDelay, _dayHoursDelay);
                _logger.Log("Starting case loop...");
            }
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback) {
            _runCaseLoop= false;
            _logger.Log("Case loop stopping...");
        }
        

        private async Task CaseLoopScript(int tries, int caseMinutesDelay, int dayHoursDelay) {
            _logger.Log($"Getting item defs...");
            var GameItemDefs = await GetGameItemDefsAsync(_appid);
            while (_runCaseLoop)
            {
                int passes = 0;
                while (passes < tries)
                {
                    _logger.Log($"Consuming playtime...");
                    var cases = await GetDrop(_appid, _timeExchanger);
                    if (cases.Count == 0)
                    {
                        _logger.Log($"No cases...");
                    }
                    foreach (var box in cases)
                    {
                        _logger.Log($"Opening case {box.itemdefid}");
                        var caseresp = await OpenCrabCase(_appid, box);
                        foreach (var item in caseresp)
                        {
                            _logger.Log($"From case {box.itemdefid}: {item.origin} {item.itemdefid}");
                        }
                    }
                    passes++;

                    _logger.Log($"Wating for {caseMinutesDelay} minutes");
                    await Delay(TimeSpan.FromMinutes(caseMinutesDelay));
                }
                _logger.Log($"Wating next day for {dayHoursDelay} hours");
                await Delay(TimeSpan.FromHours(dayHoursDelay));
            }
        }

        private Task Delay(TimeSpan time) {
            return Task.Delay(time);
        }

        private async Task<List<InventoryItemDef>> GetDrop(uint appid, ulong itemdefid) {
            var resp = (await _inventoryService.ConsumePlaytime(appid, itemdefid)).GetDeserializedResponse<CInventory_Response>();
            //if (string.IsNullOrEmpty(resp.item_json))
            //{
            //    return new List<InventoryItemDef>();
            //}
            return JSONUtils.ParseInventoryItemDefs(resp.item_json);
        }

        private List<InventoryItemDef> FindCases(CInventory_Response resp, List<string> itemdefids)
        {
            List<InventoryItemDef> items = JSONUtils.ParseInventoryItemDefs(resp.item_json);
            return items.Where(x => itemdefids.Contains(x.itemdefid)).ToList();
        }

        private async Task<List<InventoryItemDef>> OpenCrabCase(uint appid, InventoryItemDef box) {
            var resp = (await _inventoryService.ExchangeItem(
                    appid,
                    1302 - 1000 + ulong.Parse(box.itemdefid),
                    new Dictionary<ulong, uint> {
                        { ulong.Parse(box.itemid), 1}
                    }
            )).GetDeserializedResponse<CInventory_Response>();
            //if (string.IsNullOrEmpty(resp.item_json)) {
            //    return new List<InventoryItemDef>();
            //}
            return JSONUtils.ParseInventoryItemDefs(resp.item_json);
        }

        private async Task<Dictionary<string, GameItemDef>> GetGameItemDefsAsync(uint appid) {
            var items = new Dictionary<string, GameItemDef>();
            try {
                var ItemDefMetaResponse = await _inventoryService.GetItemDefMeta(appid);
                if (ItemDefMetaResponse.Result == EResult.OK)
                {
                    items = SteamMarket.GetGameItemDefs(appid.ToString(), (ItemDefMetaResponse.GetDeserializedResponse<CInventory_GetItemDefMeta_Response>()).digest);
                }
            }
            catch {
                _logger.Log("Task GetItemDefMeta was canceled");
            }
            
            return items;
        }
    }
}
