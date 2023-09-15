using SteamKit2.Internal;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Linq;
using SteamLibrary.JSONs;
using System.Threading;
using SteamLibrary.Callbacks;

namespace SteamLibrary.Services
{
    public class ItemService: Service 
    {
        private SteamUnifiedMessages _steamUnifiedMessages;
        private SteamUnifiedMessages.UnifiedService<IInventory> _inventoryService;
        private JobID _getInventoryRequestID;
        private List<JobID> _consumePlaytimeRequestIDs;
        private JobID _exchangeItemRequestID;
        private JobID _getItemDefMetaRequestID;
        private Thread _thread;
        private int _tries;

        public ItemService(SteamAccount account, Logger logger): base(account, logger, "Item")
        {
            _exchangeItemRequestID = JobID.Invalid;
            _consumePlaytimeRequestIDs = new List<JobID>();
            _getInventoryRequestID = JobID.Invalid;
            _getItemDefMetaRequestID = JobID.Invalid;
            _steamUnifiedMessages = _account.SteamClient.GetHandler<SteamUnifiedMessages>();
            _inventoryService = _steamUnifiedMessages.CreateService<IInventory>();
            _thread = new Thread(CaseLoop);
            _tries = 7;
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
            if (_consumePlaytimeRequestIDs.Contains(callback.JobID)) {
                _consumePlaytimeRequestIDs.Remove(callback.JobID);
                GetCase(callback.GetDeserializedResponse<CInventory_Response>());
            }
            if (callback.JobID == _getInventoryRequestID) {
                _getInventoryRequestID = JobID.Invalid;
                _account.SteamClient.PostCallback(new CrarGameCaseOppened(callback));
                var cases = FindCases(callback.GetDeserializedResponse<CInventory_Response>());
                OpenCases(cases);
            }
            if (callback.JobID == _exchangeItemRequestID){
                _exchangeItemRequestID = JobID.Invalid;
                OpennedCase(callback.GetDeserializedResponse<CInventory_Response>());
            }
            if (callback.JobID == _getItemDefMetaRequestID) {
                _getItemDefMetaRequestID = JobID.Invalid;
                CInventory_GetItemDefMeta_Response resp = callback.GetDeserializedResponse<CInventory_GetItemDefMeta_Response>();
                var items = SteamMarket.GetGameItemDefs("1782210", resp.digest);
            }
        }

        private void CaseLoop()
        {
            try
            {
                int passes = 0;
                while (true)
                {
                    while (passes < _tries)
                    {
                        _getItemDefMetaRequestID =  GetItemDefMeta();
                        _consumePlaytimeRequestIDs.Add(ConsumePlaytime());
                        Thread.Sleep(TimeSpan.FromMinutes(15));
                        passes++;
                    }
                    _logger.Log("Whating next day");
                    Thread.Sleep(TimeSpan.FromHours(6));
                }
            }
            catch (ThreadInterruptedException ex)
            {
                _logger.Log("Case loop stopped");
            }
        }

        private async void GetCase(CInventory_Response resp) {
            _logger.Log($"Consuming playtime...");
            List<InventoryItemDef> items = JSONUtils.ParseInventoryItemDefs(resp.item_json);
            foreach (InventoryItemDef item in items)
            {
                _logger.Log($"Consumed {item.itemdefid}");
            }

            //var a = GetInventory();
            //CInventory_Response b = (await a).GetDeserializedResponse<CInventory_Response>();

            _getInventoryRequestID = GetInventory();
        }


        private List<InventoryItemDef> FindCases(CInventory_Response resp) {
            _logger.Log($"Checking inventory...");
            List<InventoryItemDef> items = JSONUtils.ParseInventoryItemDefs(resp.item_json);
            List<InventoryItemDef> cases = new List<InventoryItemDef>();
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
            return cases;
        }

        private void OpenCases(List<InventoryItemDef> cases) { 
            if (cases.Count() != 0)
            {
                foreach (var crabcase in cases)
                {
                    _logger.Log("Opening case: " + crabcase.itemdefid);
                    _exchangeItemRequestID = ExchangeItem(1302 - 1000 + ulong.Parse(crabcase.itemdefid), ulong.Parse(crabcase.itemid));
                }
            }
            else
            {
                _logger.Log("No cases");
            }
        }

        private void OpennedCase(CInventory_Response resp) {
            _logger.Log($"Openned case!");
            List<InventoryItemDef> items = JSONUtils.ParseInventoryItemDefs(resp.item_json);
            _logger.Log($"Consumed case: {items[0].itemdefid}");
            _logger.Log($"Got item: {items[1].itemdefid}");
        }

                

        private JobID ExchangeItem(ulong itemdefid, ulong itemid) {
            CInventory_ExchangeItem_Request req = new CInventory_ExchangeItem_Request {
                appid = 1782210,
                outputitemdefid = itemdefid,
                materialsitemid = { itemid },
                materialsquantity = { 1 }
            };
            return _inventoryService.SendMessage(x => x.ExchangeItem(req));
        }

        private JobID ConsumePlaytime(){
            CInventory_ConsumePlaytime_Request req = new CInventory_ConsumePlaytime_Request { 
                appid = 1782210, 
                itemdefid = 1200 
            };
            return _inventoryService.SendMessage(x => x.ConsumePlaytime(req));
        }

        private AsyncJob<SteamUnifiedMessages.ServiceMethodResponse> GetInventory() {
            CInventory_GetInventory_Request req = new CInventory_GetInventory_Request {
                appid = 1782210 
            };
            return _inventoryService.SendMessage(x => x.GetInventory(req));
        }

        private JobID GetItemDefMeta() {
            CInventory_GetItemDefMeta_Request req = new CInventory_GetItemDefMeta_Request {
                appid = 1782210,
            };
            return _inventoryService.SendMessage(x => x.GetItemDefMeta(req));
        }
    }
}
