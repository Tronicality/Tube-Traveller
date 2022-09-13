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


        public async Task<string> GetLineId(int id)
        {
            //Credit Start
            try
            {
                var responseBody = await client.GetStringAsync("https://api.tfl.gov.uk/line/mode/tube"); // Using the API to get data
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root[id].id;
            }
            catch (Exception ex) //Reporting any errors to do with the api response message
            {
                return ex.Message;
            }
            //Credit End
        }

        public async Task<string> GetModeName(int id)
        {
            try
            {
                var responseBody = await client.GetStringAsync("https://api.tfl.gov.uk/line/mode/tube"); 
                var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
                return root[id].modeName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<DateTime> GetLineArrivals(int id)
        {
            var responseBody = await client.GetStringAsync($"https://api.tfl.gov.uk/Line/{id}/Arrivals");
            var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
            return root[id].expectedArrival;
        }

        public async Task<string> GetDestinationName(int id)
        {
            var responseBody = await client.GetStringAsync($"https://api.tfl.gov.uk/Line/{id}/Arrivals");
            var root = JsonConvert.DeserializeObject<List<Root>>(responseBody);
            return root[id].destinationName;
        }
    }
}
