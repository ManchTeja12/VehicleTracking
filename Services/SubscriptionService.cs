using System.Collections.Concurrent; //thread-safe collection class (thread a path of execution) multi threading

namespace VehicleMangement.Services
{
    public class SubscriptionService //it is like memory storgae for active signalr subscriptions
    {
        // connectionId -> vehicleId
        private readonly ConcurrentDictionary<string, string> _subscriptions = new();
        //One ConnectionId -> One VehicleId

        //one connectionid-> multiplevehiclesid -> ConcurrentDictionary<string, HashSet<string>>

        // subscribe
        public void Subscribe(string connectionId,string vehicleId)
        {
            _subscriptions[connectionId] = vehicleId;
        }

        // unsubscribe
        public void Unsubscribe(string connectionId, string vehicleId)
        {
            if (_subscriptions.TryGetValue(connectionId, out var currentVehicle)) // store vehicleid in currentvehicle
                // out it is a keyword it returns actual value
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