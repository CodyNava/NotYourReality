namespace System.GlobalEventSystem
{
    public static class GlobalEventManager
    {
        public static event Action MouseSettingsChanged;
        public static event Action OnKey;
        public static event Action OnCake;
        public static event Action OnPhone;
        public static event Action OnBeanBag;
        public static event Action OnBed;
        public static event Action OnRecord;
        public static event Action OnDoor;

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
        public static void OnBeanBagTouched()
        {
            OnBeanBag?.Invoke();
        }
        public static void OnBedTouched()
        {
            OnBed?.Invoke();
        }
        public static void OnRecordTouched()
        {
            OnRecord?.Invoke();
        }
        public static void OnDoorTouched()
        {
            OnDoor?.Invoke();
        }
        
    
    }
}
