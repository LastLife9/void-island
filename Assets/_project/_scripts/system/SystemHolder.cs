using UnityEngine;

public class SystemHolder : PersistentSingleton<SystemHolder>
{
    protected override void Awake()
    {
        base.Awake();
        InitializeSystem();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    public void InitializeSystem()
    {

    }
}
