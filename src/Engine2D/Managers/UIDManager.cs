using Engine2D.Logging;

namespace Engine2D.Managers;

public static class UIDManager
{
    private static int s_uidcounter = 0;
    private static List<int> s_takenUids = new();

    public static int GetUID()
    {
        int uid = s_uidcounter;
        s_uidcounter++;

        //Recursive call
        if (s_takenUids.Contains(uid))
            GetUID();

        s_takenUids.Add(uid);
        Log.Message("uid: " + uid);
        return uid;
    }
}