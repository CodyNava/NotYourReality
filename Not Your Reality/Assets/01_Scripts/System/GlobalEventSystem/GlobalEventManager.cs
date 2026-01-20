namespace System.GlobalEventSystem
{
    public static class GlobalEventManager
    {
        public static event Action MouseSettingsChanged;
        public static event Action OnKey;
        public static event Action OnCake;
        public static event Action OnPhone;

        public static void OnMouseSettingsChange()
        {
            MouseSettingsChanged?.Invoke();
        }
        public static void OnKeyUsed()
        {
            OnKey?.Invoke();
        }

        public static void OnCakeTouched()
        {
            OnCake?.Invoke();
        }
        public static void OnPhoneTouched()
        {
            OnPhone?.Invoke();
        }
    
    }
}
