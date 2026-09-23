using System;
using UnityEngine;

namespace VibeCooking
{
    /// <summary>
    /// Centralized event bus for decoupling game systems in Vibe Cooking.
    /// </summary>
    public static class GameEvents
    {
        // Order events
        public static event Action<OrderTicket> OnOrderCreated;
        public static void TriggerOrderCreated(OrderTicket order) => OnOrderCreated?.Invoke(order);

        // Bowl events
        public static event Action<BowlInstance> OnBowlUpdated;
        public static void TriggerBowlUpdated(BowlInstance bowl) => OnBowlUpdated?.Invoke(bowl);

        // Serving & Customer events
        public static event Action<BowlInstance, CustomerAgent> OnOrderServed;
        public static void TriggerOrderServed(BowlInstance bowl, CustomerAgent customer) => OnOrderServed?.Invoke(bowl, customer);

        // Economy events
        public static event Action<int, int> OnMoneyChanged; // totalYen, delta
        public static void TriggerMoneyChanged(int totalYen, int delta) => OnMoneyChanged?.Invoke(totalYen, delta);

        // Music & Radio events
        public static event Action<MusicTrackSO> OnTrackChanged;
        public static void TriggerTrackChanged(MusicTrackSO newTrack) => OnTrackChanged?.Invoke(newTrack);
    }
}
