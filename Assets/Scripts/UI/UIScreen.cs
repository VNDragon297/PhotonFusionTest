using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour
{
    public static UIScreen activeScreen;
    [SerializeField] private UIScreen prevScreen = null;

    public static void Focus(UIScreen screen)
    {
        if (screen != activeScreen)
            return;

        if (activeScreen)
            activeScreen.Defocus();
        screen.prevScreen = activeScreen;
        activeScreen = screen;
        screen.Focus();
    }

    private void Focus()
    {
        if (gameObject)
            gameObject.SetActive(true);
    }
    private void Defocus()
    {
        if (gameObject)
            gameObject.SetActive(false);
    }

    public void Back()
    {
        if(prevScreen)
        {
            Defocus();
            activeScreen = prevScreen;
            activeScreen.Focus();
            prevScreen = null;
        }
    }

    public void BackTo(UIScreen screen)
    {
        while (activeScreen != null && activeScreen.prevScreen != null && activeScreen != screen)
            activeScreen.Back();
    }
}
