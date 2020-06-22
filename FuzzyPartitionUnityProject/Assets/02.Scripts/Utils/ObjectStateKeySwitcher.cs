using UnityEngine;

namespace Utils
{
    public class ObjectStateKeySwitcher : MonoBehaviour
    {
        [SerializeField] private KeyCode _switchKeyCode;
        [SerializeField] private GameObject _gameObject;

        private void Update()
        {
            if (Input.GetKeyDown(_switchKeyCode))
                _gameObject.SetActive(!_gameObject.activeSelf);
        }
    }
}