using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            try
            {
                var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/Meta/Modes");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root!;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new List<Root>(); }
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
        public async Task<Root> GetLineRouteAsync(string id)
        {
            try
            {
                var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/{id}/Route/Sequence/inbound");
                var root = JsonConvert.DeserializeObject<Root>(responseBody);
                return root!;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new Root(); }
        }

        public async Task<List<Root>> GetModeNameAsync()
        {
            try
            {
                var responseBody = await _client.GetStringAsync("https://api.tfl.gov.uk/line/mode/tube");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root!;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return new List<Root>(); }
        }

        public async Task<List<Root>> GetArrivalsAsync(string line)
        {
            try
            {
                var responseBody = await _client.GetStringAsync($"https://api.tfl.gov.uk/Line/{line}/Arrivals");
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
