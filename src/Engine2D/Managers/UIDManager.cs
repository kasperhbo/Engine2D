namespace Engine2D.Managers;

public static class UIDManager
{
    private static int s_uidCounter = 0;

    public static List<int> TakenUids = new List<int>();
    
    
    public static int GetNewUID()
    {
        int uid = s_uidCounter;
        
        if (TakenUids.Contains(uid))
            return GetNewUID();
        
        s_uidCounter++;
        return uid;
    }
}