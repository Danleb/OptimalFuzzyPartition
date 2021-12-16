using UnityEngine;

namespace Utils
{
    public class ObjectStateKeySwitcher : MonoBehaviour
    {
        [SerializeField] private KeyCode _switchKeyCode;
        [SerializeField] private GameObject[] _gameObjects;

        private bool _isEnabled = false;

        public void Switch()
        {
            _isEnabled = !_isEnabled;
            foreach (var gameObject in _gameObjects)
            {
                gameObject.SetActive(_isEnabled);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(_switchKeyCode))
                Switch();
        }
    }
}
