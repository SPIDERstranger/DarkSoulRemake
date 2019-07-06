using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public IUserInput pi;
    public CameraController camcon;

    [Header("===== Player Setting =====")]
    public float walkSpeed = 1.4f;
    public float runMult = 2;
    public float jumpVelocity = 4f;
    public float rollVelocity = 2f;
    public float jabMult = 3f;

    [Space(10)]
    [Header("===== Physics Material =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;


    private float absForward=0;
    private float absRight=0;

    private Animator anim;
    private Rigidbody rigid;
    private CapsuleCollider Capsule;
    private Vector3 planarVec;
    private Vector3 thrustVec = Vector3.zero;
    private Vector3 deltaPos;
    private bool canAttack;
    private bool lockPlanar = false;
    private bool targetDirection = false;

    private float lerpTarget;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = model.GetComponent<Animator>();
        pi = GetComponent<IUserInput>();
        Capsule = GetComponent<CapsuleCollider>();

    }
    public void Update()
    {
        if (pi.lockon)
        {
            camcon.LockUnlock();
        }

        absForward =Mathf.Abs(anim.GetFloat("forward"));
        absRight = Mathf.Abs(anim.GetFloat("right"));

        if(!camcon.LockState)
        {
            anim.SetFloat("forward", pi.Dmag * (pi.run ? Mathf.Lerp(absForward, 2.0f, 0.3f) : Mathf.Lerp(absForward, 1.0f, 0.3f)));
            anim.SetFloat("right", 0);
            if (pi.Dmag > 0.01f)//修正模型旋转的
            {
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.1f);
            }
            if(!lockPlanar )
                planarVec = pi.Dmag*model.transform.forward  * walkSpeed * (pi.run ? runMult : 1.0f);
        }//未锁定状态
        else
        {
            Vector3 tempvec = transform.InverseTransformVector(pi.Dvec);
            anim.SetFloat("forward", tempvec.z * (pi.run ? Mathf.Lerp(absForward, 2.0f, 0.3f) : Mathf.Lerp(absForward, 1.0f, 0.3f)));
            anim.SetFloat("right", tempvec.x * (pi.run ? Mathf.Lerp(absRight, 2.0f, 0.2f) : Mathf.Lerp(absRight, 1.0f, 0.2f)));
            if (pi.lockRight || pi.lockLeft)
            {
                camcon.ChangeLock((pi.lockLeft ? -1 : 0) + (pi.lockRight ? 1 : 0));
            }

            if (!targetDirection)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = rigid.velocity.normalized;
            }
            if (!lockPlanar)
                planarVec = pi.Dvec* walkSpeed * (pi.run ? runMult : 1.0f);
        }//锁定状态

        if (pi.roll||rigid.velocity.y<-8f)
            anim.SetTrigger("roll");

        anim.SetBool("defense", pi.defense);

        if (pi.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }
        if (pi.attack && canAttack && CheckState("ground"))
            anim.SetTrigger("attack");


    }
    private void FixedUpdate()
    {
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }


    private bool CheckState(string stateName,string layerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }


    #region ///接受信息方法///

    //OnGroundSensor中调用
    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }
    //OnGroundSensor中调用
    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }
    public void OnGroundEnter()
    {
        pi.inputEnabled = true;
        lockPlanar = false;
        canAttack = true;
        targetDirection = false;
        Capsule.material = frictionOne;
    }
    public void OnGroundExit()
    {
        Capsule.material = frictionZero;
    }

    public void OnFallEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;

    }
    public void OnJumpEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;
        thrustVec = Vector3.up * jumpVelocity;
        targetDirection = true;
    }
    public void OnRollEnter()
    {
        thrustVec = Vector3.up * rollVelocity;
        thrustVec +=model. transform.forward * rollVelocity*4;
        pi.inputEnabled = false;
        lockPlanar = true;
        targetDirection = true;
    }
    public void OnJabEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;
    }
    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity");
    }
    public void OnAttackIdleEnter()
    {
        lerpTarget = 0;
        pi.inputEnabled = true;
    }
    public void OnAttackIdleUpdate()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.1f));
    }

    public void OnAttack1hAEnter()
    {
        lerpTarget = 1;
        pi.inputEnabled = false;
    }
    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.3f));
    }

    public void OnAnimMove(object _deltaPos)
    {
        if(CheckState("attack1hC","attack"))
             deltaPos =0.5f*deltaPos+ 0.5f*(Vector3) _deltaPos;//因为，调用的速度不相同，故使用了累加
    }


    #endregion
}
