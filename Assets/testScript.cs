using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class testScript : MonoBehaviour
{

    Keyboard keyboard;
    // Start is called before the first frame update
    void Start()
    {
        keyboard = Keyboard.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard == null) return;
        if(keyboard.numpad1Key.IsPressed())
        {
            Debug.Log(PlayerManager.playerPosition());
        }
    }
}
