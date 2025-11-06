using Systems;
using UnityEngine;

public class FollowTargetImproved : MonoBehaviour, IFollowTarget
{
    private Transform _target;

    [SerializeField] private float offsetY = 10f;
    [SerializeField] private float offsetZ = -20f;
    [SerializeField] private float lookDownDistance = -10f;

    private Transform Target
    {
        get => _target;

        set
        {
            _target = value;

            if (!_target)
                return;

            transform.position = new Vector3(_target.position.x, _target.position.y + offsetY,
                _target.position.z + offsetZ);
            transform.LookAt(_target);
        }
    }

    private void Awake() => ServiceProvider.SetService<IFollowTarget>(this);

    public void SetTarget(Transform target) => Target = target;

    private void LateUpdate()
    {
        if (!_target)
            return;

        transform.position = new Vector3(_target.position.x, _target.position.y + offsetY,
            _target.position.z + offsetZ);
    }
}