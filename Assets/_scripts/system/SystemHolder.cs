public class SystemHolder : PersistentSingleton<SystemHolder>
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }
}
