using System;

namespace Utils
{
#if UNITY_EDITOR
    using UnityEditor;

    [InitializeOnLoad]
#endif
    public static class PlayStateNotifier
    {
#pragma warning disable CS0067
        public static event Action OnPlaymodeExit;
#pragma warning restore CS0067

#if UNITY_EDITOR
        static PlayStateNotifier()
        {
            EditorApplication.playModeStateChanged += ModeChanged;
        }

        private static void ModeChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.ExitingPlayMode)
                OnPlaymodeExit?.Invoke();
        }
#endif
    }
}
