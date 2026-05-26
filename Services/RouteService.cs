namespace VehicleMangement.Services
{
    public class RouteService:IRouteService
    {
        public async Task<object> GetRoute(double sourceLat, double sourceLong, double destLat, double destLong)
        {
            return new
            {
                message = "route data"
            };
        }
    }
}
