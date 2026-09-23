using UnityEngine;

namespace VibeCooking
{
    public class ServeBell : MonoBehaviour
    {
        [Header("Target Active Bowl")]
        [SerializeField] private BowlInstance activeBowl;

        [Header("Target Active Customer (Optional)")]
        [SerializeField] private CustomerAgent activeCustomer;

        [Header("Audio (Optional)")]
        [SerializeField] private AudioClip bellDingClip;
        [SerializeField] private AudioSource audioSource;

        private Vector3 initialScale;

        private void Start()
        {
            initialScale = transform.localScale;
            if (activeBowl == null)
                activeBowl = Object.FindAnyObjectByType<BowlInstance>();
            if (activeCustomer == null)
                activeCustomer = Object.FindAnyObjectByType<CustomerAgent>();
        }

        private void OnMouseDown()
        {
            RingBell();
        }

        public void RingBell()
        {
            transform.localScale = initialScale * 0.88f;
            Invoke(nameof(ResetScale), 0.12f);

            if (audioSource != null && bellDingClip != null)
            {
                audioSource.PlayOneShot(bellDingClip);
            }

            if (activeBowl == null)
                activeBowl = Object.FindAnyObjectByType<BowlInstance>();
            if (activeCustomer != null && activeBowl != null)
            {
                Debug.Log("<color=yellow>[ServeBell] Ding! Serving active bowl to customer...</color>");
                activeCustomer.ServeOrder(activeBowl);
            }
            else
            {
                Debug.Log("<color=yellow>[ServeBell] Ding! Serving active bowl...</color>");
                GameEvents.TriggerOrderServed(activeBowl, activeCustomer);
                if (activeBowl != null)
                {
                    activeBowl.ClearBowl();
                }
            }
        }

        private void ResetScale()
        {
            transform.localScale = initialScale;
        }

        public void SetActiveBowl(BowlInstance bowl) => activeBowl = bowl;
        public void SetActiveCustomer(CustomerAgent customer) => activeCustomer = customer;
    }
}
