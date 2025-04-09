using System.Collections.Generic;
using UnityEngine;

public class CommandInput : MonoBehaviour
{
    private List<string> inputBuffer = new List<string>(); // 입력 버퍼
    private float inputTimeLimit = 1.0f;
    private float lastInputTime;

    // 부드러운 커맨드 인식: 각 단계마다 여러 입력 허용
    private readonly List<(string name, List<HashSet<string>> steps)> skillCommands = new List<(string, List<HashSet<string>>)> {
        ("66", new List<HashSet<string>> {
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),
        ("22", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "K" }
        }),
        ("623", new List<HashSet<string>> {
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "K" }
        }),
        ("236", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),
        ("41236", new List<HashSet<string>> {
            new HashSet<string>{ "Left", "Down-Left" },
            new HashSet<string>{ "Down", "Down-Left", "Down-Right" },
            new HashSet<string>{ "Right", "Down-Right" },
            new HashSet<string>{ "K" }
        }),
        ("4홀드6", new List<HashSet<string>> {
            new HashSet<string>{ "Left_Hold" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
            // 홀드 미구현
        }),
        ("236236", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
            // 아직 입력 안됨
        })
    };

    void Update()
    {
        DetectInput();
        MatchCommand();
    }

    void DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.D)) RegisterInput("Right");
        if (Input.GetKeyDown(KeyCode.A)) RegisterInput("Left");
        if (Input.GetKeyDown(KeyCode.W)) RegisterInput("Up");
        if (Input.GetKeyDown(KeyCode.S)) RegisterInput("Down");
        if (Input.GetKeyDown(KeyCode.K)) RegisterInput("K");

        // 대각선 감지
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) RegisterInput("Down-Right");
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S)) RegisterInput("Down-Left");
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
