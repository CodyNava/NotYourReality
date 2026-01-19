namespace System.GlobalEventSystem
{
    public static class GlobalEventManager
    {
        public static event Action MouseSettingsChanged;

        public static void OnMouseSettingsChange()
        {
            MouseSettingsChanged?.Invoke();
        }
    
    }
}
