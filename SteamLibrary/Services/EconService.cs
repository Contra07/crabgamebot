using SteamKit2.Internal;
using SteamKit2;
using SteamLibrary.JSONs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SteamLibrary.Services
{
    public class EconService: Service
    {
        private SteamUnifiedMessages _steamUnifiedMessages;
        private SteamUnifiedMessages.UnifiedService<IEcon> _econUnifiedService;
        
        private List<JobID> _getInventoryItemsWithDescriptionsRequestIDs;

        public EconService(SteamAccount account, Logger logger) : base(account, logger, "Econ")
        {
            _steamUnifiedMessages = _account.SteamClient.GetHandler<SteamUnifiedMessages>();
            _econUnifiedService = _steamUnifiedMessages.CreateService<IEcon>();
            _getInventoryItemsWithDescriptionsRequestIDs = new List<JobID>();
        }

        protected override void Subscribe()
        {
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
                _getInventoryItemsWithDescriptionsRequestIDs.Add(GetInventoryItemsWithDescriptionsRequestIDs());
            }
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {

        }


        private void OnUnifiedMessageResponse(SteamUnifiedMessages.ServiceMethodResponse callback)
        {
            if (_getInventoryItemsWithDescriptionsRequestIDs.Contains(callback.JobID))
            {
                FindNamesAsync(callback.GetDeserializedResponse<CEcon_GetInventoryItemsWithDescriptions_Response>());
                _getInventoryItemsWithDescriptionsRequestIDs.Remove(callback.JobID);
            }
        }

        private void FindNamesAsync(CEcon_GetInventoryItemsWithDescriptions_Response resp) {
            foreach (var desc in resp.descriptions) {
                string price = SteamMarket.Test(desc);
                _logger.Log("Maybe item name: " + desc.market_hash_name + " maybe price: " + price);
            }
        }

        private JobID GetInventoryItemsWithDescriptionsRequestIDs()
        {
            CEcon_GetInventoryItemsWithDescriptions_Request req = new CEcon_GetInventoryItemsWithDescriptions_Request()
            {
                steamid = _account.SteamClient.SteamID,
                appid = 1782210,
                for_trade_offer_verification = false,
                get_descriptions = true,
                contextid = 2,
                language = "english"
            };
            return _econUnifiedService.SendMessage(x => x.GetInventoryItemsWithDescriptions(req));
        }
    }
}
