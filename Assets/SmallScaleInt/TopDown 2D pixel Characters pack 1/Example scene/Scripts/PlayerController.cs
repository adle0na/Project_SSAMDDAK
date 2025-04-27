using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

namespace SmallScaleInc.TopDownPixelCharactersPack1
{
    public class PlayerController : MonoBehaviour
    {
        [LabelText("이동 속도")]
        public float speed = 2.0f;
        
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Vector2 movementDirection;
        private float lastAngle;

        private AnimationController animController;
        private CommandInput usingCommand;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animController = GetComponent<AnimationController>();
            usingCommand = GetComponent<CommandInput>();
        }
        void Update()
        {
            ReadInput();
            animController?.HandleMovement(lastAngle, movementDirection);
            HandleAttackInput();    
        }
        
        void FixedUpdate()
        {
            if (animController != null && !animController.isMovable) return;

            rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
        }

        // 이동 방향 설정
        void ReadInput()
        {
            bool w = Input.GetKey(KeyCode.W);
            bool a = Input.GetKey(KeyCode.A);
            bool s = Input.GetKey(KeyCode.S);
            bool d = Input.GetKey(KeyCode.D);

            // 각도 계산 (생략 부분 동일)
            /* … */

            // 이동 벡터
            movementDirection = (w||a||s||d)
                ? new Vector2(Mathf.Cos(lastAngle * Mathf.Deg2Rad),
                    Mathf.Sin(lastAngle * Mathf.Deg2Rad)).normalized
                : Vector2.zero;
        }
        
        void HandleAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                //AttackAttack
                    //Attack2
                animController.AttackInput();
            }
        }
    }
}