using System.Collections.Generic;
using UnityEngine;

public class CommandInput : MonoBehaviour
{
    private List<string> inputBuffer = new List<string>(); // 입력 버퍼
    private float inputTimeLimit = 1.0f;
    private float lastInputTime;
    
    float leftHoldTime = 0f;
    float rightHoldTime = 0f;
    float holdThreshold = 0.5f;
    
    bool leftHoldTriggered = false;
    bool rightHoldTriggered = false;

    private HashSet<string> frameInputs = new HashSet<string>();
    
    // 부드러운 커맨드 인식: 각 단계마다 여러 입력 허용
    private readonly List<(string name, List<HashSet<string>> steps)> skillCommands = new List<(string, List<HashSet<string>>)>
    {
        ("→ → K", new List<HashSet<string>> {
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),
    
        ("← ← K", new List<HashSet<string>> {
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "K" }
        }),
        
        ("↓ ↓ K", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "K" }
        }),
        
        ("→ ↓ ↘ K", new List<HashSet<string>> {
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "K" }
        }),
        
        ("← ↓ ↙ K", new List<HashSet<string>> {
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Left", "Left", "Down" },
            new HashSet<string>{ "K" }
        }),
        
        ("↓ ↘ → K", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),
    
        ("↓ ↙ ← K", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Left", "Left", "Down" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "K" }
        }),
        
        ("← ↓ → K", new List<HashSet<string>> {
            new HashSet<string>{ "Left", "Down-Left" },
            new HashSet<string>{ "Down", "Down-Left", "Down-Right" },
            new HashSet<string>{ "Right", "Down-Right" },
            new HashSet<string>{ "K" }
        }),
        
        ("→ ↓ ← K", new List<HashSet<string>> {
            new HashSet<string>{ "Right", "Down-Right" },
            new HashSet<string>{ "Down", "Down-Left", "Down-Right" },
            new HashSet<string>{ "Left", "Down-Left" },
            new HashSet<string>{ "K" }
        }),
        
        ("←(Hold) → K", new List<HashSet<string>> {
            new HashSet<string>{ "Left_Hold" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),
    
        ("→(Hold) ← K", new List<HashSet<string>> {
            new HashSet<string>{ "Right_Hold" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "K" }
        }),
        
        ("↓ ↘ → ↓ ↘ → K", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),
    
        ("↓ ↙ ← ↓ ↙ ← K", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Left", "Left", "Down" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Left", "Left", "Down" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "K" }
        }),
    };


    void Update()
    {
        frameInputs.Clear();
        DetectInput();
        DetectHold();
        MatchCommand();
    }

    void DetectInput()
    {
        // 키 다운 기반으로만 감지
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
        // 왼쪽 홀드 감지
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

        // 오른쪽 홀드 감지
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

        inputBuffer.Add(input);
        lastInputTime = Time.time;
        Debug.Log($"입력: {input} / 버퍼: {string.Join(", ", inputBuffer)}");
    }

    void MatchCommand()
    {
        foreach (var (name, steps) in skillCommands)
        {
            if (IsCommandMatch(steps))
            {
                Debug.Log($"✅ 커맨드 발동됨: {name}");
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
}
