using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    [SerializeField] Player player;

    private Animator animator;

    #region AnimationParameterConsts

    private const string IS_MOVING = "IsWalking";

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool(IS_MOVING, player.IsWalking());
    }
}
