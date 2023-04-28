using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Tube_Traveller.Model;

namespace Tube_Traveller
{
    internal class TflClient
    {
        private readonly HttpClient _client = new HttpClient();
        //private const string appkey = "?app_key=%20497eeeec89d74dd7b963b24a0f6a3b9a"; //Key to allow up to 500 calls
        // API from https://api-portal.tfl.gov.uk/api-details#api=ReleasedUnifiedAPIProd&operation=AccidentStats_Get
        
        public async Task<List<Root>> GetAllLinesByModeAsync(string mode)
        {
            var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/Mode/{mode}/Route"); //Getting json from api
            var root = JsonConvert.DeserializeObject<List<Root>>(responseBody); //Turning json into readable classes
            return root!;
        }

        public async Task<Root> GetLineRouteByLineAsync(string lineId, string direction)
        {
            var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/{lineId}/Route/Sequence/{direction}");
            var root = JsonConvert.DeserializeObject<Root>(responseBody);
            return root!;
        }

        public async Task<List<Root>> GetAllStationsByLine(string lineId)
        {
            var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/{lineId}/StopPoints");
            var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
            return root!;
        }

        public async Task<Root> GetJourney(string fromStationId, string toStationId, string time, string depArr)
        {
            var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Journey/JourneyResults/{fromStationId}/to/{toStationId}?time={time}&timeIs={depArr}&mode=tube");
            var root = JsonConvert.DeserializeObject<Root>(responseBody);
            return root!;
        }
    }
}
