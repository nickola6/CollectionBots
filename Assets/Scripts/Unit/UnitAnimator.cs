using System.Collections;
using UnityEngine;

public class UnitAnimator
{
    private const int BaseLayer = 0;
    private const float StartTime = 0f;

    private readonly int _sitAnimation = Animator.StringToHash(nameof(UnitAnimationState.Sit));
    private readonly int _runAnimation = Animator.StringToHash(nameof(UnitAnimationState.Run));
    private readonly int _collectAnimation = Animator.StringToHash(nameof(UnitAnimationState.Collect));

    private Animator _animator;

    public void Initialize(Animator animator)
    {
        _animator = animator;
    }

    public void SetState(UnitAnimationState state)
    {
        switch (state)
        {
            case UnitAnimationState.Sit:
                _animator.Play(_sitAnimation, BaseLayer, StartTime);
                break;

            case UnitAnimationState.Run:
                _animator.Play(_runAnimation, BaseLayer, StartTime);
                break;

            case UnitAnimationState.Collect:
                _animator.Play(_collectAnimation, BaseLayer, StartTime);
                break;
        }
    }

    public IEnumerator WaitForAnimationFinished(UnitAnimationState state)
    {
        int stateHash = GetStateHash(state);
        bool isPlaying = true;

        while (isPlaying == true)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.shortNameHash == stateHash && stateInfo.normalizedTime >= 1f)
                isPlaying = false;

            yield return null;
        }
    }

    private int GetStateHash(UnitAnimationState state)
    {
        return state switch
        {
            UnitAnimationState.Sit => _sitAnimation,
            UnitAnimationState.Run => _runAnimation,
            UnitAnimationState.Collect => _collectAnimation,
            _ => 0,
        };
    }
}