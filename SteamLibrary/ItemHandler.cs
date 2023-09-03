using System;
using System.Collections.Generic;
using SteamKit2;
using SteamKit2.Internal;
using SteamLibrary.JSONs;

namespace SteamLibrary;

public class ItemHandler
{
    private SteamUnifiedMessages _steamUnifiedMessages;
    private SteamUnifiedMessages.UnifiedService<IInventory> _inventoryService;
    private JobID _inventoryRequest;
    private JobID _consumePlaytimeRequest;
    private Session _session;
    private CallbackManager _manager;
    private bool _isRunning = false;
    // private ulong[,] caseid = new ulong[4, 100];    
    public ItemHandler(Session session){
        _manager.Subscribe<SteamUser.LoggedOnCallback>( OnLoggedOn);
        _steamUnifiedMessages = session.Client.GetHandler<SteamUnifiedMessages>();
        _inventoryService = _steamUnifiedMessages.CreateService<IInventory>();
        var c = _manager.Subscribe<SteamUnifiedMessages.ServiceMethodResponse>( OnMethodResponse );
        _manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);
        _session = session;
        _inventoryRequest = JobID.Invalid;
        _consumePlaytimeRequest = JobID.Invalid;
    }

    public void Start() {
        _isRunning = true;
        while (_isRunning) {
            _manager.RunWaitAllCallbacks(TimeSpan.FromMilliseconds(1000));
        }
    }

    public void OnLoggedOn( SteamUser.LoggedOnCallback callback )
    {
        if ( callback.Result == EResult.OK )
        {
            CInventory_ConsumePlaytime_Request req2 = new CInventory_ConsumePlaytime_Request { appid = 1782210, itemdefid = 1200 };
            _consumePlaytimeRequest = _inventoryService.SendMessage(x => x.ConsumePlaytime(req2));
            CInventory_GetInventory_Request req3 = new CInventory_GetInventory_Request { appid = 1782210 };
            _inventoryRequest = _inventoryService.SendMessage(x => x.GetInventory(req3));
        }
    }

    public void OnDisconnected(SteamClient.DisconnectedCallback callback)
    {
        Log("Item Disconnected");
        _isRunning = false;
    }

    public void OnMethodResponse(SteamUnifiedMessages.ServiceMethodResponse callback)
    {
        Log("Result: " + callback.Result + " JobID: " + callback.JobID + " : " + (callback.JobID == _inventoryRequest));
        if (callback.JobID != _inventoryRequest || callback.JobID != _consumePlaytimeRequest)
        {
            return;
        }
        //CInventory_GetItemDefMeta_Response resp = callback.GetDeserializedResponse<CInventory_GetItemDefMeta_Response>();
        //Log(resp.digest + " " + resp.modified);
        
        CInventory_Response resp2 = callback.GetDeserializedResponse<CInventory_Response>();
        //JSON response

        List<InventoryItem> items = JSONUtils.ParseInventory(resp2.item_json);
        List<InventoryItem> cases = new List<InventoryItem>();
        foreach(var item in items){
            if(item.itemdefid == "1000" ||
                item.itemdefid == "1001" ||
                item.itemdefid == "1002" ||
                item.itemdefid == "1003")
            {
                cases.Add(item);
            }
        }

        // Case open

        foreach(var crabcase in cases){
             CInventory_ExchangeItem_Request req = new CInventory_ExchangeItem_Request { 
                    appid = 1782210, 
                    outputitemdefid = 1302-1000+ulong.Parse(crabcase.itemdefid) 
                    };
                req.materialsitemid.Add(ulong.Parse(crabcase.itemid));
                req.materialsquantity.Add(1);
                _inventoryRequest = _inventoryService.SendMessage(x => x.ExchangeItem(req));
                Log(JSONUtils.SerializeInventoryItem(crabcase));
        }
        _inventoryRequest = JobID.Invalid;
        _session.Client.GetHandler<SteamUser>()?.LogOff();
    }

    private void Log(string log){
        Console.WriteLine(_session.Account.Login +": " + log);
    }
}
