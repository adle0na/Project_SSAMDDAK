using System.Collections.Generic;
using UnityEngine;

public class CommandInput : MonoBehaviour
{
    private List<string> inputBuffer = new List<string>(); // 입력 버퍼
    private float inputTimeLimit = 1.0f; // 입력 유지 시간 (초)
    private float lastInputTime;
    private Vector2 characterPosition = Vector2.zero; // 캐릭터 위치
    private float moveSpeed = 5.0f; // 이동 속도

    // 커맨드 정의 (➡️⬇️↘️K)
    private readonly List<string> hadoukenCommand = new List<string> { "Right", "Down", "Down-Right", "K" };

    void Update()
    {
        DetectInput();
        CheckCommand();
        MoveCharacter();
    }

    void DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) RegisterInput("Right");
        if (Input.GetKeyDown(KeyCode.LeftArrow)) RegisterInput("Left");
        if (Input.GetKeyDown(KeyCode.UpArrow)) RegisterInput("Up");
        if (Input.GetKeyDown(KeyCode.DownArrow)) RegisterInput("Down");
        if (Input.GetKeyDown(KeyCode.K)) RegisterInput("K"); // 공격 버튼 K 등록
        
        // 대각선 ↘️ 입력 감지 (오른쪽 + 아래 동시 입력 시 Down-Right 등록)
        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.DownArrow))
        {
            RegisterInput("Down-Right");
        }
    }

    void RegisterInput(string input)
    {
        if (Time.time - lastInputTime > inputTimeLimit)
        {
            inputBuffer.Clear(); // 입력 시간이 초과되면 초기화
        }
        
        inputBuffer.Add(input);
        lastInputTime = Time.time;
    }

    void CheckCommand()
    {
        if (inputBuffer.Count >= hadoukenCommand.Count)
        {
            // 입력 버퍼의 마지막 N개가 커맨드와 일치하는지 확인
            int startIndex = inputBuffer.Count - hadoukenCommand.Count;
            bool match = true;
            for (int i = 0; i < hadoukenCommand.Count; i++)
            {
                if (inputBuffer[startIndex + i] != hadoukenCommand[i])
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                ExecuteCommand();
                inputBuffer.Clear(); // 커맨드 입력 후 버퍼 초기화
            }
        }
    }

    void ExecuteCommand()
    {
        Debug.Log("➡️⬇️↘️K 커맨드 실행! (Hadouken!)");
        // 여기에 실제 캐릭터 공격 동작을 넣으면 됨.
    }

    void MoveCharacter()
    {
        Vector2 moveDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.RightArrow)) moveDirection.x += 1;
        if (Input.GetKey(KeyCode.LeftArrow)) moveDirection.x -= 1;
        if (Input.GetKey(KeyCode.UpArrow)) moveDirection.y += 1;
        if (Input.GetKey(KeyCode.DownArrow)) moveDirection.y -= 1;

        characterPosition += moveDirection * moveSpeed * Time.deltaTime;
        Debug.Log($"캐릭터 위치: X = {characterPosition.x}, Y = {characterPosition.y}");
    }
}
