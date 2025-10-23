using UnityEngine;

public static class Keybinds
{
    public static MainInput MainInput;

    #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
    #endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void Initialize()
    {
        MainInput = new MainInput();
    }
}
