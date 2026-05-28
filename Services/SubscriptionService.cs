using System.Collections.Concurrent;

namespace VehicleMangement.Services
{
    public class SubscriptionService
    {
        // connectionId -> vehicleId
        private readonly ConcurrentDictionary<string, string> _subscriptions = new();

        // subscribe
        public void Subscribe(string connectionId,string vehicleId)
        {
            _subscriptions[connectionId] = vehicleId;
        }

        // unsubscribe
        public void Unsubscribe(string connectionId, string vehicleId)
        {
            if (_subscriptions.TryGetValue(connectionId, out var currentVehicle))
            {
                if (currentVehicle == vehicleId)
                {
                    _subscriptions.TryRemove(connectionId, out _);
                }
            }
        }

        // validation
        public bool IsSubscribed(string connectionId,string vehicleId)
        {
            return _subscriptions.TryGetValue(connectionId,out var currentVehicle)&&currentVehicle == vehicleId;
        }

        // current vehicle
        public string? GetVehicle(string connectionId)
        {
            _subscriptions.TryGetValue(connectionId,out var vehicleId);
            return vehicleId;
        }

        // remove disconnected client
        public void RemoveConnection(string connectionId)
        {
            _subscriptions.TryRemove(connectionId,out _);
        }
    }
}