using UnityEngine;

namespace Utils
{
    public class DisableOnAwake : MonoBehaviour
    {
        [SerializeField] private bool work;

        private void Awake()
        {
            if (work)
                gameObject.SetActive(false);
        }
    }
}
