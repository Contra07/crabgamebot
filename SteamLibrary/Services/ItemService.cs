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
    public class ItemService: Service
    {
        private SteamUnifiedMessages _steamUnifiedMessages;
        private SteamUnifiedMessages.UnifiedService<IInventory> _inventoryService;
        private JobID _inventoryRequestID;
        private JobID _consumePlaytimeRequestID;
        private JobID _exchangeItemID;
        private Thread _thread;

        public ItemService(SteamAccount account, Logger logger): base(account, logger, "Item")
        {
            _exchangeItemID = JobID.Invalid;
            _consumePlaytimeRequestID = JobID.Invalid;
            _inventoryRequestID = JobID.Invalid;
            _steamUnifiedMessages = _account.SteamClient.GetHandler<SteamUnifiedMessages>();
            _inventoryService = _steamUnifiedMessages.CreateService<IInventory>();
            _thread = new Thread(CaseLoop);
        }

        protected override void Subscribe() {
            _callbackFunctionPointers = new List<IDisposable> {
                _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn),
                _account.CallbackManager.Subscribe<SteamUnifiedMessages.ServiceMethodResponse>(OnUnifiedMessageResponse),
                _account.CallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected)
            };
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                _thread.Start();
                _logger.Log("Started case loop");
            }
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback) {
            _logger.Log("Case loop stopping...");
            _thread.Interrupt();
        }


        private void OnUnifiedMessageResponse(SteamUnifiedMessages.ServiceMethodResponse callback)
        {
            if (callback.JobID == _consumePlaytimeRequestID) {
                CInventory_Response resp = callback.GetDeserializedResponse<CInventory_Response>();
                _logger.Log($"Consuming playtime...");
                List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
                foreach (InventoryItem item in items) {
                    _logger.Log($"Consumed {item.itemdefid}");
                }
                _consumePlaytimeRequestID = JobID.Invalid;
                GetInventory();
            }
            if (callback.JobID == _inventoryRequestID) {
                _logger.Log($"Checking inventory...");
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
                        _logger.Log($"Found case: {item.itemdefid}");
                    }
                }
                if (cases.Count() != 0)
                {
                    foreach (var crabcase in cases)
                    {
                        _logger.Log("Opening case: " + crabcase.itemdefid);
                        OpenCase(crabcase.itemdefid, crabcase.itemid);
                    }
                }
                else {
                    _logger.Log("No cases");
                }
                _inventoryRequestID = JobID.Invalid;
            }
            if (callback.JobID == _exchangeItemID){
                CInventory_Response resp = callback.GetDeserializedResponse<CInventory_Response>();
                _logger.Log($"Openned case!");
                List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
                _logger.Log($"Consumed case: {items[0].itemdefid}");
                _logger.Log($"Got item: {items[1].itemdefid}");
                _exchangeItemID = JobID.Invalid;
            }
        }

        private void CaseLoop() {
            try {
                while (true)
                {
                    ConsumeTime();
                    GetInventory();
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                }
            }
            catch (ThreadInterruptedException ex)
            {
                _logger.Log("Case loop stopped");
            }
            
        }

        private void OpenCase(string caseid, string itemid) {
            CInventory_ExchangeItem_Request req = new CInventory_ExchangeItem_Request
            {
                appid = 1782210,
                outputitemdefid = 1302 - 1000 + ulong.Parse(caseid)
            };
            req.materialsitemid.Add(ulong.Parse(itemid));
            req.materialsquantity.Add(1);
            _exchangeItemID = _inventoryService.SendMessage(x => x.ExchangeItem(req));
        }

        private void ConsumeTime(){
            CInventory_ConsumePlaytime_Request req2 = new CInventory_ConsumePlaytime_Request { appid = 1782210, itemdefid = 1200 };
            _consumePlaytimeRequestID = _inventoryService.SendMessage(x => x.ConsumePlaytime(req2));
        }

        private void GetInventory() {
            CInventory_GetInventory_Request req3 = new CInventory_GetInventory_Request { appid = 1782210 };
            _inventoryRequestID = _inventoryService.SendMessage(x => x.GetInventory(req3));
        }
    }
}
