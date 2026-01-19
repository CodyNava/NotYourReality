namespace System.GlobalEventSystem
{
    public static class GlobalEventManager
    {
        public static event Action MouseSettingsChanged;
        public static event Action OnKey;

        public static void OnMouseSettingsChange()
        {
            MouseSettingsChanged?.Invoke();
        }
        public static void OnKeyUsed()
        {
            OnKey?.Invoke();
        }
    
    }
}
