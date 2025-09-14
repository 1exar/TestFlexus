using TestFlexus.Scripts.Transfer;
using UnityEngine;

namespace TestFlexus.Scripts
{
	public class TrajectoryRenderer : MonoBehaviour
	{
		[SerializeField] private LineRenderer trajectoryLine;
		[SerializeField] private Transform hitMarker;
	
		private readonly int pointsCount = 100;
		private readonly float hitMarkerOffset = 0.025f;

		private float time;

		private void Start()
		{
			trajectoryLine.material.mainTextureScale = new Vector2(trajectoryLine.positionCount, 1f);
		}

		public void PredictTrajectory(PhysicsProjectileData projectile)
		{
			time = 0; 
			Vector3 velocity = projectile.Direction * projectile.Speed;
			Vector3 position = projectile.Position;
			Vector3 nextPosition;

			UpdateLineRender(pointsCount, (0, position));

			for (int i = 1; i < pointsCount; i++)
			{
				velocity = (velocity * Mathf.Clamp01(1f - projectile.Drag * time)) + (Physics.gravity * projectile.Mass * time);
				nextPosition = position + velocity;
			
				if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, Vector3.Distance(position, nextPosition)))
				{
					UpdateLineRender(i, (i - 1, hit.point));
					MoveHitMarker(hit);
					break;
				}

				hitMarker.gameObject.SetActive(false);
				position = nextPosition;
				time += Time.fixedDeltaTime;
				UpdateLineRender(pointsCount, (i, position));
			}
		}
	
		private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
		{
			trajectoryLine.positionCount = count;
			trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
		}

		private void MoveHitMarker(RaycastHit hit)
		{
			hitMarker.gameObject.SetActive(true);
			hitMarker.position = hit.point + hit.normal * hitMarkerOffset;
			hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
		}
	}
}