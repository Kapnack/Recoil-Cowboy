using System.Collections;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.LayerClassGenerator;
using UnityEngine;

public class CharacterSpin : MonoBehaviour, ICharacterSpin
{
    [SerializeField] private float maxSpeedSpin;

    private bool _spin = false;
    private float _startingRotation = 0;

    private bool _returning = false;
    private const float ReturnSpeed = 360f;

    private float _lastMaxSpeed;
    private Vector3 _dir;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponentInParent<Rigidbody>();
    }

    private void Start()
    {
        _startingRotation = transform.localRotation.z;
    }

    public void SetSpin(Vector3 dir)
    {
        _dir = dir;
        _spin = true;
    }

    private void StopSpin() => _spin = false;

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (_spin)
        {
            float newSpeed = Mathf.Max(_lastMaxSpeed, Mathf.Abs(_rb.linearVelocity.y));

            _lastMaxSpeed = newSpeed > maxSpeedSpin ? maxSpeedSpin : newSpeed;

            Vector3 currentRot = transform.localEulerAngles;
            currentRot.z += _lastMaxSpeed * Mathf.Sign(_dir.x);
            transform.localEulerAngles = currentRot;

            if (_rb.linearVelocity.y >= 0)
                return;
            
            if (Physics.BoxCast(transform.position, new Vector3(0.2f, 0.5f, 0.5f), Vector3.down,
                    Quaternion.identity, 1.0f, LayerMask.GetMask(Layers.Environment)))
            {
                transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y,
                    _startingRotation);

                StopSpin();
            }
        }
        else if (_returning)
        {
            Quaternion targetRot = Quaternion.Euler(
                transform.localRotation.eulerAngles.x,
                transform.localRotation.eulerAngles.y,
                _startingRotation);

            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                targetRot,
                ReturnSpeed * transform.localRotation.eulerAngles.y);

            if (Quaternion.Angle(transform.localRotation, targetRot) < 0.5f)
            {
                transform.localRotation = targetRot;
                _returning = false;
            }
        }
    }
}

public interface ICharacterSpin
{
    public void SetSpin(Vector3 dir);
}