using UnityEngine;

public class ChickenAnimator : MonoBehaviour, IAnimate
{
    private Animator _animator;
    private static readonly int Jumping = Animator.StringToHash("Jumping");

    private void Awake() =>
        _animator = GetComponentInChildren<Animator>();

    public void ChangeAnimation(bool state) =>
        _animator.SetBool(Jumping, state);
}