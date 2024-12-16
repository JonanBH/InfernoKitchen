using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    [SerializeField] private ContainerCounter containerCounter;
    private Animator animator;

    const string OPEN_CLOSE_TRIGGER_NAME = "OpenClose";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        containerCounter.OnPlayerGrabberObject += ContainerCounter_OnPlayerGrabberObject;
    }

    private void ContainerCounter_OnPlayerGrabberObject(object sender, System.EventArgs e)
    {
        animator.SetTrigger(OPEN_CLOSE_TRIGGER_NAME);
    }
}
