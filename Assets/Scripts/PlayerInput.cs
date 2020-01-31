using System;
using DefaultNamespace;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Player _player;
    private string _horizontalAxis;
    private string _verticalAxis;
    private string _aButton;
    private string _bButton;
    private string _xButton;
    private string _yButton;
    private string _triggerAxis;
    private int _controllerNumber;
    
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
            case Button.A: return Input.GetButton(_aButton);
            case Button.B: return Input.GetButton(_bButton);
            case Button.X: return Input.GetButton(_xButton);
            case Button.Y: return Input.GetButton(_yButton);
        }

        return false;
    }

    private void SetControllerNumber(int number)
    {
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
            Debug.Log($"Horizontal: {Horizontal} Vertical: {Vertical}");
        }
    }
}
