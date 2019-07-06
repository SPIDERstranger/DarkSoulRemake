using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    public bool OnPressed { get; private set; } = false;
    public bool IsPressing { get; private set; } = false;
    public bool OnReleased { get; private set; } = false;
    public bool IsExtending { get; private set; } = false;
    public bool IsDelaying { get; private set; } = false;

    public float extTime = 0.3f;
    public float delayTime = 0.3f;

    private bool curState = false;
    private bool lastState = false;

    private MyTimer ExtendTimer;
    private MyTimer DelayTimer;

    public MyButton(float ExtendTime = 1.0f, float DelayTime = 1.0f)
    {
        ExtendTimer = new MyTimer(extTime);
        DelayTimer = new MyTimer(delayTime);
    }

    public void Tick(bool buttonSign)
    {

        DelayTimer.Tick();
        ExtendTimer.Tick();

        curState = buttonSign;

        OnPressed = (curState && !lastState);
        IsPressing = curState;
        OnReleased = (!curState && lastState);

        lastState = curState;

        if (OnPressed)
            DelayTimer.Go();
        if (OnReleased)
            ExtendTimer.Go();

        IsDelaying = DelayTimer.TimerState == MyTimer.State.RUN;
        IsExtending = ExtendTimer.TimerState == MyTimer.State.RUN;


    }

}

public class MyTimer
{
    public enum State
    {
        IDLE,
        RUN
    }
    private float maxTime;
    private float timer;
    public State TimerState { get; private set; } = State.IDLE;

    public MyTimer(float maxTime)
    {
        this.maxTime = maxTime;
    }
    public void Tick()
    {
        switch (TimerState)
        {
            case State.IDLE:
                break;
            case State.RUN:
                timer += Time.deltaTime;
                if (timer >= maxTime)
                    TimerState = State.IDLE;
                break;
            default:
                Debug.LogWarning("Timer Error");
                break;
        }
    }
    public void Go()
    {
        TimerState = State.RUN;
        timer = 0;
    }
}
