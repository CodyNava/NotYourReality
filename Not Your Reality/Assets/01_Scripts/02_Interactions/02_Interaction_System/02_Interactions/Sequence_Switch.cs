using UnityEngine;

public class Sequence_Switch : Interactable_Base
{
    [SerializeField] private Switch_Sequence switchSequence;
    private bool _interacted;

    public bool Interacted
    {
        get => _interacted;
        set => _interacted = value;
    }

    public override void OnInteract()
    {
        if (_interacted) return;
        base.OnInteract();
        ToggleOn();
    }
    
    private void ToggleOn()
    {
        gameObject.transform.Rotate(-60,0,0);
        switchSequence.AddSwitch(this);
        _interacted = true;
    }
    public void ToggleOff()
    {
        gameObject.transform.Rotate(60,0,0);
        _interacted = false;
    }
}