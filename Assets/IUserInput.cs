using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IUserInput : MonoBehaviour
{
    [Space(10)]
    [Header("===== output signals =====")]
    public float Dup;
    public float Dright;
    public float Dmag; //方向向量长度
    public Vector3 Dvec;

    public float Jup;
    public float Jright;

    //press type
    public bool run;
    public bool defense;
    //trigger type
    public bool jump;
    public bool roll;
    public bool attack;
    public bool lockon;
    public bool lockLeft;
    public bool lockRight;
    [Space(10)]
    [Header("===== other =====")]
    public bool inputEnabled = true;

    protected float targetDup;
    protected float targetDright;
    protected float velocityDup;
    protected float velocityDright;

    public Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1.0f - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1.0f - (input.x * input.x) / 2.0f);
        return output;
    }
}
