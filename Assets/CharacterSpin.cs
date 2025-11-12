using Systems.LayerClassGenerator;
using UnityEngine;

public class CharacterSpin : MonoBehaviour, ICharacterSpin
{
    [SerializeField] private float maxSpeedSpin;

    private bool _spin = false;
    private float _startingRotation = 0;

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
            float absVelY = Mathf.Abs(_rb.linearVelocity.y);
            float newSpeed = Mathf.Max(_lastMaxSpeed, absVelY);

            _lastMaxSpeed = Mathf.Min(newSpeed, maxSpeedSpin);

            if (absVelY > 0.01f)
            {
                Vector3 currentRot = transform.localEulerAngles;
                currentRot.z += _lastMaxSpeed * Mathf.Sign(_dir.x) * Time.deltaTime * 100f;
                transform.localEulerAngles = currentRot;
            }

            if (_rb.linearVelocity.y >= 0)
                return;

            if (Physics.BoxCast(transform.position, new Vector3(0.2f, 0.5f, 0.5f), Vector3.down,
                    Quaternion.identity, 1.0f, LayerMask.GetMask(Layers.Environment)))
            {
                transform.localRotation = Quaternion.Euler(
                    transform.localEulerAngles.x,
                    transform.localEulerAngles.y,
                    _startingRotation);

                StopSpin();
            }
        }
    }
}

public interface ICharacterSpin
{
    public void SetSpin(Vector3 dir);
}