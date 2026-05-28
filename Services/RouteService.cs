using System.Text.Json;

namespace VehicleMangement.Services
{
    // get route from osrm api
    public class RouteService:IRouteService
    {
        private readonly HttpClient _httpClient;
        public RouteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<double[]>> GetRoute(double sourceLat, double sourceLng, double destLat, double destLng)
        {
            var url =$"http://router.project-osrm.org/route/v1/driving/" +
                      $"{sourceLng},{sourceLat};{destLng},{destLat}" +
                      $"?overview=full&geometries=geojson";
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"OSRM Request Failed: {response.StatusCode}");
                    return new List<double[]>();
                }

                var json = await response.Content.ReadAsStringAsync();
        
                var osrmData = JsonSerializer.Deserialize<JsonElement>(json);

                var rawCoords = osrmData.GetProperty("routes")[0].GetProperty("geometry").GetProperty("coordinates");

                var coordinates = new List<double[]>();
                foreach (var point in rawCoords.EnumerateArray())
                {
                    var arr = point.EnumerateArray().Select(x => x.GetDouble()).ToArray();
                    coordinates.Add(arr);
                }

                return coordinates;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"OSRM Error:{ex}");
                return new List<double[]>();
            }
           
        }
    }
}
