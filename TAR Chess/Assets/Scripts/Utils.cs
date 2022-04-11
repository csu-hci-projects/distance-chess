using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static int numPiecesUpdated = 0;
    public static float moveSpeed = 10f;
    public static List<string> pins;
    public const string FILE = "ABCDEFGH";
    public const string RANK = "12345678";
    public static Vector3 NULL_COORDS = new Vector3(-1,-1,-1);
    public static Dictionary<bool, string> kingPosition = new Dictionary<bool, string>();

    public static string position(int file, int rank) {
        if(file < 0 || file > 7 || rank < 0 || rank > 7) return null;
        return FILE.Substring(file,1) + RANK.Substring(rank,1);
    }
    public static int file(string position) { return FILE.IndexOf(Char.ToUpper(position[0])); }
    public static int rank(string position) { return RANK.IndexOf(position[1]); }

    public static string positionFrom(string position, int fileDistance, int rankDistance) {
        int file = Utils.file(position), rank = Utils.rank(position);
        file += fileDistance; rank += rankDistance;
        return Utils.position(file, rank);
    }

    public static void pinPosition(string pinningPosition, string pinnedPosition) {
        pins.Add(pinningPosition + ">" + pinnedPosition);
    }
    public static void unpinPosition(string pinningPosition, string pinnedPosition) {
        pins.Remove(pinningPosition + ">" + pinnedPosition);
    }

    public static bool validUnderPin(string position, string move) {
        int numPins = 0; string pinningPosition = null;
        foreach(string pin in pins) {
            if(pin.EndsWith(position)) {
                ++numPins;
                if(numPins > 1) return false;
                pinningPosition = pin.Substring(0,2);
            }
        }
        if(pinningPosition is null) return true;
        return move.Equals(pinningPosition) || positionBetween(position, move, pinningPosition);
    }

    public static bool positionBetween(string posA, string posB, string posC) {
        return getPositionsBetween(posA,posC).Contains(posB);
    }

    public static bool sameFile(string positionA, string positionB) {
        return file(positionA) == file(positionB);
    }
    public static bool sameRank(string positionA, string positionB) {
        return rank(positionA) == rank(positionB);
    }
    public static bool diagonal(string positionA, string positionB) {
        return Math.Abs(file(positionA) - file(positionB)) == Math.Abs(rank(positionA) - rank(positionB));
    }

    public static List<string> getPositionsBetween(string positionA, string positionB) {
        List<string> positions = new List<string>();
        int fa=file(positionA), fb=file(positionB);
        int ra=rank(positionA), rb=rank(positionB);
        if(sameFile(positionA,positionB)) {
            int file = fa;
            int dir = (rb > ra)? 1:-1;
            for(int rank=ra+dir; rank<rb; rank += dir)
                positions.Add(position(file,rank));
        }
        if(sameRank(positionA,positionB)) {
            int rank = ra;
            int dir = (fb > fa)? 1:-1;
            for(int file=fa+dir; file<fb; file += dir)
                positions.Add(position(file,rank));
        }
        if(diagonal(positionA,positionB)) {
            int fdir = (fb > fa)? 1:-1;
            int rdir = (rb > ra)? 1:-1;
            for(int file=fa+fdir, rank=ra+rdir; file<fb; ) {
                positions.Add(position(file,rank));
                file += fdir;
                rank += rdir;
            }
        }
        return positions;
    }

    public static Vector3 getLocalCoordsFromPosition(String position) {
        if(position is null || position.Length != 2) return NULL_COORDS;
        float f=file(position), r=rank(position);
        return new Vector3(f, 0, r);
    }
    public static bool pieceAt(Transform transform, String position) {
        if(position is null || position.Length != 2) return false;
        if(_MOVE_position is null) {
            _MOVE_position = position;
            _MOVE_coords = getLocalCoordsFromPosition(position);
        }
        if(Vector3.Distance(transform.localPosition, _MOVE_coords) < 0.001f) {
            _MOVE_position = null;
            return true;
        }
        return false;
    }
    public static Vector3 moveTowards(Transform transform, String position) {
        if(position is null || position.Length != 2) return transform.localPosition;
        if(_MOVE_position is null) {
            _MOVE_position = position;
            _MOVE_coords = getLocalCoordsFromPosition(position);
        }
        return Vector3.MoveTowards(
            transform.localPosition,
            _MOVE_coords,
            moveSpeed * Time.deltaTime
        );
    }
        private static String _MOVE_position = null;
        private static Vector3 _MOVE_coords;
}
