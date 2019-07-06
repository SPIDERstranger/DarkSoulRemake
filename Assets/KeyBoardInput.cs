using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardInput : IUserInput
{
    [Space(10)]
    [Header("===== key setting =====")]
    public string KeyUp = "w";
    public string KeyDown = "s";
    public string KeyLeft = "a";
    public string KeyRight = "d";

    public string KeyA;
    public string KeyB;
    public string KeyC;
    public string KeyD;

    public string KeyJUp = "up";
    public string KeyJDown = "down";
    public string KeyJLeft = "left";
    public string KeyJRight = "right";

    [Header("===== mouse setting =====")]
    public bool mouseEnable;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;




    MyButton buttonA = new MyButton();
    MyButton buttonB = new MyButton();
    MyButton buttonC = new MyButton();
    MyButton buttonD = new MyButton();

    void Update()
    {

        //视角移动信号
        if (mouseEnable)
        {
            Jup = Input.GetAxis("Mouse Y") * mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * mouseSensitivityX;

        }
        else
        {
            Jup = (Input.GetKey(KeyJUp) ? 1.0f : 0) - (Input.GetKey(KeyJDown) ? 1.0f : 0);
            Jright = (Input.GetKey(KeyJRight) ? 1.0f : 0) - (Input.GetKey(KeyJLeft) ? 1.0f : 0);
        }
        //人物移动信号
        targetDup = (Input.GetKey(KeyUp) ? 1.0f : 0) - (Input.GetKey(KeyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(KeyRight) ? 1.0f : 0) - (Input.GetKey(KeyLeft) ? 1.0f : 0);

        if (!inputEnabled)
        {
            targetDup = 0;
            targetDright = 0;
        }
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, .2f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, .2f);
        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;
        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = transform.right * Dright2 + transform.forward * Dup2;
        //获取跑步信号
        buttonA.Tick(Input.GetKey(KeyA));

        run = buttonA.IsPressing;


        //跳跃信号
        buttonB.Tick(Input.GetKey(KeyB));
        jump = buttonB.OnPressed&&buttonB.IsExtending;

        //攻击信号
        buttonC.Tick(Input.GetKey(KeyC));
        attack = buttonC.OnPressed;

        //获取防御信号
        buttonD.Tick(Input.GetKey(KeyD));

        defense = buttonD.IsPressing;


    }


}
