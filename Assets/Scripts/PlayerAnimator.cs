using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
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
        if (!IsOwner) return;

        animator.SetBool(IS_MOVING, player.IsWalking());
    }
}
