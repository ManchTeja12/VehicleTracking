namespace VehicleMangement.Services
{
    public interface IRouteService
    {
        Task<object> GetRoute(
             double sourceLat,
            double sourceLong,
            double destLat,
            double destLong
        );
    }
}
