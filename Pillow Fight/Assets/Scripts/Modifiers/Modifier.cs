using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Modifier : MonoBehaviour
{
    protected abstract void Start();

    public abstract void OnRoundStart();

    public abstract void OnRoundEnd();

    protected abstract void OnDestroy();

    public abstract string GetName();

    public abstract int GetID();

    public abstract List<int> GetFilteredMods();
}
