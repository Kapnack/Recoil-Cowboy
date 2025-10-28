using MouseTracker;
using Systems;
using UnityEngine;

public class GunRotation : MonoBehaviour, IGunRotation
{
    private IMousePositionTracker _mouseTracker;
    private Transform _player;

    private void Awake()
    {
        _mouseTracker = ServiceProvider.GetService<IMousePositionTracker>();
        _player = transform.parent.gameObject.transform;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;
        
        Vector3 dir = _mouseTracker.GetMouseDir(_player);

        RotateGun(_player, dir.x, dir.y);
    }

    public void RotateGun(Transform owner, float x, float y)
    {
        float angle = Mathf.Atan2(y, x);
        const float radius = 1f;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), -0.5f) * radius;
        transform.position = owner.position + offset;

        Vector3 dirToCenter = (owner.position - transform.position).normalized;
        
        Quaternion rot = Quaternion.FromToRotation(Vector3.right, -dirToCenter);
        transform.rotation = rot;
    }
}

public interface IGunRotation
{
    public void RotateGun(Transform owner, float x, float y);
}