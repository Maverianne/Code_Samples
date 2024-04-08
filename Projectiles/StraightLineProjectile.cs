using System.Collections;
using _9YoS.Scripts.Pool;
using UnityEngine;

namespace _9YoS.Scripts.BossFights.Bubble
{
    public class StraightLineProjectile : PooledObject
    {
        [SerializeField] private GameObject europaTarget; 
        [SerializeField] private GameObject damageBox;
        [SerializeField] private GameObject[] visualComponents;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private float waitToFall = 1f;
        [SerializeField] private float riseSpeed = 10f;
        [SerializeField] private float fallSpeed = 7f;
        [SerializeField] private float highTargetDifference = 2.4f;
        [SerializeField] private float tolerance = 0.1f;
        [SerializeField] private float gravityScale = 1f;
        private bool ReachedHighTarget => Vector2.Distance(transform.position, _highTarget) < tolerance;
        private bool ReachedAttackTarget => Vector2.Distance(transform.position, _europaPos) < tolerance;
        
        private Vector2 _highTarget;
        private DropCompass _dropCompass;
        private GameObject _europa; 
        private Vector2 _europaPos;
        private Rigidbody2D _rigidbody2D;
        

        public bool Follow { get; private set; }

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _dropCompass = GetComponentInChildren<DropCompass>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetVisualComponents(true);
            _rigidbody2D.simulated = false;
        }

        public void SetProjectile(Vector2 pos)
        {
            transform.position = pos;
            _highTarget = new Vector2(pos.x, pos.y + highTargetDifference);
            damageBox.SetActive(false);
            Follow = false;
            StartCoroutine(Fall());
        }
        private void SetVisualComponents(bool status)
        {
            foreach (var component in visualComponents)
            {
                component.SetActive(status);
            }
            
            spriteRenderer.enabled = status;
            if (status)
            {
                particles.Play();
            }
        }
        private IEnumerator Fall()
        {
            while (!ReachedHighTarget)
            {
                transform.position =  Vector2.MoveTowards(transform.position, _highTarget, riseSpeed * Time.deltaTime);
                yield return null;
            }
            Follow = true;
            _dropCompass.FollowPlayer();
            damageBox.SetActive(true);
            
            yield return new WaitForSeconds(waitToFall);
            
            Follow = false;
            _europaPos = europaTarget.transform.position;
            _rigidbody2D.simulated = true;
            _rigidbody2D.gravityScale = 0;
            while (!ReachedAttackTarget)
            {
                var gameObjectPosition = transform.position;
                gameObjectPosition =  Vector2.MoveTowards(gameObjectPosition, _europaPos, fallSpeed * Time.deltaTime);
                transform.position = gameObjectPosition;
                yield return null;
            }
            _rigidbody2D.gravityScale = gravityScale;
        }

        public override void TerminateImmediatelyAndSilently()
        {
            _dropCompass.StopFollowPlayer();
            particles.Stop();
            damageBox.SetActive(false);
            base.TerminateImmediatelyAndSilently();
        }
    }
}
