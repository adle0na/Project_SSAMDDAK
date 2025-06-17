using System;
using UnityEngine;

public class IngameUIController : MonoBehaviour
{
    [SerializeField] private GameObject characterUIList;
    private bool isOn;

    private void Start()
    {
        characterUIList.SetActive(false);
        isOn = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOn = !isOn;
            characterUIList.SetActive(isOn);
        }
    }
}
