using Systems;
using UnityEngine;
using UnityEngine.Serialization;

public class FollowTarget : MonoBehaviour, IFollowTarget
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

        Vector3 currentPos = transform.position;

        float targetY = _target.position.y > currentPos.y - offsetY
            ? _target.position.y + offsetY
            : currentPos.y;

        Vector3 desiredPos = new(
            _target.position.x,
            targetY,
            _target.position.z + offsetZ
        );

        transform.position = desiredPos;

        if (_target.position.y > currentPos.y - offsetY || _target.position.y < transform.position.y && _target.position.y > transform.position.y + lookDownDistance)
            transform.LookAt(_target);
    }
}

public interface IFollowTarget
{
    public void SetTarget(Transform target);
}