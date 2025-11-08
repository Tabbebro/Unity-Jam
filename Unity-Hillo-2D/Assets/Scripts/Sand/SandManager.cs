using System;
using NUnit.Framework.Constraints;
using UnityEngine;

public class SandManager : MonoBehaviour
{
    public bool AllSandGoneThrough;
    public int howManyGoneThrough;
    public event Action OnAllSandWentThrough;

    void Start()
    {

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
