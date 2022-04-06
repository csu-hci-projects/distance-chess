using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public const string FILE = "ABCDEFGH";
    public const string RANK = "12345678";
    public static string position(int file, int rank) {
        if(file < 0 || file > 7 || rank < 0 || rank > 7) return null;
        return FILE.Substring(file,1) + RANK.Substring(rank,1);
    }
    public static int file(string position) { return FILE.IndexOf(position[0]); }
    public static int rank(string position) { return RANK.IndexOf(position[1]); }

    public static string positionFrom(string position, int fileDistance, int rankDistance) {
        int file = Utils.file(position), rank = Utils.rank(position);
        file += fileDistance; rank += rankDistance;
        return Utils.position(file, rank);
    }
}
