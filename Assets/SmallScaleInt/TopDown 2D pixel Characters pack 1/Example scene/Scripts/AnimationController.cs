using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace SmallScaleInc.TopDownPixelCharactersPack1
{
    public class AnimationController : MonoBehaviour
    {
        private Animator animator;
        
        [LabelText("현재 방향")]
        public string currentDirection = "isEast";
        
        [LabelText("이동 가능 여부")]
        public bool isMovable = true;
        
        [LabelText("이동중")]
        public bool isRunning;

        void Start()
        {
            animator = GetComponent<Animator>();
            animator.SetBool("isEast", true); //Sets the default direction to east.
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouchRunning", false);
            animator.SetBool("isCrouchIdling", false);
        }
        
        public void HandleMovement(float angle)
        {
            if (!isMovable) return;
            
            if (angle < 0 || 
                (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && 
                 !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)))
            {
                isRunning = false;

                animator.SetBool("isRunning", false);
                animator.SetBool("isCrouchRunning", false);
                ResetAllMovementBools();

                animator.SetBool(currentDirection, true);
                return;
            }
            
            if (angle < 0) angle += 360;
            
            string newDirection = DetermineDirectionFromAngle(angle);
            UpdateDirection(newDirection);
            string movementDirection = newDirection.Substring(2); // Remove "is" from the direction name

            // Capture movement input states
            isRunning = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
            
            // Reset all directional movement parameters
            ResetAllMovementBools();
            
            animator.SetBool("isRunning", isRunning);

            SetMovementAnimation(true, "Move", movementDirection);
        }

        void UpdateDirection(string newDirection)
        {
            SetDirectionBools(false, false, false, false, false, false, false, false);
            
            animator.SetBool(newDirection, true);

            currentDirection = newDirection;

            ResetAttackAttackParameters();
        }
        
        void SetMovementAnimation(bool isActive, string baseKey, string direction)
        {
            if (isActive)
            {
                string animationKey = $"{baseKey}{direction}";
                animator.SetBool(animationKey, true);
            }
        }

        void ResetAllMovementBools()
        {
            string[] directions = new string[] { "North", "South", "East", "West", "NorthEast", "NorthWest", "SouthEast", "SouthWest" };
            foreach (string baseKey in new string[] { "Move"})
            {
                foreach (string direction in directions)
                {
                    animator.SetBool($"{baseKey}{direction}", false);
                }
            }

            animator.SetBool("CrouchRunNorth", false);
            animator.SetBool("CrouchRunSouth", false);
            animator.SetBool("CrouchRunEast", false);
            animator.SetBool("CrouchRunWest", false);
            animator.SetBool("CrouchRunNorthEast", false);
            animator.SetBool("CrouchRunNorthWest", false);
            animator.SetBool("CrouchRunSouthEast", false);
            animator.SetBool("CrouchRunSouthWest", false);
        }
        
        string DetermineDirectionFromAngle(float angle)
        {
            // Adjust the ranges based on your snapped angles
            if ((angle >= 330 || angle < 15))
                return "isEast"; // Corresponds to angle 0
            else if ((angle >= 15 && angle < 60))
                return "isNorthEast"; // Corresponds to angle 35
            else if ((angle >= 60 && angle < 120))
                return "isNorth"; // Corresponds to angle 90
            else if ((angle >= 120 && angle < 165))
                return "isNorthWest"; // Corresponds to angle 150
            else if ((angle >= 165 && angle < 195))
                return "isWest"; // Corresponds to angle 180
            else if ((angle >= 195 && angle < 240))
                return "isSouthWest"; // Corresponds to angle 215
            else if ((angle >= 240 && angle < 300))
                return "isSouth"; // Corresponds to angle 270
            else if ((angle >= 300 && angle < 345))
                return "isSouthEast"; // Corresponds to angle 330

            return "isEast"; // Default direction
        }
        
        void SetDirectionBools(bool isWest, bool isEast, bool isSouth, bool isSouthWest, bool isNorthEast, bool isSouthEast, bool isNorth, bool isNorthWest)
        {
            animator.SetBool("isWest", isWest);
            animator.SetBool("isEast", isEast);
            animator.SetBool("isSouth", isSouth);
            animator.SetBool("isSouthWest", isSouthWest);
            animator.SetBool("isNorthEast", isNorthEast);
            animator.SetBool("isSouthEast", isSouthEast);
            animator.SetBool("isNorth", isNorth);
            animator.SetBool("isNorthWest", isNorthWest);
        }

        string GetCurrentDirection()
        {
            if (animator.GetBool("isNorth"))      return "North";
            if (animator.GetBool("isSouth"))      return "South";
            if (animator.GetBool("isEast"))       return "East";
            if (animator.GetBool("isWest"))       return "West";
            if (animator.GetBool("isNorthEast"))  return "NorthEast";
            if (animator.GetBool("isNorthWest"))  return "NorthWest";
            if (animator.GetBool("isSouthEast"))  return "SouthEast";
            return "SouthWest";
        }

        public void AttackInput()
        {
            TriggerAttack();
        }
        
        void ResetAttackAttackParameters()
        {
            // Reset both AttackAttack and Attack2 parameters for all dir ections
            string[] directions = new string[] { "North", "South", "East", "West", "NorthEast", "NorthWest", "SouthEast", "SouthWest" };
            foreach (string dir in directions)
            {
                animator.SetBool("AttackAttack" + dir, false);
                animator.SetBool("Attack2" + dir, false);
                animator.SetBool("AttackRun" + dir, false);
            }
            
            RestoreDirectionAfterAttack();
        }
        
        void RestoreDirectionAfterAttack()
        {
            animator.SetBool("isAttackAttacking", false);
            animator.SetBool("isAttackRunning", false);
            animator.SetBool("isRunning", false);
            SetDirectionBools(false, false, false, false, false, false, false, false); // Reset all directions
            animator.SetBool(currentDirection, true);

            isMovable = true;
        }

        #region Animations

        public void TriggerAttack()
        {
            if (!gameObject.activeInHierarchy) return;

            animator.SetBool("isAttackAttacking", true);

            // 방향별 파라미터 선택
            string param = currentDirection switch
            {
                "isNorth"      => "AttackAttackNorth",
                "isSouth"      => "AttackAttackSouth",
                "isEast"       => "AttackAttackEast",
                "isWest"       => "AttackAttackWest",
                "isNorthEast"  => "AttackAttackNorthEast",
                "isNorthWest"  => "AttackAttackNorthWest",
                "isSouthEast"  => "AttackAttackSouthEast",
                "isSouthWest"  => "AttackAttackSouthWest",
                _              => string.Empty
            };

            if (param != string.Empty)
            {
                animator.SetBool(param, true);
                StartCoroutine(WaitForAttackEnd(param));
            }
        }
        
        private IEnumerator WaitForAttackEnd(string attackParam)
        {
            /* ── 1) 실제로 해당 스테이트에 들어올 때까지 기다린다 ── */
            yield return null; // 다음 프레임
            yield return new WaitUntil(() =>
            {
                var info = animator.GetCurrentAnimatorStateInfo(0);
                return info.IsName(attackParam);         // 지정 스테이트에 진입?
            });

            isMovable = false;                          // 이동 잠금

            /* ── 2) 스테이트가 끝날 때까지 기다린다 ── */
            yield return new WaitUntil(() =>
            {
                var info = animator.GetCurrentAnimatorStateInfo(0);
                bool finished = !animator.IsInTransition(0) &&
                                info.IsName(attackParam) &&
                                info.normalizedTime >= 0.99f;   // 99 % 이상 재생
                return finished;
            });

            /* ── 3) 플래그·상태 리셋 ── */
            animator.SetBool("isAttackAttacking", false);
            animator.SetBool(attackParam,           false);

            // 필요하다면 여기에 특수 스킬 플래그들 초기화
            // ResetSpecialAbilityFlags();

            RestoreDirectionAfterAttack();
            isMovable = true;
        }
        
        public void TriggerDie()
        {
            Debug.Log("입력감지");
            if (!gameObject.activeInHierarchy)
            {
                Debug.Log("리턴체크1");
                return;
            }
            // Ensure that we're indicating a death state is happening
            animator.SetBool("isDie", true);
            // Check the current direction and trigger the appropriate die animation
            if (currentDirection.Equals("isNorth")) TriggerDeathAnimation("dieNorth");
            else if (currentDirection.Equals("isSouth")) TriggerDeathAnimation("dieSouth");
            else if (currentDirection.Equals("isEast")) TriggerDeathAnimation("dieEast");
            else if (currentDirection.Equals("isWest")) TriggerDeathAnimation("dieWest");
            else if (currentDirection.Equals("isNorthEast")) TriggerDeathAnimation("dieNorthEast");
            else if (currentDirection.Equals("isNorthWest")) TriggerDeathAnimation("dieNorthWest");
            else if (currentDirection.Equals("isSouthEast")) TriggerDeathAnimation("dieSouthEast");
            else if (currentDirection.Equals("isSouthWest")) TriggerDeathAnimation("dieSouthWest");
            
            Debug.Log("리턴체크2");
        }

        private void TriggerDeathAnimation(string deathDirectionParam)
        {
            // Trigger the specific death direction
            animator.SetBool(deathDirectionParam, true);
            // Reset all die direction parameters to ensure only the correct direction is triggered
            StartCoroutine(ResetDieParameters());
        }

        IEnumerator ResetDieParameters()
        {
            yield return new WaitForSeconds(2);

            animator.SetBool("isDie", false);
            animator.SetBool("dieNorth", false);
            animator.SetBool("dieSouth", false);
            animator.SetBool("dieEast", false);
            animator.SetBool("dieWest", false);
            animator.SetBool("dieNorthEast", false);
            animator.SetBool("dieNorthWest", false);
            animator.SetBool("dieSouthEast", false);
            animator.SetBool("dieSouthWest", false);
            // Restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack();
        }
        
        public void TriggerSpecialAbility1Animation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isSpecialAbility1' to true to initiate the special ability animation
            animator.SetBool("isSpecialAbility1", true);

            // Determine the current direction and trigger the appropriate special ability animation
            if (animator.GetBool("isNorth")) animator.SetBool("SpecialAbility1North", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("SpecialAbility1South", true);
            else if (animator.GetBool("isEast")) animator.SetBool("SpecialAbility1East", true);
            else if (animator.GetBool("isWest")) animator.SetBool("SpecialAbility1West", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("SpecialAbility1NorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("SpecialAbility1NorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("SpecialAbility1SouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("SpecialAbility1SouthWest", true);

            // Reset the special ability parameters after a delay or at the end of the animation
            StartCoroutine(ResetSpecialAbility1Parameters());
        }

        IEnumerator ResetSpecialAbility1Parameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.5f); // Adjust the wait time based on your animation length

            // Reset all special ability parameters to false
            animator.SetBool("isSpecialAbility1", false);
            animator.SetBool("SpecialAbility1North", false);
            animator.SetBool("SpecialAbility1South", false);
            animator.SetBool("SpecialAbility1East", false);
            animator.SetBool("SpecialAbility1West", false);
            animator.SetBool("SpecialAbility1NorthEast", false);
            animator.SetBool("SpecialAbility1NorthWest", false);
            animator.SetBool("SpecialAbility1SouthEast", false);
            animator.SetBool("SpecialAbility1SouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack();
        }
        
        public void TriggerSpecialAbility2Animation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isSpecialAbility1' to true to initiate the special ability animation
            animator.SetBool("isSpecialAbility2", true);

            // Determine the current direction and trigger the appropriate special ability animation
            if (animator.GetBool("isNorth")) animator.SetBool("SpecialAbility2North", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("SpecialAbility2South", true);
            else if (animator.GetBool("isEast")) animator.SetBool("SpecialAbility2East", true);
            else if (animator.GetBool("isWest")) animator.SetBool("SpecialAbility2West", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("SpecialAbility2NorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("SpecialAbility2NorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("SpecialAbility2SouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("SpecialAbility2SouthWest", true);

            // Reset the special ability parameters after a delay or at the end of the animation
            StartCoroutine(ResetSpecialAbility2Parameters());
        }

        IEnumerator ResetSpecialAbility2Parameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.5f); // Adjust the wait time based on your animation length

            // Reset all special ability parameters to false
            animator.SetBool("isSpecialAbility2", false);
            animator.SetBool("SpecialAbility2North", false);
            animator.SetBool("SpecialAbility2South", false);
            animator.SetBool("SpecialAbility2East", false);
            animator.SetBool("SpecialAbility2West", false);
            animator.SetBool("SpecialAbility2NorthEast", false);
            animator.SetBool("SpecialAbility2NorthWest", false);
            animator.SetBool("SpecialAbility2SouthEast", false);
            animator.SetBool("SpecialAbility2SouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack();
        }
        
        public void TriggerCastSpellAnimation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isCastingSpell' to true to initiate the spell casting animation
            animator.SetBool("isCastingSpell", true);

            // Determine the current direction and trigger the appropriate cast spell animation
            if (animator.GetBool("isNorth")) animator.SetBool("CastSpellNorth", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("CastSpellSouth", true);
            else if (animator.GetBool("isEast")) animator.SetBool("CastSpellEast", true);
            else if (animator.GetBool("isWest")) animator.SetBool("CastSpellWest", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("CastSpellNorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("CastSpellNorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("CastSpellSouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("CastSpellSouthWest", true);

            // Reset the cast spell parameters after a delay or at the end of the animation
            StartCoroutine(ResetCastSpellParameters());
        }

        IEnumerator ResetCastSpellParameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.5f); // Adjust the wait time based on your animation length

            // Reset all cast spell parameters to false
            animator.SetBool("isCastingSpell", false);
            animator.SetBool("CastSpellNorth", false);
            animator.SetBool("CastSpellSouth", false);
            animator.SetBool("CastSpellEast", false);
            animator.SetBool("CastSpellWest", false);
            animator.SetBool("CastSpellNorthEast", false);
            animator.SetBool("CastSpellNorthWest", false);
            animator.SetBool("CastSpellSouthEast", false);
            animator.SetBool("CastSpellSouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack(); 
        }
        
        public void TriggerKickAnimation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isKicking' to true to initiate the kick animation
            animator.SetBool("isKicking", true);

            // Determine the current direction and trigger the appropriate kick animation
            if (animator.GetBool("isNorth")) animator.SetBool("KickNorth", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("KickSouth", true);
            else if (animator.GetBool("isEast")) animator.SetBool("KickEast", true);
            else if (animator.GetBool("isWest")) animator.SetBool("KickWest", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("KickNorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("KickNorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("KickSouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("KickSouthWest", true);

            // Reset the kick parameters after a delay or at the end of the animation
            StartCoroutine(ResetKickParameters());
        }

        IEnumerator ResetKickParameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.5f); // Adjust the wait time based on your animation length

            // Reset all kick parameters to false
            animator.SetBool("isKicking", false);
            animator.SetBool("KickNorth", false);
            animator.SetBool("KickSouth", false);
            animator.SetBool("KickEast", false);
            animator.SetBool("KickWest", false);
            animator.SetBool("KickNorthEast", false);
            animator.SetBool("KickNorthWest", false);
            animator.SetBool("KickSouthEast", false);
            animator.SetBool("KickSouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack(); 
        }
        
        public void TriggerFlipAnimation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isFlipping' to true to initiate the flip animation
            animator.SetBool("isFlipping", true);

            // Determine the current direction and trigger the appropriate flip animation
            if (animator.GetBool("isNorth")) animator.SetBool("FrontFlipNorth", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("FrontFlipSouth", true);
            else if (animator.GetBool("isEast")) animator.SetBool("FrontFlipEast", true);
            else if (animator.GetBool("isWest")) animator.SetBool("FrontFlipWest", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("FrontFlipNorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("FrontFlipNorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("FrontFlipSouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("FrontFlipSouthWest", true);

            // Reset the flip parameters after a delay or at the end of the animation
            StartCoroutine(ResetFlipParameters());
        }

        IEnumerator ResetFlipParameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.5f); // Adjust the wait time based on your animation length

            // Reset all flip parameters to false
            animator.SetBool("isFlipping", false);
            animator.SetBool("FrontFlipNorth", false);
            animator.SetBool("FrontFlipSouth", false);
            animator.SetBool("FrontFlipEast", false);
            animator.SetBool("FrontFlipWest", false);
            animator.SetBool("FrontFlipNorthEast", false);
            animator.SetBool("FrontFlipNorthWest", false);
            animator.SetBool("FrontFlipSouthEast", false);
            animator.SetBool("FrontFlipSouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack();  
        }
        
        public void TriggerRollAnimation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isRolling' to true to initiate the roll animation
            animator.SetBool("isRolling", true);

            // Determine the current direction and trigger the appropriate roll animation
            if (animator.GetBool("isNorth")) animator.SetBool("RollingNorth", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("RollingSouth", true);
            else if (animator.GetBool("isEast")) animator.SetBool("RollingEast", true);
            else if (animator.GetBool("isWest")) animator.SetBool("RollingWest", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("RollingNorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("RollingNorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("RollingSouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("RollingSouthWest", true);

            // Reset the roll parameters after a delay or at the end of the animation
            StartCoroutine(ResetRollParameters());
        }

        IEnumerator ResetRollParameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.5f); // Adjust the wait time based on your animation length

            // Reset all roll parameters to false
            animator.SetBool("isRolling", false);
            animator.SetBool("RollingNorth", false);
            animator.SetBool("RollingSouth", false);
            animator.SetBool("RollingEast", false);
            animator.SetBool("RollingWest", false);
            animator.SetBool("RollingNorthEast", false);
            animator.SetBool("RollingNorthWest", false);
            animator.SetBool("RollingSouthEast", false);
            animator.SetBool("RollingSouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack();  
        }
        
        public void TriggerSlideAnimation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isSliding' to true to initiate the slide animation
            animator.SetBool("isSliding", true);

            // Determine the current direction and trigger the appropriate slide animation
            if (animator.GetBool("isNorth")) animator.SetBool("SlidingNorth", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("SlidingSouth", true);
            else if (animator.GetBool("isEast")) animator.SetBool("SlidingEast", true);
            else if (animator.GetBool("isWest")) animator.SetBool("SlidingWest", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("SlidingNorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("SlidingNorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("SlidingSouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("SlidingSouthWest", true);

            // Reset the slide parameters after a delay or at the end of the animation
            StartCoroutine(ResetSlideParameters());
        }

        IEnumerator ResetSlideParameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.7f); // Adjust the wait time based on your animation length

            // Reset all slide parameters to false
            animator.SetBool("isSliding", false);
            animator.SetBool("SlidingNorth", false);
            animator.SetBool("SlidingSouth", false);
            animator.SetBool("SlidingEast", false);
            animator.SetBool("SlidingWest", false);
            animator.SetBool("SlidingNorthEast", false);
            animator.SetBool("SlidingNorthWest", false);
            animator.SetBool("SlidingSouthEast", false);
            animator.SetBool("SlidingSouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack(); 
        }
        
        public void TriggerPummelAnimation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isPummeling' to true to initiate the pummel animation
            animator.SetBool("isPummeling", true);

            // Determine the current direction and trigger the appropriate pummel animation
            if (animator.GetBool("isNorth")) animator.SetBool("PummelNorth", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("PummelSouth", true);
            else if (animator.GetBool("isEast")) animator.SetBool("PummelEast", true);
            else if (animator.GetBool("isWest")) animator.SetBool("PummelWest", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("PummelNorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("PummelNorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("PummelSouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("PummelSouthWest", true);

            // Reset the pummel parameters after a delay or at the end of the animation
            StartCoroutine(ResetPummelParameters());
        }

        IEnumerator ResetPummelParameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.5f); // Adjust the wait time based on your animation length

            // Reset all pummel parameters to false
            animator.SetBool("isPummeling", false);
            animator.SetBool("PummelNorth", false);
            animator.SetBool("PummelSouth", false);
            animator.SetBool("PummelEast", false);
            animator.SetBool("PummelWest", false);
            animator.SetBool("PummelNorthEast", false);
            animator.SetBool("PummelNorthWest", false);
            animator.SetBool("PummelSouthEast", false);
            animator.SetBool("PummelSouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack();  
        }
        
        public void TriggerAttackSpinAnimation()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            // Set 'isAttackSpinning' to true to initiate the attack spin animation
            animator.SetBool("isAttackSpinning", true);

            // Determine the current direction and trigger the appropriate attack spin animation
            if (animator.GetBool("isNorth")) animator.SetBool("AttackSpinNorth", true);
            else if (animator.GetBool("isSouth")) animator.SetBool("AttackSpinSouth", true);
            else if (animator.GetBool("isEast")) animator.SetBool("AttackSpinEast", true);
            else if (animator.GetBool("isWest")) animator.SetBool("AttackSpinWest", true);
            else if (animator.GetBool("isNorthEast")) animator.SetBool("AttackSpinNorthEast", true);
            else if (animator.GetBool("isNorthWest")) animator.SetBool("AttackSpinNorthWest", true);
            else if (animator.GetBool("isSouthEast")) animator.SetBool("AttackSpinSouthEast", true);
            else if (animator.GetBool("isSouthWest")) animator.SetBool("AttackSpinSouthWest", true);

            // Reset the attack spin parameters after a delay or at the end of the animation
            StartCoroutine(ResetAttackSpinParameters());
        }

        IEnumerator ResetAttackSpinParameters()
        {
            // Wait for the length of the animation before resetting
            yield return new WaitForSeconds(0.5f); // Adjust the wait time based on your animation length

            // Reset all attack spin parameters to false
            animator.SetBool("isAttackSpinning", false);
            animator.SetBool("AttackSpinNorth", false);
            animator.SetBool("AttackSpinSouth", false);
            animator.SetBool("AttackSpinEast", false);
            animator.SetBool("AttackSpinWest", false);
            animator.SetBool("AttackSpinNorthEast", false);
            animator.SetBool("AttackSpinNorthWest", false);
            animator.SetBool("AttackSpinSouthEast", false);
            animator.SetBool("AttackSpinSouthWest", false);

            // Optionally, restore the direction to ensure the character returns to the correct idle state
            RestoreDirectionAfterAttack(); 
        }
        

        #endregion
    }
}