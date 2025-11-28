using System;
using Characters;
using Characters.PlayerSRC;
using UnityEngine;

[Serializable]
public class TheVoid : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 20.0f;

    [SerializeField] private Transform target;

    [SerializeField] private float distanceFromPlayer = 10f;

    private BoxCollider _boxCollider;

    private float _relativeY;
    [SerializeField] private bool instantMovement;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _relativeY = GetRelativeY(target);
        transform.position = new Vector3(target.position.x, _relativeY, target.position.z);
    }

    private void Update()
    {
        if (target.position.y >
            transform.position.y + transform.localScale.y * _boxCollider.size.y * 0.5f + distanceFromPlayer)
        {
            if (instantMovement)
                _relativeY = GetRelativeY(target);
            else
                _relativeY = transform.position.y + movementSpeed * Time.deltaTime;
        }

        transform.position = new Vector3(0, _relativeY, 0);
    }

    private float GetRelativeY(Transform other)
    {
        return other.position.y - transform.localScale.y * _boxCollider.size.y / 2 - distanceFromPlayer;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out IHealthSystem healthSystem))
            return;

        healthSystem.InstantDead();
    }

    private void OnTriggerStay(Collider other) => OnTriggerEnter(other);
}