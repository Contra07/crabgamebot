using SteamKit2.Internal;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamLibrary.JSONs;
using System.Threading;

namespace SteamLibrary.Services
{
    public class ItemService
    {
        private SteamUnifiedMessages _steamUnifiedMessages;
        private SteamUnifiedMessages.UnifiedService<IInventory> _inventoryService;
        private JobID _inventoryRequestID;
        private JobID _consumePlaytimeRequestID;
        private JobID _exchangeItemID;
        private SteamAccount _account;

        public ItemService(SteamAccount account)
        {
            _account = account;
            _exchangeItemID = JobID.Invalid;
            _consumePlaytimeRequestID = JobID.Invalid;
            _inventoryRequestID = JobID.Invalid;
            _steamUnifiedMessages = _account.SteamClient.GetHandler<SteamUnifiedMessages>();
            _inventoryService = _steamUnifiedMessages.CreateService<IInventory>();
            _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            _account.CallbackManager.Subscribe<SteamUnifiedMessages.ServiceMethodResponse>(OnUnifiedMessageResponse);
        }

        public void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                new Thread(() => {
                    ConsumeTime();
                    GetInventory();
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                }).Start();
            }
        }

        public void OnUnifiedMessageResponse(SteamUnifiedMessages.ServiceMethodResponse callback)
        {
            if (callback.JobID == _consumePlaytimeRequestID) {
                CInventory_Response resp = callback.GetDeserializedResponse<CInventory_Response>();
                Log($"Consuming playtime... {resp.etag} ,{resp.itemdef_json} , {resp.ticket}, {resp.replayed}");
                foreach (var id in resp.removeditemids) {
                    Log($"removed: {id}");
                }
                List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
                foreach (InventoryItem item in items) {
                    Log($"Consumed {item.itemdefid}");
                }
                _consumePlaytimeRequestID = JobID.Invalid;
            }
            if (callback.JobID == _inventoryRequestID) {
                Log($"Checking inventory...");
                CInventory_Response resp = callback.GetDeserializedResponse<CInventory_Response>();
                List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
                List<InventoryItem> cases = new List<InventoryItem>();
                foreach (var item in items)
                {
                    if (item.itemdefid == "1000" ||
                        item.itemdefid == "1001" ||
                        item.itemdefid == "1002" ||
                        item.itemdefid == "1003")
                    {
                        cases.Add(item);
                        Log($"case: {item.itemdefid}");
                    }
                }
                if (cases.Count() != 0)
                {
                    foreach (var crabcase in cases)
                    {
                        CInventory_ExchangeItem_Request req = new CInventory_ExchangeItem_Request
                        {
                            appid = 1782210,
                            outputitemdefid = 1302 - 1000 + ulong.Parse(crabcase.itemdefid)
                        };
                        req.materialsitemid.Add(ulong.Parse(crabcase.itemid));
                        req.materialsquantity.Add(1);
                        Log("Opening case...");
                        Log(JSONUtils.SerializeInventoryItem(crabcase));
                        _exchangeItemID = _inventoryService.SendMessage(x => x.ExchangeItem(req));
                    }
                }
                else {
                    Log("No cases");
                }
                _inventoryRequestID = JobID.Invalid;
            }
            if (callback.JobID == _exchangeItemID){
                CInventory_Response resp = callback.GetDeserializedResponse<CInventory_Response>();
                Log($"Openned case! {resp.etag}");
                List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
                foreach (InventoryItem item in items){
                    Log(JSONUtils.SerializeInventoryItem(item));
                }
                _exchangeItemID = JobID.Invalid;
            }
        }
        private void ConsumeTime(){
            CInventory_ConsumePlaytime_Request req2 = new CInventory_ConsumePlaytime_Request { appid = 1782210, itemdefid = 1200 };
            _consumePlaytimeRequestID = _inventoryService.SendMessage(x => x.ConsumePlaytime(req2));
        }

        private void GetInventory() {
            CInventory_GetInventory_Request req3 = new CInventory_GetInventory_Request { appid = 1782210 };
            _inventoryRequestID = _inventoryService.SendMessage(x => x.GetInventory(req3));
        }

        private void Log(string log)
        {
            Console.WriteLine($"[{_account.Username,15}]: {log}");
        }
    }
}
