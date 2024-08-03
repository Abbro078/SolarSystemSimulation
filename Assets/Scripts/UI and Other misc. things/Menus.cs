using UnityEngine;

public class Menus : MonoBehaviour
{
    public GameObject OnScreen;
    public GameObject Instructions;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleObjects();
        }
    }

    void ToggleObjects()
    {
        bool isActive1 = OnScreen.activeSelf;
        bool isActive2 = Instructions.activeSelf;

        OnScreen.SetActive(!isActive1);
        Instructions.SetActive(!isActive2);
    }
}