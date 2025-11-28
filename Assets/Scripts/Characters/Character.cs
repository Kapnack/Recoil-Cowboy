using UnityEngine;

namespace Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Character : MonoBehaviour
    {
        protected Rigidbody Rb;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody>();
            
            Rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
    }
}