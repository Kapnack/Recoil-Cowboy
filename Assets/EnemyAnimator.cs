using UnityEngine;

public class EnemyAnimator : MonoBehaviour, IAnimate
{
    private Animator _animator;
    private readonly int _state = Animator.StringToHash("State");

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    
    public void ChangeAnimation(int state) =>
    _animator.SetInteger(_state, state);
}

public interface IAnimate
{
    public void ChangeAnimation(int state);
}
