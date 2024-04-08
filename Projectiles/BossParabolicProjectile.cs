using System.Collections;
using _9YoS.Scripts.Pool;
using UnityEngine;

namespace _9YoS.Scripts.BossFights.Bubble
{
    public class BossParabolicProjectile : PooledObject
    {
        [SerializeField] private float activateDamageBox;
        [SerializeField] private GameObject damageBox;
        [SerializeField] private GameObject[] visualComponents;
        [SerializeField] private ParticleSystem particles;
        private Rigidbody2D _rb;
        private IEnumerator _damageBoxActivate;
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            damageBox.SetActive(false);
            if (_damageBoxActivate != null)
            {
                StopCoroutine(_damageBoxActivate);
                _damageBoxActivate = null;
            }
               
            SetVisualComponents(true);
        }
        private void SetVisualComponents(bool status)
        {
            foreach (var component in visualComponents)
            {
                component.SetActive(status);
            }

            if (status)
            {
                particles.Play();
            }
        }

        public void ApplyInitialForce(float maxHeight, Vector2 originPos, Vector2 targetPos)
        {
            Vector2 v0;

            var gravity = Mathf.Abs(Physics2D.gravity.y);
            var shortTime = Mathf.Sqrt(Mathf.Abs(2 * (maxHeight - Mathf.Max(originPos.y, targetPos.y)) / gravity));
            var totalTime = Mathf.Sqrt(Mathf.Abs(2 * (maxHeight - Mathf.Min(originPos.y, targetPos.y)) / gravity));

            v0.y = gravity * totalTime;
            var distance = targetPos.x - originPos.x;
            v0.x = distance / (shortTime + totalTime);

            _rb.AddForce(v0, ForceMode2D.Impulse);
            _damageBoxActivate = ActivateDamageBox(activateDamageBox);
            StartCoroutine(_damageBoxActivate);
        }

        public override void TerminateImmediatelyAndSilently()
        {
            particles.Stop();
            damageBox.SetActive(false);
            if (_damageBoxActivate == null) return;
            StopCoroutine(_damageBoxActivate);
            _damageBoxActivate = null;
            base.TerminateImmediatelyAndSilently();
        }

        private IEnumerator ActivateDamageBox(float time)
        {
            yield return new WaitForSeconds(time);
            damageBox.SetActive(true);
        }
    }
}
