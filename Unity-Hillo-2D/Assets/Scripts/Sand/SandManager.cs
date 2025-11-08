using System;
using NUnit.Framework.Constraints;
using UnityEngine;

public class SandManager : MonoBehaviour
{
    public bool AllSandGoneThrough;
    int howManyGoneThrough;
    public event Action OnAllSandWentThrough;

    void Start()
    {
        Hourglass.Instance.FinishedRotating += HourglassRotated;
    }

    private void HourglassRotated()
    {
        AllSandGoneThrough = true;
        howManyGoneThrough = 0;
    }

    public void SandWentThrough()
    {
        howManyGoneThrough++;
        if (howManyGoneThrough == transform.childCount)
        {
            AllSandGoneThrough = true;
            OnAllSandWentThrough?.Invoke();
        }
    }
}
