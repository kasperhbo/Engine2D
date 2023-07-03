namespace Engine2D.Managers;

internal static class UIDManager
{
    private static int s_uidcounter;
    internal static List<int> TakenUIDS = new();

    internal static void AddUID(int uid)
    {
        TakenUIDS.Add(uid);
    }

    internal static int GetUID()
    {
        var uid = s_uidcounter;
        
        s_uidcounter++;
        //Recursive call
        if (TakenUIDS.Contains(uid))
        {
            
            return GetUID();
        }
 

        TakenUIDS.Add(uid);
        return uid;
    }
}