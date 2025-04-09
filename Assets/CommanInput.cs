using System.Collections.Generic;
using UnityEngine;

public class CommandInput : MonoBehaviour
{
    private List<string> inputBuffer = new List<string>(); // 입력 버퍼
    private float inputTimeLimit = 1.0f;
    private float lastInputTime;
    private Vector2 characterPosition = Vector2.zero;
    private float moveSpeed = 5.0f;

    // 부드러운 커맨드 인식: 각 단계마다 여러 입력 허용
    private readonly List<HashSet<string>> superCommand = new List<HashSet<string>> {
        new HashSet<string>{ "Left", "Down-Left" },
        new HashSet<string>{ "Down", "Down-Left", "Down-Right" },
        new HashSet<string>{ "Right", "Down-Right" },
        new HashSet<string>{ "K" }
    };

    void Update()
    {
        DetectInput();
        CheckCommand();
        MoveCharacter();
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

    void CheckCommand()
    {
        if (MatchFlexibleCommand(superCommand))
        {
            ExecuteSuperCommand();
            inputBuffer.Clear();
        }
    }

    // 부드러운 커맨드 매칭 (단계별로 포함만 확인)
    bool MatchFlexibleCommand(List<HashSet<string>> command)
    {
        int bufferIndex = 0;

        foreach (var step in command)
        {
            bool matched = false;

            while (bufferIndex < inputBuffer.Count)
            {
                if (step.Contains(inputBuffer[bufferIndex]))
                {
                    matched = true;
                    bufferIndex++;
                    break;
                }
                bufferIndex++;
            }

            if (!matched) return false;
        }

        return true;
    }

    void ExecuteSuperCommand()
    {
        Debug.Log("⬅️⬇️➡️ 슈퍼 커맨드 실행! (Smooth Input)");
    }

    void MoveCharacter()
    {
        Vector2 moveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.D)) moveDirection.x += 1;
        if (Input.GetKey(KeyCode.A)) moveDirection.x -= 1;
        if (Input.GetKey(KeyCode.W)) moveDirection.y += 1;
        if (Input.GetKey(KeyCode.S)) moveDirection.y -= 1;

        characterPosition += moveDirection * moveSpeed * Time.deltaTime;
    }
}
