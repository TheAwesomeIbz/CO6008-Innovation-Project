using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_PlayerInputManager : MonoBehaviour
{
    
    public static bool PlayerControlsEnabled;

    public ButtonProperty Jump { get; private set; }
    public ButtonProperty Attack { get; private set; }
    public AxisProperty Horizontal { get; private set; }
    public AxisProperty Vertical { get; private set; }

    void Start()
    {
        PlayerControlsEnabled = true;

        Jump = new ButtonProperty("Jump");
        Horizontal = new AxisProperty("Horizontal");
        Vertical = new AxisProperty("Vertical");
        Attack = new ButtonProperty("Submit");
    }

    public class AxisProperty : ButtonProperty
    {
        const float DeadZone = 0.5f;
        public float AxisValue => PlayerControlsEnabled ? Input.GetAxisRaw(_buttonName) : 0;
        public override bool PressedThisFrame() => Input.GetButtonDown(_buttonName) && PlayerControlsEnabled && AxisValue != 0;
        public override bool ReleasedThisFrame() => Input.GetButtonUp(_buttonName) && PlayerControlsEnabled && AxisValue != 0;
        public override bool IsPressed() => Input.GetButton(_buttonName);

        public bool PositiveAxisPressed() => Input.GetButton(_buttonName) && PlayerControlsEnabled && AxisValue > DeadZone;
        public bool NegativeAxisPressed() => Input.GetButton(_buttonName) && PlayerControlsEnabled && AxisValue < -DeadZone;
        
        public AxisProperty(string buttonName) : base(buttonName) { 
        }
    }
    public class ButtonProperty
    {
        protected string _buttonName;
        public virtual bool PressedThisFrame() => Input.GetButtonDown(_buttonName) && PlayerControlsEnabled;
        public virtual bool ReleasedThisFrame() => Input.GetButtonUp(_buttonName) && PlayerControlsEnabled;
        public virtual bool IsPressed() => Input.GetButton(_buttonName) && PlayerControlsEnabled;
        public ButtonProperty(string buttonName)
        {
            _buttonName = buttonName;
        }
    }
}
