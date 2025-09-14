using DG.Tweening;
using UnityEngine;

namespace TestFlexus.Scripts.Controllers
{
    public class CameraShakeController : MonoBehaviour
    {
        
        [SerializeField] private float shakeStrength = 5f;
        [SerializeField] private float shakeDuration = 0.5f;
        
        public void MakeShake() => transform.DOShakePosition(shakeDuration, shakeStrength);
        
    }
}
