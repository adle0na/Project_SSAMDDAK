using System.Collections.Generic;
using UnityEngine;

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
}
