using SteamKit2.Internal;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SteamLibrary.Core
{
    public static class InventoryExtension
    {
        public static AsyncJob<SteamUnifiedMessages.ServiceMethodResponse> GetItemDefMeta(this SteamUnifiedMessages.UnifiedService<IInventory> inventoryService, uint appid)
        {
            CInventory_GetItemDefMeta_Request req = new CInventory_GetItemDefMeta_Request
            {
                appid = appid,
            };
            return inventoryService.SendMessage(x => x.GetItemDefMeta(req));
        }

        public static AsyncJob<SteamUnifiedMessages.ServiceMethodResponse> GetInventory(this SteamUnifiedMessages.UnifiedService<IInventory> inventoryService, uint appid)
        {
            CInventory_GetInventory_Request req = new CInventory_GetInventory_Request
            {
                appid = appid
            };
            return inventoryService.SendMessage(x => x.GetInventory(req));
        }

        public static AsyncJob<SteamUnifiedMessages.ServiceMethodResponse> ConsumePlaytime(this SteamUnifiedMessages.UnifiedService<IInventory> inventoryService, uint appid, ulong itemdefid)
        {
            CInventory_ConsumePlaytime_Request req = new CInventory_ConsumePlaytime_Request
            {
                appid = appid,
                itemdefid = itemdefid
            };
            return inventoryService.SendMessage(x => x.ConsumePlaytime(req));
        }

        public static AsyncJob<SteamUnifiedMessages.ServiceMethodResponse> ExchangeItem(this SteamUnifiedMessages.UnifiedService<IInventory> inventoryService, uint appid, ulong outputitemdefid, Dictionary<ulong, uint> materialsitemid)
        {
            CInventory_ExchangeItem_Request req = new CInventory_ExchangeItem_Request
            {
                appid = appid,
                outputitemdefid = outputitemdefid,
            };

            foreach (var material in materialsitemid) {
                req.materialsitemid.Add(material.Key);
                req.materialsquantity.Add(material.Value);
            }

            return inventoryService.SendMessage(x => x.ExchangeItem(req));
        }
    }
}
