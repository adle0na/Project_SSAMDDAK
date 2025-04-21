using UnityEngine;
using System.Collections;

namespace SmallScaleInc.TopDownPixelCharactersPack1
{
    public class PlayerController : MonoBehaviour
    {
        public float speed = 2.0f;
        private Rigidbody2D rb;
        private Vector2 movementDirection;
        private SpriteRenderer spriteRenderer;
        private float lastAngle;
        
        public bool isRanged; //If the character is an archer OR caster character
        public GameObject projectilePrefab; //prefab to the projectile
        public GameObject AoEPrefab;

        public float projectileSpeed = 10.0f; // Speed at which the projectile travels
        public float shootDelay = 0.5f; // Delay in seconds before the projectile is fired
        
        private AnimationController animController;
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animController = GetComponent<AnimationController>();
        }
        void Update()
        {
            UpdateMovementDirectionByKeys();

            if (isRanged)
            {
                if (Input.GetKeyDown(KeyCode.J))
                    Invoke(nameof(DelayedShoot), shootDelay);
                // if (Input.GetKeyDown(KeyCode.Alpha5))
                //     StartCoroutine(Quickshot());
                // if (Input.GetKeyDown(KeyCode.Alpha6))
                //     StartCoroutine(CircleShot());
                // if (Input.GetKeyDown(KeyCode.Alpha3))
                //     StartCoroutine(DeployAoEDelayed());
            }
        }
        
        void FixedUpdate()
        {
            if (animController != null && animController.isAttacking) return;

            if (movementDirection != Vector2.zero)
            {
                rb.MovePosition(rb.position + movementDirection * speed * Time.fixedDeltaTime);
            }
        }

        void UpdateMovementDirectionByKeys()
        {
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
            
            GetComponent<AnimationController>()?.HandleMovement(lastAngle);
        }
        
        float SnapAngleToEightDirections(float angle)
        {
            angle = (angle + 360) % 360;
            
            if (angle < 15 || angle >= 345)
                return 0; // East (isEast)
            else if (angle >= 15 && angle < 60)
                return 35; // Northeast (isNorthEast)
            else if (angle >= 60 && angle < 120)
                return 90; // North (isNorth)
            else if (angle >= 120 && angle < 165)
                return 145; // Northwest (isNorthWest)
            else if (angle >= 165 && angle < 195)
                return 180; // West (isWest)
            else if (angle >= 195 && angle < 240)
                return 215; // Southwest (isSouthWest)
            else if (angle >= 240 && angle < 300)
                return 270; // South (isSouth)
            else if (angle >= 300 && angle < 345)
                return 330; // Southeast (isSouthEast)
            
            return 0;
        }

        public void SetArcherStatus(bool status)
        {
            isRanged = status;
        }
        
        void DelayedShoot()
        {
            Vector2 fireDirection = new Vector2(Mathf.Cos(lastAngle * Mathf.Deg2Rad), Mathf.Sin(lastAngle * Mathf.Deg2Rad));
            ShootProjectile(fireDirection);
        }

        void ShootProjectile(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
            Rigidbody2D rbProjectile = projectileInstance.GetComponent<Rigidbody2D>();
            if (rbProjectile != null)
            {
                rbProjectile.linearVelocity = direction * projectileSpeed;
            }
        }

        IEnumerator Quickshot()
        {
            // Initial small delay before starting the quickshot sequence
            yield return new WaitForSeconds(0.1f);

            // Loop to fire five projectiles in the facing direction
            for (int i = 0; i < 5; i++)
            {
                Vector2 fireDirection = new Vector2(Mathf.Cos(lastAngle * Mathf.Deg2Rad), Mathf.Sin(lastAngle * Mathf.Deg2Rad));
                ShootProjectile(fireDirection);

                // Wait for 0.18 seconds before firing the next projectile
                yield return new WaitForSeconds(0.18f);
            }
        }
        
        IEnumerator CircleShot()
        {
            float initialDelay = 0.1f;
            float timeBetweenShots = 0.9f / 8;  // Total time divided by the number of shots

            yield return new WaitForSeconds(initialDelay);

            // Use the lastAngle as the start angle and generate projectiles in 8 directions
            for (int i = 0; i < 8; i++)
            {
                float angle = lastAngle + i * 45;  // Increment by 45 degrees for each direction
                angle = Mathf.Deg2Rad * angle;  // Convert to radians for direction calculation
                Vector2 fireDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                ShootProjectile(fireDirection);

                yield return new WaitForSeconds(timeBetweenShots);
            }
        }

        IEnumerator DeployAoEDelayed()
        {
            if (AoEPrefab != null)
            {
                // Wait for 0.3 seconds before instantiating the AoEPrefab
                yield return new WaitForSeconds(0.3f);

                // Instantiate the AoE prefab at the player's position
                GameObject aoeInstance = Instantiate(AoEPrefab, transform.position, Quaternion.identity);

                // Destroy the instantiated prefab after another 0.5 seconds
                Destroy(aoeInstance, 0.5f);
            }
        }
    }
}