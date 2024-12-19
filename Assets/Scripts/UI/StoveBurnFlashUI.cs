using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurnFlashUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;

    private const string IS_FLASHING = "IsFlashing";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        bool isFlashing = e.progressNormalized >= burnShowProgressAmount && stoveCounter.IsFried();

        animator.SetBool(IS_FLASHING, isFlashing);
    }
}
