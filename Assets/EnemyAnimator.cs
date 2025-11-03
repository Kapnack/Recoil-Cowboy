using UnityEngine;

public class EnemyAnimator : MonoBehaviour, IAnimate
{
    private Animator _animator;
    private static readonly int Hide = Animator.StringToHash("Hide");

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void ChangeAnimation(bool state) =>
        _animator.SetBool(Hide, state);
}

public interface IAnimate
{
    public void ChangeAnimation(bool state);
}