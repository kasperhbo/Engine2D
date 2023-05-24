using Engine2D.Logging;

namespace Engine2D.Managers;

public static class UIDManager
{
    private static int s_uidcounter = 0;
    public static List<int> TakenUIDS = new();

    public static int GetUID()
    {
        int uid = s_uidcounter;
        s_uidcounter++;

        //Recursive call
        if (TakenUIDS.Contains(uid))
            GetUID();

        TakenUIDS.Add(uid);
        Log.Message("uid: " + uid);
        return uid;
    }
}