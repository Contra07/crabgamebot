using SteamKit2.Internal;
using SteamKit2;
using SteamLibrary.JSONs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace SteamLibrary.Services
{
    public class EconService: Service
    {
        private static Dictionary<string, GameAsset> _gameAssets;
        private static Dictionary<string, GameItemDef> _gameItemDefs;
        private Dictionary<string, KeyValuePair<InventoryAsset, InventoryDescription>> _inventoryAssets;


        private SteamUnifiedMessages _steamUnifiedMessages;
        private SteamUnifiedMessages.UnifiedService<IEcon> _econUnifiedService;
        
        private List<JobID> _getInventoryItemsWithDescriptionsRequestIDs;

        public EconService(SteamAccount account, Logger logger) : base(account, logger, "Econ")
        {
            _steamUnifiedMessages = _account.SteamClient.GetHandler<SteamUnifiedMessages>();
            _econUnifiedService = _steamUnifiedMessages.CreateService<IEcon>();
            _getInventoryItemsWithDescriptionsRequestIDs = new List<JobID>();
            _inventoryAssets = new Dictionary<string, KeyValuePair<InventoryAsset, InventoryDescription>>();
            if (_gameAssets is null) {
                _gameAssets = SteamMarket.GetGameAssets(1782210);
            }
            if (_gameItemDefs is null)
            {
                _gameItemDefs = SteamMarket.GetGameItemDefs("1782210", "E98CD32FEC0729EA09130DB1760843330474F4EC");
            }
        }

        protected override void Subscribe()
        {
            _callbackFunctionPointers = new List<IDisposable> {
                _account.CallbackManager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn),
                _account.CallbackManager.Subscribe<SteamUnifiedMessages.ServiceMethodResponse>(OnUnifiedMessageResponse),
                _account.CallbackManager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected),
            };
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback callback)
        {
            if (callback.Result == EResult.OK)
            {
                _getInventoryItemsWithDescriptionsRequestIDs.Add(GetInventoryItemsWithDescriptions());
            }
        }

        private void OnDisconnected(SteamClient.DisconnectedCallback callback)
        {

        }


        private void OnUnifiedMessageResponse(SteamUnifiedMessages.ServiceMethodResponse callback)
        {
            if (_getInventoryItemsWithDescriptionsRequestIDs.Contains(callback.JobID))
            {
                FindNames(callback.GetDeserializedResponse<CEcon_GetInventoryItemsWithDescriptions_Response>());
                _getInventoryItemsWithDescriptionsRequestIDs.Remove(callback.JobID);
            }
        }

        private void FindNames(CEcon_GetInventoryItemsWithDescriptions_Response resp) {
            InventoryAssets assets = JSONUtils.ParseInventoryAssets(JsonSerializer.Serialize(resp));
            _inventoryAssets = new Dictionary<string, KeyValuePair<InventoryAsset, InventoryDescription>>();
            foreach (var desc in assets.descriptions) {
                var asset = assets.assets.Where(x => x.classid == desc.classid).First();
                _inventoryAssets.Add(desc.market_hash_name, new KeyValuePair<InventoryAsset, InventoryDescription>(asset, desc));
            }
        }

        private JobID GetInventoryItemsWithDescriptions()
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
