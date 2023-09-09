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
        private JobID _getInventoryRequestID;
        private List<JobID> _consumePlaytimeRequestIDs;
        private JobID _exchangeItemRequestID;
        private JobID _getItemDefMetaRequestID;
        private JobID _getEligiblePromoItemDefIDsRequestID;
        private List<JobID> _inspectItemRequestIDs;
        private Thread _thread;
        private int _tries;

        public ItemService(SteamAccount account, Logger logger): base(account, logger, "Item")
        {
            _exchangeItemRequestID = JobID.Invalid;
            _consumePlaytimeRequestIDs = new List<JobID>();
            _getInventoryRequestID = JobID.Invalid;
            _getItemDefMetaRequestID = JobID.Invalid;
            _getEligiblePromoItemDefIDsRequestID = JobID.Invalid;
            _inspectItemRequestIDs = new List<JobID>();
            _steamUnifiedMessages = _account.SteamClient.GetHandler<SteamUnifiedMessages>();
            _inventoryService = _steamUnifiedMessages.CreateService<IInventory>();
            _steamUnifiedMessages.CreateService<IMarketingMessages>();
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
                //InspectEachItem(callback.GetDeserializedResponse<CInventory_Response>());
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
            }
            if (_inspectItemRequestIDs.Contains(callback.JobID)) {
                _inspectItemRequestIDs.Remove(callback.JobID);
                CInventory_Response resp = callback.GetDeserializedResponse<CInventory_Response>();
                _logger.Log(resp.itemdef_json.ToString());
                //List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
            }
            if (callback.JobID == _getEligiblePromoItemDefIDsRequestID) {
                CInventory_GetEligiblePromoItemDefIDs_Response resp = callback.GetDeserializedResponse<CInventory_GetEligiblePromoItemDefIDs_Response>();
                _getEligiblePromoItemDefIDsRequestID = JobID.Invalid;
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
                        //_GetItemDefMetaRequestID =  GetItemDefMeta();
                        _consumePlaytimeRequestIDs.Add(ConsumePlaytime());
                        _getEligiblePromoItemDefIDsRequestID = GetEligiblePromoItemDefIDs();
                        //GetInventory();
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

        private void GetCase(CInventory_Response resp) {
            _logger.Log($"Consuming playtime...");
            List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
            foreach (InventoryItem item in items)
            {
                _logger.Log($"Consumed {item.itemdefid}");
            }
            _getInventoryRequestID = GetInventory();
        }


        private List<InventoryItem> FindCases(CInventory_Response resp) {
            _logger.Log($"Checking inventory...");
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
            return cases;
        }

        private void OpenCases(List<InventoryItem> cases) { 
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
            List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
            _logger.Log($"Consumed case: {items[0].itemdefid}");
            _logger.Log($"Got item: {items[1].itemdefid}");
        }

        private void InspectEachItem(CInventory_Response resp) {
            List<InventoryItem> items = JSONUtils.ParseInventory(resp.item_json);
            foreach (var item in items)
            {
                _logger.Log($"Inspecting item: {item.itemdefid}");
                _inspectItemRequestIDs.Add(InspectItem(ulong.Parse(item.itemid), ulong.Parse(item.itemdefid), item.tags));
                Thread.Sleep(100);
            }
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

        private JobID GetInventory() {
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

        private JobID InspectItem(ulong itemid, ulong itemdefid, string tags)
        {
            CInventory_InspectItem_Request req = new CInventory_InspectItem_Request
            {
                itemid = itemid,
                itemdefid = itemdefid,
                tags = tags
            };
            return _inventoryService.SendMessage(x => x.InspectItem(req));
        }

        private JobID GetEligiblePromoItemDefIDs()
        {
            CInventory_GetEligiblePromoItemDefIDs_Request req = new CInventory_GetEligiblePromoItemDefIDs_Request();
            { };
            return _inventoryService.SendMessage(x => x.GetEligiblePromoItemDefIDs(req));
        }
    }
}
