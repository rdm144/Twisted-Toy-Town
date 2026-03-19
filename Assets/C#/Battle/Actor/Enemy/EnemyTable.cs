using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    GrabbyHands, Shield_Enemy
}

public class EnemyTable
{
    public static Dictionary<EnemyType, string> table = new()
    {
        {EnemyType.GrabbyHands, "GrabbyHands" },
        {EnemyType.Shield_Enemy, "Shield Enemy" }
    };
}
