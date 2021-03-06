﻿using System.Collections.Generic;
using UnityEngine;

public enum KeyInputMode
{
    KeyPressed,
    KeyUp,
    KeyDown
}

public class KeyAction
{
    public delegate void Callback();
    public KeyInputMode keyMode;
    public KeyCode code;
    public Callback callback;

    public KeyAction(KeyInputMode keyMode, KeyCode code, Callback callback)
    {
        this.keyMode = keyMode;
        this.code = code;
        this.callback = callback;
    }
}

public class InputManager : Manager<InputManager>
{
    public const string HORIZONTAL_AXIS = "Horizontal";
    public const string VERTICAL_AXIS = "Vertical";
    public const string MOUSE_X = "Mouse X";
    public const string MOUSE_Y = "Mouse Y";

    public const KeyCode PRIMARY_WEAPON_KEY = KeyCode.Mouse0;
    public const KeyCode SECONDARY_WEAPON_KEY = KeyCode.Mouse1;

    public const KeyCode TURRET_UP_KEY = KeyCode.E;
    public const KeyCode TURRET_DOWN_KEY = KeyCode.R;
    public const KeyCode TURRET_LEFT_KEY = KeyCode.F;
    public const KeyCode TURRET_RIGHT_KEY = KeyCode.G;

    public const KeyCode PAUSE_KEY = KeyCode.Escape;
    public const KeyCode SPAWN_PLAYER_KEY = KeyCode.Z;
    public const KeyCode WEAPON_CHANGE_KEY = KeyCode.Q;

    private List<KeyAction> actions;

    private void Awake()
    {
        actions = new List<KeyAction>();
    }

    public float GetWheel()
    {
        return Input.mouseScrollDelta.y;
    }

    public float GetAxis(string axisName)
    {
        return Input.GetAxis(axisName);
    }

    public void AddKeyAction(KeyAction item)
    {
        actions.Add(item);
    }

    public void RemoveKeyActions(MonoBehaviour obj)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].callback.Target.GetType() == obj.GetType())
                actions.RemoveAt(i--);
        }
    }

    private void Update()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            switch (actions[i].keyMode)
            {
                case KeyInputMode.KeyPressed:
                    {
                        if (Input.GetKey(actions[i].code))
                        {
                            actions[i].callback.Invoke();
                        }
                        break;
                    }
                case KeyInputMode.KeyUp:
                    {
                        if (Input.GetKeyUp(actions[i].code))
                        {
                            actions[i].callback.Invoke();
                        }
                        break;
                    }
                case KeyInputMode.KeyDown:
                    {
                        if (Input.GetKeyDown(actions[i].code))
                        {
                            actions[i].callback.Invoke();
                        }
                        break;
                    }
            }
        }
    }
}