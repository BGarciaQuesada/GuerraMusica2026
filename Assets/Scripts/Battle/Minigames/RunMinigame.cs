using System;
using UnityEngine;

public class RunMinigame : MonoBehaviour
{
    public int pressesNeeded = 15;

    int presses;

    public event Action<bool> OnFinished;

    public void StartRun()
    {
        presses = 0;
        gameObject.SetActive(true);
    }

    public void RegisterPress()
    {
        presses++;

        if (presses >= pressesNeeded)
        {
            Finish(true);
        }
    }

    void Finish(bool success)
    {
        gameObject.SetActive(false);
        OnFinished?.Invoke(success);
    }
}
