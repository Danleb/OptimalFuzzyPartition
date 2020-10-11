using UnityEngine;

namespace Utils
{
    public class ObjectStateKeySwitcher : MonoBehaviour
    {
        [SerializeField] private KeyCode _switchKeyCode;
        [SerializeField] private GameObject _gameObject;

        public void Switch()
        {
            _gameObject.SetActive(!_gameObject.activeSelf);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_switchKeyCode))
                Switch();
        }
    }
}