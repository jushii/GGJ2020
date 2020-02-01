using System;
using DefaultNamespace;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Player _player;
    public string _horizontalAxis;
    public string _verticalAxis;
    public string _aButton;
    public string _bButton;
    public string _xButton;
    public string _yButton;
    public string _triggerAxis;
    public int _controllerNumber;
    
    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    
    public enum Button
    {
        A,
        B,
        X,
        Y
    }

    private void Awake()
    {
        _player = GetComponent<Player>();
        SetControllerNumber(_player.playerNumber);
    }

    public bool IsButtonDown(Button button)
    {
        switch (button)
        {
            case Button.A: return Input.GetButtonDown(_aButton);
            case Button.B: return Input.GetButtonDown(_bButton);
            case Button.X: return Input.GetButtonDown(_xButton);
            case Button.Y: return Input.GetButtonDown(_yButton);
        }

        return false;
    }

    private void SetControllerNumber(int number)
    {
        if (number == 0) return;
        
        _controllerNumber = number;
        _horizontalAxis = "Joystick_" + _controllerNumber + "_Horizontal";
        _verticalAxis = "Joystick_" + _controllerNumber + "_Vertical";
        _aButton = "Joystick_" + _controllerNumber + "_A";
        _bButton = "Joystick_" + _controllerNumber + "_B";
        _xButton = "Joystick_" + _controllerNumber + "_X";
        _yButton = "Joystick_" + _controllerNumber + "_Y";
    }

    private void FixedUpdate()
    {
        if (_controllerNumber > 0)
        {
            float horizontal = Input.GetAxis(_horizontalAxis);
            if (horizontal < 0.3f && horizontal > -0.3f) horizontal = 0;
            float vertical = Input.GetAxis(_verticalAxis);
            if (vertical < 0.3f && vertical > -0.3f) vertical = 0;
            Horizontal = horizontal;
            Vertical = vertical;
            // Debug.Log($"Horizontal: {Horizontal} Vertical: {Vertical}");
        }
    }
}
