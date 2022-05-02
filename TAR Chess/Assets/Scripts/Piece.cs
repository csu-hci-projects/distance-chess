using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public Game game;
    public enum PieceColor { white, black };
    public enum PieceType {
        pawn, rook, knight, bishop, queen, king
    };

    public PieceType type;
    public PieceColor color;

    public string position = "a1";

    private const string FILE = "abcdefgh";
    private const string RANK = "12345678";

    void Start() {
        reposition(position);
    }

    void Update() {
        
    }

    public int file() {
        string f = position.Substring(0,1);
        if(FILE.Contains(f))
            return FILE.IndexOf(f);
        else
            return -1;
    }
    public int rank() {
        string r = position.Substring(1);
        if(RANK.Contains(r))
            return RANK.IndexOf(r);
        else
            return -1;
    }

    public bool reposition(string newPosition) {
        if(validPosition(newPosition))
            position = newPosition;
        else
            return false;

        return reposition(localCoordsFrom(newPosition));
    }
    public bool reposition(Vector3 coords) {
        if(coords.Equals(Vector3.down))
            return false;
        
        transform.localPosition = coords;

        return true;
    }
    public bool objectIsAt(string position) {
        return objectIsAt(localCoordsFrom(position));
    }
    public bool objectIsAt(Vector3 coords) {
        if(Vector3.Distance(transform.localPosition, coords) < 0.001f)
            return reposition(coords);
        else
            return false;
    }

    public static bool validPosition(string position) {
        if(position is null || position.Length != 2)
            return false;
        if(FILE.IndexOf(position[0]) == -1)
            return false;
        if(RANK.IndexOf(position[1]) == -1)
            return false;
        return true;
    }
    public static Vector3 localCoordsFrom(string position) {
        Vector3 coords = Vector3.down;
        if(validPosition(position)) {
            coords.x = (float) file(position);
            coords.y = 0f;
            coords.z = (float) rank(position);
        }
        return coords;
    }
    public static int file(string position) {
        if(!validPosition(position)) return -1;
        else return FILE.IndexOf(position[0]);
    }
    public static int rank(string position) {
        if(!validPosition(position)) return -1;
        else return RANK.IndexOf(position[1]);
    }
}
