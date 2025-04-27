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
            UpdateMovementDirectionByKeys();
                
            HandleAttackInput();
        }
        
        void FixedUpdate()
        {
            if (movementDirection != Vector2.zero)
            {
                if(animController.isMovable)
                    rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
            }
        }

        // 이동 방향 설정
        void UpdateMovementDirectionByKeys()
        {
            if (!animController.isMovable) return;
            
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
            if (Input.GetKeyDown(KeyCode.J))
            {
                //AttackAttack
                    //Attack2
                animController.AttackInput();
            }
        }
    }
}