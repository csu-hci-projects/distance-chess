using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public Game game;
    public enum PieceColor { white = 1, black = -1 };
    public enum PieceType {
        pawn, rook, knight, bishop, queen, king
    };

    public PieceType type;
    public PieceColor color;

    public string position = "a1";

    private const string FILE = "abcdefgh";
    private const string RANK = "12345678";
    public List<string> possibleMoves = new List<string>();
    protected bool updated = false;

    protected virtual void Start() {
        if(color == game.playerColor) {
            game.kill(position);
            gameObject.SetActive(false);
        } else {
            game.addPiece(this);
        }
    }

    public virtual void updatePossibleMoves() {
        Debug.Log("Piece updating.");
        possibleMoves.Clear();
    }
    public int file() => Piece.file(position);
    public int rank() => Piece.rank(position);

    public virtual bool validMove(string move) => validMove(Piece.file(move), Piece.rank(move));
    public virtual bool validMove(int file, int rank) {
        return false;
    }

    private float _moveSpeed = -1f;
    private Vector3 _moveCoords = Vector3.down;
    public bool glideTo(string position) {
        if(!validPosition(position))
            return false;
        if(_moveCoords.Equals(Vector3.down))
            _moveCoords = localCoordsFrom(position);
        return glideTo(_moveCoords);
    }
    public bool glideTo(Vector3 coords) {
        if(_moveSpeed == -1f) {
            _moveSpeed = Vector3.Distance(transform.localPosition, coords) / Game.moveTime;
        }

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, coords, _moveSpeed*Time.deltaTime);
        if(objectIsAt(coords)) {
            _moveSpeed = -1f;
            _moveCoords = Vector3.down;
            return true;
        }

        return false;
    }

    public string positionFromPiece(int dFile, int dRank) {
        int f = file() + dFile;
        int r = rank() + dRank;
        if(!(f>=0 && f<=7)) return null;
        if(!(r>=0 && r<=7)) return null;
        
        return FILE.Substring(f,1) + RANK.Substring(r,1);
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
    public static string Position(int file, int rank) {
        if(file<0 || file>7 || rank<0 || rank>7)
            return null;
        else return
            FILE.Substring(file,1) + RANK.Substring(rank,1);
    }
}
