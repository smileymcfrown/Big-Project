using DumplingKitchen.Dumplings;
using UnityEngine;

namespace DumplingKitchen.Sabotage
{
    /// <summary>
    /// A pot, jug or stack of plates the dumplings can shove off the counter.
    /// It counts as "restored" when the chef picks it up again (add a ChefRestoreHandle
    /// next to its XR Grab Interactable).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Knockable : SabotageTarget
    {
        [SerializeField, Min(0f)] private float shoveSpeed = 2.5f;
        [SerializeField, Min(0f)] private float upwardSpeed = 1.5f;
        [SerializeField, Min(0f)] private float tumble = 6f;
        [SerializeField] private AudioSource crashSound;

        private Rigidbody _body;

        private void Awake() => _body = GetComponent<Rigidbody>();

        protected override void OnSabotagedBy(DumplingInteractor dumpling)
        {
            Vector3 away = Vector3.ProjectOnPlane(transform.position - dumpling.transform.position, Vector3.up).normalized;

            // VelocityChange ignores mass, so a heavy pot and a light cup fly the same way.
            _body.AddForce(away * shoveSpeed + Vector3.up * upwardSpeed, ForceMode.VelocityChange);
            _body.AddTorque(Random.onUnitSphere * tumble, ForceMode.VelocityChange);
        }

        protected override void OnSabotaged()
        {
            if (crashSound != null)
                crashSound.Play();
        }

        protected override void OnRestored() { }
    }
}
