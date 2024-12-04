using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_PlayerInputManager : MonoBehaviour
{
    /// <summary>
    /// Static boolean keeping track of whether the player controls should be enabled or not
    /// </summary>
    public static bool PlayerControlsEnabled;

    public MouseProperty LeftClick { get; private set; }
    public MouseProperty RightClick { get; private set; }
    public Vector3 CursorWorldPoint => new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
    public ButtonProperty Dodge { get; private set; }
    public ButtonProperty Submit { get; private set; }
    public AxisProperty Axis2D { get; private set; }

    void Start()
    {
        PlayerControlsEnabled = true;

        Dodge = new ButtonProperty("Dodge");
        Submit = new ButtonProperty("Submit");

        Axis2D = new AxisProperty();
        LeftClick = new MouseProperty(0);
        RightClick = new MouseProperty(1);

    }

    public class AxisProperty
    {
        const string Horizontal = "Horizontal";
        const string Vertical = "Vertical";
        const float DeadZone = 0.25f;
        public Vector2 AxisValue => PlayerControlsEnabled ?
            new Vector2(Input.GetAxisRaw(Horizontal), Input.GetAxisRaw(Vertical)).normalized : Vector2.zero;
        public bool PressedThisFrame() => (Input.GetButtonDown(Horizontal) || Input.GetButtonDown(Vertical)) && PlayerControlsEnabled && AxisValue.magnitude > DeadZone && PlayerControlsEnabled;
        public bool ReleasedThisFrame() => (Input.GetButtonUp(Horizontal) || Input.GetButtonUp(Vertical)) && PlayerControlsEnabled && AxisValue.magnitude > DeadZone && PlayerControlsEnabled;
        public bool IsPressed() => (Input.GetButton("Horizontal") || Input.GetButton("Vertical")) && PlayerControlsEnabled;
    }

    public class MouseProperty
    {
        protected int _buttonNumber;
        public virtual bool PressedThisFrame() => Input.GetMouseButtonDown(_buttonNumber) && PlayerControlsEnabled;
        public virtual bool ReleasedThisFrame() => Input.GetMouseButtonUp(_buttonNumber) && PlayerControlsEnabled;
        public virtual bool IsPressed() => Input.GetMouseButton(_buttonNumber) && PlayerControlsEnabled;
        public MouseProperty(int buttonNumber)
        {
            _buttonNumber = buttonNumber;
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
