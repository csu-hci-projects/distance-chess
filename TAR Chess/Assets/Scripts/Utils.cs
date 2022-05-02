using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour {
    public const string FILE = "abcdefgh";
    public const string RANK = "12345678";
    
    public static bool validPosition(string position) => file(position)!=-1 && rank(position)!=-1;
    public static string position(int file, int rank) {
        if(file<0 || file>7 || rank<0 || rank>7)
            return null;
        return "" + FILE[file] + RANK[rank];
    }
    public static int file(string position) {
        if(position is null || position.Length != 2)
            return -1;
        return FILE.IndexOf(position[0]);
    }
    public static int rank(string position) {
        if(position is null || position.Length != 2)
            return -1;
        return RANK.IndexOf(position[1]);
    }
    public static Vector3 coords(string position) {
        float x=file(position), z=rank(position);
        if(x == -1f || z == -1f)
            return Vector3.down;
        else
            return new Vector3(x, 0, z);
    }
}
