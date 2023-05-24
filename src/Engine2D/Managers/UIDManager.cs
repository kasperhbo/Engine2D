namespace Engine2D.Managers;

public static class UIDManager
{
    private static Int64 s_uidCounter = 0;

    public static List<Int64> TakenUids = new List<Int64>();
    
    
    public static Int64 GetNewUID()
    {
        Int64 uid = s_uidCounter;
        
        if (TakenUids.Contains(uid))
            return GetNewUID();
        
        s_uidCounter++;
        return uid;
    }
}