using System.Collections;
using UnityEngine;

public class UnitAnimator
{
    private const int BaseLayer = 0;
    private const float AnimationFinishedTime = 1f;

    private readonly int _sitAnimation = Animator.StringToHash(nameof(UnitAnimationState.Sit));
    private readonly int _runAnimation = Animator.StringToHash(nameof(UnitAnimationState.Run));
    private readonly int _collectAnimation = Animator.StringToHash(nameof(UnitAnimationState.Collect));

    private readonly Animator _animator;

    public UnitAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void SetState(UnitAnimationState state)
    {
        _animator.Play(GetStateHash(state), BaseLayer, 0f);
    }

    public IEnumerator FinishedRoutine(UnitAnimationState state)
    {
        int stateHash = GetStateHash(state);

        while (IsFinished(stateHash) == false)
            yield return null;
    }

    private bool IsFinished(int stateHash)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(BaseLayer);

        return stateInfo.shortNameHash == stateHash && stateInfo.normalizedTime >= AnimationFinishedTime;
    }

    private int GetStateHash(UnitAnimationState state)
    {
        return state switch
        {
            UnitAnimationState.Sit => _sitAnimation,
            UnitAnimationState.Run => _runAnimation,
            UnitAnimationState.Collect => _collectAnimation,
            _ => throw new System.ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
}