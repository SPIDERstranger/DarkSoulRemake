using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void RestTrigger(string triggerName)
    {
        animator.ResetTrigger(triggerName);
    }

    
}
