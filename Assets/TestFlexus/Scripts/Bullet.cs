using TestFlexus.Scripts.Pools.Bullets;
using UnityEngine;

namespace TestFlexus.Scripts
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private BulletsPool thisPool;
        
        [Header("Physics Settings")]
        [SerializeField] private float mass;
        [SerializeField] private float drag;
        [SerializeField] private float bounce = 1f;
        [SerializeField] private int maxBounces = 1;

        [Header("Effects")]
        [SerializeField] private GameObject explosionPrefab;

        private int remainingBounces;
        private float flyTime;
        private float fallTime;

        private Vector3 velocity;
        private Vector3 nextPosition;

        private PhysicsProjectileData data;

        public float Mass => mass;
        public float Drag => drag;

        public void Init(PhysicsProjectileData projectileData)
        {
            data = projectileData;

            flyTime = 0;
            fallTime = 0;
            
            remainingBounces = maxBounces;
            velocity = data.Direction * data.Speed;
            transform.position = data.Position;
        }

        private void OnEnable()
        {
            
        }

        private void FixedUpdate()
        {
            SimulateMotion();
            flyTime += Time.fixedDeltaTime;
            fallTime += Time.fixedDeltaTime;
        }

        private void SimulateMotion()
        {
            velocity = velocity * Mathf.Clamp01(1f - data.Drag * flyTime) +
                       (Physics.gravity * mass * fallTime);

            nextPosition = transform.position + velocity;

            if (Physics.Raycast(transform.position, velocity.normalized, out RaycastHit hit, Vector3.Distance(transform.position, nextPosition)))
            {
                HandleCollision(hit);
            }
            else
            {
                transform.position = nextPosition;
            }
        }

        private void HandleCollision(RaycastHit hit)
        {
            velocity += hit.normal * velocity.magnitude *
                        (Vector3.Dot(hit.normal, -velocity.normalized) + bounce);

            transform.position = hit.point;
            fallTime = 0;
            remainingBounces--;

            if (remainingBounces < 0)
            {
                Explode(hit);
                thisPool.Release(this);
            }
            else
            {
                if (hit.collider.TryGetComponent(out SurfacePainter painter))
                {
                    painter.Paint(hit);
                }
            }
            
        }

        private void Explode(RaycastHit hit)
        {
            if (explosionPrefab != null)
            {
                var fx = Instantiate(explosionPrefab,
                    hit.point + hit.normal,
                    Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(fx, 2f);
            }
        }
    }
}
