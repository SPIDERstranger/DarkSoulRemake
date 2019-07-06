using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoysticksInput : IUserInput
{
    public string axisX = "axisX";
    public string axisY = "axisY";
    public string axisJX = "axis6";
    public string axisJY = "axis3";

    public string BtnA = "btn1";
    public string BtnB = "btn2";
    public string BtnX = "btn0";
    public string BtnY = "btn3";

    public string BtnRB = "btn5";
    public string BtnLB = "btn4";

    public string BtnJoystick = "btn11";

    MyButton buttonA = new MyButton();
    MyButton buttonB = new MyButton();
    MyButton buttonX = new MyButton();
    MyButton buttonY = new MyButton();

    MyButton buttonRB = new MyButton();
    MyButton buttonLB = new MyButton();

    MyButton buttonJoystick = new MyButton();
    MyButton buttonLockLeft = new MyButton();
    MyButton buttonLockRight = new MyButton();
    // Update is called once per frame
    void Update()
    {

        //视角移动信号
        Jup = Input.GetAxis(axisJY);
        Jright = -Input.GetAxis(axisJX) ;
        //人物移动信号
        targetDup = Input.GetAxis(axisY);
        targetDright = Input.GetAxis(axisX);

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
        buttonA.Tick(Input.GetButton(BtnA));
        run = (buttonA.IsPressing && !buttonA.IsDelaying)||buttonA.IsExtending;


        //跳跃信号
        jump = buttonA.IsExtending && buttonA.OnPressed;
        roll = buttonA.OnReleased && buttonA.IsDelaying ;

        //攻击信号
        buttonX.Tick(Input.GetButton(BtnX));
        attack = buttonX.OnPressed;
        
        //获取防御信号
        defense = Input.GetButton(BtnLB);

        //视角锁定信号
        buttonJoystick.Tick(Input.GetButton(BtnJoystick));
        lockon = buttonJoystick.OnPressed;

        //视角锁定向左向右信号
        buttonLockLeft.Tick(Jright < -0.8);
        buttonLockRight.Tick(Jright > 0.8);
        lockLeft = buttonLockLeft.OnPressed;
        lockRight = buttonLockRight.OnPressed;

    }
}
