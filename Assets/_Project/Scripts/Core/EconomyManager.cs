using UnityEngine;

namespace VibeCooking
{
    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }

        [Header("Starting Economy")]
        [SerializeField] private int startingYen = 1000;
        [SerializeField] private int currentYen;

        public int CurrentYen => currentYen;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            currentYen = startingYen;
        }

        private void Start()
        {
            // Initial event trigger to refresh HUD
            GameEvents.TriggerMoneyChanged(currentYen, 0);
        }

        public void AddMoney(int amount, string reason = "")
        {
            if (amount <= 0) return;
            currentYen += amount;
            GameEvents.TriggerMoneyChanged(currentYen, amount);
            Debug.Log($"<color=yellow>[EconomyManager] +¥{amount} ({reason}) | New Balance: ¥{currentYen}</color>");
        }

        public bool SpendMoney(int amount)
        {
            if (amount <= 0) return false;
            if (currentYen < amount)
            {
                Debug.LogWarning($"[EconomyManager] Insufficient funds! Needed ¥{amount}, have ¥{currentYen}");
                return false;
            }

            currentYen -= amount;
            GameEvents.TriggerMoneyChanged(currentYen, -amount);
            Debug.Log($"[EconomyManager] -¥{amount} | New Balance: ¥{currentYen}");
            return true;
        }

        public void ResetBalance()
        {
            currentYen = startingYen;
            GameEvents.TriggerMoneyChanged(currentYen, 0);
        }
    }
}
