using System;
using System.Collections.Generic;
using SteamKit2;
using SteamKit2.Internal;

namespace SteamLibrary;

public class ItemHandler
{
    private SteamUnifiedMessages _steamUnifiedMessages;
    private SteamUnifiedMessages.UnifiedService<IInventory> _inventoryService;
    private JobID _inventoryRequest;
    private Session _session;
    // private ulong[,] caseid = new ulong[4, 100];    
    public ItemHandler(Session session){
        session.Manager.Subscribe<SteamUser.LoggedOnCallback>( OnLoggedOn);
        _steamUnifiedMessages = session.Client.GetHandler<SteamUnifiedMessages>();
        _inventoryService = _steamUnifiedMessages.CreateService<IInventory>();
        session.Manager.Subscribe<SteamUnifiedMessages.ServiceMethodResponse>( OnMethodResponse );
        _session = session;
        _inventoryRequest = JobID.Invalid;
    }

    public void OnLoggedOn( SteamUser.LoggedOnCallback callback )
    {
        if ( callback.Result == EResult.OK )
        {
            CInventory_ConsumePlaytime_Request req2 = new CInventory_ConsumePlaytime_Request { appid = 1782210, itemdefid = 1200 };
            _inventoryRequest = _inventoryService.SendMessage(x => x.ConsumePlaytime(req2));
            CInventory_GetInventory_Request req3 = new CInventory_GetInventory_Request { appid = 1782210 };
            _inventoryRequest = _inventoryService.SendMessage(x => x.GetInventory(req3));
        }
    }

    public void OnMethodResponse(SteamUnifiedMessages.ServiceMethodResponse callback)
    {
        Log("Result: " + callback.Result + " JobID: " + callback.JobID + " : " + (callback.JobID == _inventoryRequest));
        if (callback.JobID != _inventoryRequest)
        {
            return;
        }
        CInventory_GetItemDefMeta_Response resp = callback.GetDeserializedResponse<CInventory_GetItemDefMeta_Response>();
        Log(resp.digest + " " + resp.modified);
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

        // int o = 1;
        // int k = 0;
        // for (int j = 0; j < resp2.item_json.Length; j++)
        // {
        //     if (resp2.item_json[j] == '{')
        //     {
        //         k = 0;
        //     }
        //     if (resp2.item_json[j] == '}')
        //     {
        //         k++;
        //     }
        //     if ((k == 1) && (resp2.item_json[j] == ','))
        //     {
        //         o++;
        //     }
        // }
        // string[,] output = new string[o, 100];
        // o = 0;
        // k = 0;
        // for (int j = 0; j < resp2.item_json.Length; j++)
        // {
        //     if (resp2.item_json[j] != '[' && resp2.item_json[j] != ']')
        //     {
        //         if (resp2.item_json[j] == '}')
        //         {
        //             o++;
        //         }
        //         else if (resp2.item_json[j] == '{')
        //         {
        //             k = 0;
        //         }
        //         else if ((resp2.item_json[j] == ',') || (resp2.item_json[j] == ':'))
        //         {
        //             k++;

        //         }
        //         else if (resp2.item_json[j] != '"')
        //         {
        //             output[o, k] += resp2.item_json[j];
        //         }
        //     }
        // }
        // caseid = new ulong[4, 20];
        // for (int j = 0; j < output.GetLength(0); j++)
        // {
        //     for (int c = 0; c < output.GetLength(1); c++)
        //     {
        //         int p = 0;
        //         if (output[j, c] == "itemid" && int.Parse(output[j, c + 7]) == 1000)
        //         {
        //             p = 0;
        //             while (caseid[0, p] != 0)
        //             {
        //                 p++;
        //             }
        //             caseid[0, p] = ulong.Parse(output[j, c + 1]);
        //         }
        //         if (output[j, c] == "itemid" && int.Parse(output[j, c + 7]) == 1001)
        //         {
        //             p = 0;
        //             while (caseid[1, p] != 0)
        //             {
        //                 p++;
        //             }
        //             caseid[1, p] = ulong.Parse(output[j, c + 1]);
        //         }
        //         if (output[j, c] == "itemid" && int.Parse(output[j, c + 7]) == 1002)
        //         {
        //             p = 0;
        //             while (caseid[2, p] != 0)
        //             {
        //                 p++;
        //             }
        //             caseid[2, p] = ulong.Parse(output[j, c + 1]);
        //         }
        //         if (output[j, c] == "itemid" && int.Parse(output[j, c + 7]) == 1003)
        //         {
        //             p = 0;
        //             while (caseid[3, p] != 0)
        //             {
        //                 p++;
        //             }
        //             caseid[3, p] = ulong.Parse(output[j, c + 1]);
        //         }

        //     }
        // }

        // //Open case

        // for (int j = 0; j < caseid.GetLength(0); j++)
        // {
        //     for (int c = 0; c < caseid.GetLength(1); c++)
        //     {
        //         if (caseid[j, c] != 0) 
        //         {
        //             CInventory_ExchangeItem_Request req = new CInventory_ExchangeItem_Request { appid = 1782210, outputitemdefid = (1302+(ulong)j) };
        //             req.materialsitemid.Add(caseid[j, c]);
        //             req.materialsquantity.Add(1);
        //             Console.WriteLine("{0}      {1}     {2}        {3}", req.appid, req.materialsitemid[0], req.materialsquantity[0], req.outputitemdefid );
        //             InvRequest = inventoryService.SendMessage(x => x.ExchangeItem(req));
        //         }
        //     }
        // }
        _inventoryRequest = JobID.Invalid;
        _session.Client.Disconnect();
    }

    private void Log(string log){
        Console.WriteLine(_session.Account.Login +": " + log);
    }
}
