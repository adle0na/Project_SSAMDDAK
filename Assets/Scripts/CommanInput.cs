using System.Collections.Generic;
using SmallScaleInc.TopDownPixelCharactersPack1;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class CommandInput : MonoBehaviour
{
    protected List<string> inputBuffer = new List<string>();
    protected float inputTimeLimit = 1.0f;
    protected float lastInputTime;

    float leftHoldTime = 0f;
    float rightHoldTime = 0f;
    float holdThreshold = 0.5f;

    bool leftHoldTriggered = false;
    bool rightHoldTriggered = false;

    private HashSet<string> frameInputs = new HashSet<string>();

    protected abstract List<(string name, List<HashSet<string>> steps)> SkillCommands { get; }

    protected virtual void Update()
    {
        frameInputs.Clear();
        DetectInput();
        DetectHold();
        MatchCommand();
        UpdateBuffs();
    }

    void DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.J)) HandleNormalAttack();
        if (Input.GetKeyDown(KeyCode.K)) RegisterInputOnce("K");

        bool d = Input.GetKey(KeyCode.D);
        bool a = Input.GetKey(KeyCode.A);
        bool s = Input.GetKey(KeyCode.S);

        if (Input.GetKeyDown(KeyCode.D) && s) RegisterInputOnce("Down-Right");
        else if (Input.GetKeyDown(KeyCode.A) && s) RegisterInputOnce("Down-Left");
        else
        {
            if (Input.GetKeyDown(KeyCode.D)) RegisterInputOnce("Right");
            if (Input.GetKeyDown(KeyCode.A)) RegisterInputOnce("Left");
            if (Input.GetKeyDown(KeyCode.S)) RegisterInputOnce("Down");
            if (Input.GetKeyDown(KeyCode.W)) RegisterInputOnce("Up");
        }
    }

    void DetectHold()
    {
        if (Input.GetKey(KeyCode.A))
        {
            leftHoldTime += Time.deltaTime;
            if (!leftHoldTriggered && leftHoldTime >= holdThreshold)
            {
                RegisterInputOnce("Left_Hold");
                leftHoldTriggered = true;
            }
        }
        else
        {
            leftHoldTime = 0f;
            leftHoldTriggered = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rightHoldTime += Time.deltaTime;
            if (!rightHoldTriggered && rightHoldTime >= holdThreshold)
            {
                RegisterInputOnce("Right_Hold");
                rightHoldTriggered = true;
            }
        }
        else
        {
            rightHoldTime = 0f;
            rightHoldTriggered = false;
        }
    }

    void RegisterInputOnce(string input)
    {
        if (frameInputs.Contains(input)) return;
        frameInputs.Add(input);
        RegisterInput(input);
    }

    void RegisterInput(string input)
    {
        if (Time.time - lastInputTime > inputTimeLimit)
        {
            inputBuffer.Clear();
        }

        if (input == "Left_Hold" && inputBuffer.Contains("Right_Hold")) return;
        if (input == "Right_Hold" && inputBuffer.Contains("Left_Hold")) return;
        
        inputBuffer.Add(input);
        lastInputTime = Time.time;
        Debug.Log($"입력: {input} / 버퍼: {string.Join(", ", inputBuffer)}");
    }

    void MatchCommand()
    {
        foreach (var (name, steps) in SkillCommands)
        {
            if (IsCommandMatch(steps))
            {
                if (passiveActive)
                {
                    Debug.Log($"✅ 강화 커맨드 발동됨: {name}");
                    passiveActive = false;
                }
                else
                {
                    Debug.Log($"✅ 커맨드 발동됨: {name}");
                }
                inputBuffer.Clear();
                break;
            }
        }
    }

    bool IsCommandMatch(List<HashSet<string>> commandSteps)
    {
        if (inputBuffer.Count < commandSteps.Count) return false;

        int startIdx = inputBuffer.Count - commandSteps.Count;

        for (int i = 0; i < commandSteps.Count; i++)
        {
            if (!commandSteps[i].Contains(inputBuffer[startIdx + i]))
                return false;
        }
        return true;
    }

    // ====== 평타 & 패시브 ======
    int normalAttackCount = 0;
    float passiveTimer = 0f;
    float passiveDuration = 0.5f;
    bool passiveActive = false;

    void HandleNormalAttack()
    {
        normalAttackCount++;
        if (normalAttackCount >= 3)
        {
            passiveActive = true;
            passiveTimer = passiveDuration;
            normalAttackCount = 0;
            Debug.Log("🔷 평타 3타 완료! 다음 스킬 강화됨");
        }
    }

    void UpdateBuffs()
    {
        if (passiveActive)
        {
            passiveTimer -= Time.deltaTime;
            if (passiveTimer <= 0f)
            {
                passiveActive = false;
                Debug.Log("❌ 강화 효과 종료");
            }
        }
    }
    
        
    // void NormalAttack()
    // {
    //     Vector2 fireDirection = new Vector2(Mathf.Cos(lastAngle * Mathf.Deg2Rad), Mathf.Sin(lastAngle * Mathf.Deg2Rad));
    //     NormalAttackEffect(fireDirection);
    // }
    //
    // void NormalAttackEffect(Vector2 direction)
    // {
    //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //     GameObject projectileInstance = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
    //     Rigidbody2D rbProjectile = projectileInstance.GetComponent<Rigidbody2D>();
    //     if (rbProjectile != null)
    //     {
    //         rbProjectile.linearVelocity = direction * projectileSpeed;
    //     }
    // }
    
    // IEnumerator Quickshot()
    // {
    //     // Initial small delay before starting the quickshot sequence
    //     yield return new WaitForSeconds(0.1f);
    //
    //     // Loop to fire five projectiles in the facing direction
    //     for (int i = 0; i < 5; i++)
    //     {
    //         Vector2 fireDirection = new Vector2(Mathf.Cos(lastAngle * Mathf.Deg2Rad), Mathf.Sin(lastAngle * Mathf.Deg2Rad));
    //         ShootProjectile(fireDirection);
    //
    //         // Wait for 0.18 seconds before firing the next projectile
    //         yield return new WaitForSeconds(0.18f);
    //     }
    // }
    //     
    // IEnumerator CircleShot()
    // {
    //     float initialDelay = 0.1f;
    //     float timeBetweenShots = 0.9f / 8;  // Total time divided by the number of shots
    //
    //     yield return new WaitForSeconds(initialDelay);
    //
    //     // Use the lastAngle as the start angle and generate projectiles in 8 directions
    //     for (int i = 0; i < 8; i++)
    //     {
    //         float angle = lastAngle + i * 45;  // Increment by 45 degrees for each direction
    //         angle = Mathf.Deg2Rad * angle;  // Convert to radians for direction calculation
    //         Vector2 fireDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    //         ShootProjectile(fireDirection);
    //
    //         yield return new WaitForSeconds(timeBetweenShots);
    //     }
    // }
    //
    // IEnumerator DeployAoEDelayed()
    // {
    //     if (AoEPrefab != null)
    //     {
    //         // Wait for 0.3 seconds before instantiating the AoEPrefab
    //         yield return new WaitForSeconds(0.3f);
    //
    //         // Instantiate the AoE prefab at the player's position
    //         GameObject aoeInstance = Instantiate(AoEPrefab, transform.position, Quaternion.identity);
    //
    //         // Destroy the instantiated prefab after another 0.5 seconds
    //         Destroy(aoeInstance, 0.5f);
    //     }
    // }
    
    // IEnumerator ComboRoutine()
    // {
    //     while (comboStep <= maxCombo)
    //     {
    //         // 1) 애니메이션 트리거
    //         string dir   = dirCtrl.GetCurrentDirection();
    //         anim.SetTrigger($"Attack{comboStep}_{dir}");
    //
    //         // 2) 전진
    //         Vector2 v    = DirectionToVector(dir);
    //         rb.MovePosition(rb.position + v * lungeDistance);
    //
    //         // 3) 입력 대기
    //         queueNext = false;
    //         float t = 0f;
    //         while (t < comboInputWindow)
    //         {
    //             if (queueNext && comboStep < maxCombo) break;
    //             t += Time.deltaTime;
    //             yield return null;
    //         }
    //
    //         if (queueNext && comboStep < maxCombo)
    //         {
    //             comboStep++;
    //             continue;
    //         }
    //         break;          // 콤보 종료
    //     }
    //
    //     combatCo = null;
    // }
    
}
