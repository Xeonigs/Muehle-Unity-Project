using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameMenu : MonoBehaviour
{
    public GameObject gameMenu;
    public void ToggleMenu()
    {
        gameMenu.SetActive(!gameMenu.activeSelf);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleMenu();
    }
}
