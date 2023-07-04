namespace Engine2D.Managers;

internal static class UIDManager
{
    private static int _sUidcounter;
    public static readonly List<int> TakenUids = new();

    internal static int GetUID()
    {
        _sUidcounter++;
        //Recursive call
        if (TakenUids.Contains(_sUidcounter))
        {
            return GetUID();
        }
 
        TakenUids.Add(_sUidcounter);
        return _sUidcounter;
    }
}