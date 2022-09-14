using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tube_Traveller.Model;

namespace Tube_Traveller
{
    internal class TflData
    {
        private readonly HttpClient client = new HttpClient(); 
        private const string appkey = "?app_key=%20497eeeec89d74dd7b963b24a0f6a3b9a"; //Key to allow up to 500 calls
        // API Taken from https://api-portal.tfl.gov.uk/api-details#api=Line&operation=Forward_Proxy

        public async Task<string> GetLineType(int lineId)
        {
            //Credit Start
            try
            {
                var responseBody = await client.GetStringAsync("https://api.tfl.gov.uk/Line/{id}/Arrivals"); // Getting Data
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody); // Allowing the use of data towards classes
                return root[lineId].Type; // The data
            }
            catch (Exception ex) { return ex.Message; } //Reporting any errors to do with the api response message
            //Credit End
        }

        public async Task<string> GetLineId(int lineId)
        {
            
            try
            {
                var responseBody = await client.GetStringAsync("https://api.tfl.gov.uk/line/mode/tube"); 
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root[lineId].Id;
            }
            catch (Exception ex) { return ex.Message; } 
            
        }

        public async Task<string> GetModeName(int lineId)
        {
            try
            {
                var responseBody = await client.GetStringAsync("https://api.tfl.gov.uk/line/mode/tube"); 
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root[lineId].ModeName;
            }
            catch (Exception ex) { return ex.Message; }
        }

        public async Task<string> GetName(int lineId)
        {
            try
            {
                var responseBody = await client.GetStringAsync($"https://api.tfl.gov.uk/line/mode/tube");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root[lineId].Name;
            }
            catch (Exception ex) { return ex.Message; }
        }

        public async Task<DateTime> GetLineArrivals(int lineId)
        {
            try
            {
                var responseBody = await client.GetStringAsync($"https://api.tfl.gov.uk/Line/{lineId}/Arrivals");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root[lineId].ExpectedArrival;
            }
            catch { return DateTime.Now; }
        }

        public async Task<string> GetDestinationName(int lineId)
        {
            try
            {
                var responseBody = await client.GetStringAsync($"https://api.tfl.gov.uk/Line/{lineId}/Arrivals");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root[lineId].DestinationName;
            }
            catch (Exception ex) { return ex.Message; }
        }

        public async Task<string> GetCurrentLocation(int lineId)
        {
            try
            {
                var responseBody = await client.GetStringAsync($"https://api.tfl.gov.uk/Line/{lineId}/Arrivals");
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root[lineId].CurrentLocation;
            }
            catch (Exception ex) { return ex.Message; }
        }
    }
}
