using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace TestFlexus.Scripts.Controllers
{
    public class CannonController : MonoBehaviour
    {
        [Header("Cannon parts")]
        [SerializeField] private GameObject cannonPivot;
        [SerializeField] private GameObject barrelPivot;

        [Header("Rotation Speeds")]
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private float elevationSpeed = 20f;

        [Header("Elevation Limits")]
        [SerializeField] private float barrelElevationUpLimit = 30f;
        [SerializeField] private float barrelElevationDownLimit = 10f;

        [Header("Fire Settings")]
        [SerializeField] private Transform startPosition;
        [SerializeField] private float force = 50f;
        [SerializeField] private float forceMultiplier = 1f;
        [SerializeField] private Bullet bullet;
        [SerializeField] private TrajectoryRenderer trajectoryRenderer;

        [Header("Barrel Animation")] 
        [SerializeField] private float moveZ;
        [SerializeField] private float animationTime;
        
        [SerializeField] private UnityEvent onFire;
        
        private float barrelAngleX = 0f;
    
        private void Update()
        {
            HandleRotation();

            if (Input.GetKeyDown(KeyCode.Space))
                Fire();
        }

        private void LateUpdate()
        {
            trajectoryRenderer.PredictTrajectory(ProjectileData());
        }

        private void HandleRotation()
        {
            float horizontal = 0f;
            if (Input.GetKey(KeyCode.A)) horizontal = -1f;
            else if (Input.GetKey(KeyCode.D)) horizontal = 1f;

            if (horizontal != 0f)
            {
                float rotationY = horizontal * rotationSpeed * Time.deltaTime;
                cannonPivot.transform.Rotate(0, rotationY, 0, Space.Self);
            }

            float vertical = 0f;
            if (Input.GetKey(KeyCode.W)) vertical = 1f;
            else if (Input.GetKey(KeyCode.S)) vertical = -1f;

            if (vertical != 0f)
            {
                barrelAngleX += vertical * elevationSpeed * Time.deltaTime;
                barrelAngleX = Mathf.Clamp(barrelAngleX, -barrelElevationDownLimit, barrelElevationUpLimit);

                barrelPivot.transform.localRotation = Quaternion.Euler(barrelAngleX, 0, 0);
            }
        }

        private PhysicsProjectileData ProjectileData() => new()
        {
            Direction = startPosition.forward, Position = startPosition.position, Speed = force * forceMultiplier, Mass = bullet.Mass, Drag = bullet.Drag
        };

        private void Fire()
        {
            onFire?.Invoke();

            barrelPivot.transform.DOLocalMoveZ(moveZ, animationTime).OnComplete(() => barrelPivot.transform.DOLocalMoveZ(0, animationTime));
            
            var shotProjectile = Instantiate(bullet, startPosition.position, Quaternion.identity);
            shotProjectile.Init(ProjectileData());
        }
    }
}

