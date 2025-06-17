using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

namespace SmallScaleInc.TopDownPixelCharactersPack1
{
    public class PlayerControllerSample : MonoBehaviour
    {
        [LabelText("이동 속도")]
        public float speed = 2.0f;
        
        [LabelText("대시 이동 거리")]     public float attackDashDistance  = 0.45f;
        [LabelText("대시 지속 시간")]     public float attackDashDuration  = 0.08f;
        [LabelText("대시 후 감속 시간")]  public float dashBrakeTime       = 0.02f;
        [LabelText("공격 최소 간격")]     public float minAttackInterval   = 0.15f;
        
        [Header("Combo Settings")]
        [LabelText("콤보 허용 간격")] public float comboTolerance = 0.25f;  // 입력猶豫 시간
        public int   comboStep   = 0;    // 0-based (0,1,2)
        public float comboTimer  = 0f;
        
        [SerializeField] bool[] dashPerHit = { true, true, true };
        
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Vector2 movementDirection;
        private float lastAngle;

        private AnimationController animController;
        private CommandInput usingCommand;

        private bool isDashing;
        float nextAttackAllowedTime = 0f;
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animController = GetComponent<AnimationController>();
            usingCommand = GetComponent<CommandInput>();
        }
        void Update()
        {
            UpdateMovementDirectionByKeys();
                
            HandleAttackInput();
            
            if (comboTimer > 0f)
            {
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0f)
                {
                    comboStep  = 0;
                    comboTimer = 0f;
                }
            }
        }
        
        void FixedUpdate()
        {
            if (isDashing) return;
            
            if (movementDirection != Vector2.zero)
            {
                if(animController.isMovable)
                    rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
            }
        }

        // 이동 방향 설정
        void UpdateMovementDirectionByKeys()
        {
            if (!animController.isMovable || isDashing) return;
            
            bool w = Input.GetKey(KeyCode.W);
            bool a = Input.GetKey(KeyCode.A);
            bool s = Input.GetKey(KeyCode.S);
            bool d = Input.GetKey(KeyCode.D);

            movementDirection = Vector2.zero;

            if (w && d) { lastAngle = 45f; }
            else if (w && a) { lastAngle = 135f; }
            else if (s && d) { lastAngle = 315f; }
            else if (s && a) { lastAngle = 225f; }
            else if (w) { lastAngle = 90f; }
            else if (s) { lastAngle = 270f; }
            else if (a) { lastAngle = 180f; }
            else if (d) { lastAngle = 0f; }
            
            if (w || a || s || d)
            {
                movementDirection = new Vector2(Mathf.Cos(lastAngle * Mathf.Deg2Rad), Mathf.Sin(lastAngle * Mathf.Deg2Rad)).normalized;
            }

            animController.HandleMovement(lastAngle);
        }
        
        void HandleAttackInput()
        {
            if (!Input.GetKeyDown(KeyCode.J) || isDashing) return;

            // ① 첫 공격이거나, 타이머 안에서 들어온 추가 입력
            if (comboStep == 0 || comboTimer > 0f)
            {
                animController.AttackInput(comboStep);           // ▼ AnimationController 쪽으로 단계 전달
                if (dashPerHit[Mathf.Clamp(comboStep,0,2)])
                    StartCoroutine(AttackDash());                // 대시 사용 여부

                comboStep = (comboStep + 1) % 3;                 // 0→1→2→0
                comboTimer = comboTolerance;                     // 타이머 갱신
            }
        }
        
        IEnumerator AttackDash()
        {
            isDashing = true;

            Vector2 dashDir = AngleToVector(lastAngle);
            if (dashDir == Vector2.zero) dashDir = Vector2.right;
            
            float targetSpeed = attackDashDistance / attackDashDuration;
            
            float currentAlong = Vector2.Dot(rb.linearVelocity, dashDir);  // 현속도의 진행방향 성분
            float deltaSpeed   = Mathf.Max(0f, targetSpeed - currentAlong);
            float impulseMag   = rb.mass * deltaSpeed;
            
            yield return new WaitForSeconds(attackDashDuration);
            
            rb.AddForce(dashDir * impulseMag, ForceMode2D.Impulse);
            
            if (dashBrakeTime <= 0f)
            {
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                Vector2 startVel = rb.linearVelocity;
                float   t = 0f;
                while (t < dashBrakeTime)
                {
                    rb.linearVelocity = Vector2.Lerp(startVel, Vector2.zero, t / dashBrakeTime);
                    t += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                rb.linearVelocity = Vector2.zero;
            }
            
            yield return new WaitUntil(() => animController.isMovable);

            isDashing = false;
        }
        
        Vector2 AngleToVector(float angDeg)
        {
            float rad = angDeg * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
        }
    }
}