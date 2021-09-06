using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static int LoopValue(int a, int length)
    {
        if (a < 0)
            return length + a;
        return a % length;
    }
}
