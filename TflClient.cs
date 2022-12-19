using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tube_Traveller.Model;

namespace Tube_Traveller
{
    internal class TflClient
    {
        private readonly HttpClient _client = new HttpClient();
        //private const string appkey = "?app_key=%20497eeeec89d74dd7b963b24a0f6a3b9a"; //Key to allow up to 500 calls
        // API Taken from https://api-portal.tfl.gov.uk/api-details#api=Line&operation=Forward_Proxy

        public async Task<Stream> GetDetailedStationDataStreamAsync()
        {
            Stream stream = await _client.GetStreamAsync(@"https://api.tfl.gov.uk/stationdata/tfl-stationdata-detailed.zip"); //Gets zipfile from api
            return stream;
        }

        public async Task<List<Root>> GetAllModesAsync()
        {
            var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/Meta/Modes"); //Getting json from api
            var root = JsonConvert.DeserializeObject<List<Root>>(responseBody); //Turning json into readable classes
            return root!;
        }
        public async Task<List<Root>> GetAllLinesByModeAsync(string mode)
        {
            try
            {
                var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/Mode/{mode}/Route");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root!;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new List<Root>(); }
        }

        public async Task<Root> GetLineRouteByLineAsync(string lineId, string direction)
        {
            try
            {
                var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/{lineId}/Route/Sequence/{direction}");
                var root = JsonConvert.DeserializeObject<Root>(responseBody);
                return root!;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new Root(); }
        }
        public async Task<List<Root>> GetAllLineBusRouteAsync()
        {
            try
            {
                var responseBody = await _client.GetStringAsync("https://api.tfl.gov.uk/Line/Route");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root!;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new List<Root>(); }
        }


        public async Task<List<Root>> GetArrivalsByLineAsync(string lineId)
        {
            try
            {
                var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/{lineId}/Arrivals");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); throw;
            }
        }

        public async Task<List<Root>> GetAllStationsByLine(string lineId)
        {
            try
            {
                var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/{lineId}/StopPoints");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root!;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new List<Root>(); }
        }

        public async Task<List<Root>> GetCanonicalDirectionAsync(string originatingStationId, string destinationStationId, [Optional] string lineId) //Whether to go inbound or outbound - not working
        {
            try
            {
                var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/StopPoint/{originatingStationId}/DirectionTo/{destinationStationId}/{lineId}");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); throw;
            }
        }
    }
}
