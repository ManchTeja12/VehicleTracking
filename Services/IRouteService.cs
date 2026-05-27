namespace VehicleMangement.Services
{
    public interface IRouteService
    {
        Task<List<double[]>> GetRoute(
             double sourceLat,
            double sourceLng,
            double destLat,
            double destLng
        );
    }
}
    