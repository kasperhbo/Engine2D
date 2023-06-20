namespace Engine2D.Managers;

public static class UIDManager
{
    private static int s_uidcounter;
    public static List<int> TakenUIDS = new();

    public static void AddUID(int uid)
    {
        TakenUIDS.Add(uid);
    }
    
    public static int GetUID()
    {
        int uid = s_uidcounter;
        s_uidcounter++;

        //Recursive call
        if (TakenUIDS.Contains(uid))
            return GetUID();

        TakenUIDS.Add(uid);
        return uid;
    }
}