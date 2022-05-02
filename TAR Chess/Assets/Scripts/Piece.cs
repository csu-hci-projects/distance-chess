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

    public void die() {
        gameObject.SetActive(false);
    }

    public virtual void updatePossibleMoves() {
        possibleMoves.Clear();
    }
    public int file() => Utils.file(position);
    public int rank() => Utils.rank(position);

    public virtual bool validMove(string move) => validMove(Utils.file(move), Utils.rank(move));
    public virtual bool validMove(int file, int rank) {
        return false;
    }

    private float _moveSpeed = -1f;
    private Vector3 _moveCoords = Vector3.down;
    public bool glideTo(string position) {
        if(!Utils.validPosition(position))
            return false;
        if(_moveCoords.Equals(Vector3.down))
            _moveCoords = Utils.coords(position);
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

    public string positionFromPiece(int dFile, int dRank) =>
        Utils.position(file() + dFile, rank() + dRank);

    public bool reposition(string newPosition) {
        if(Utils.validPosition(newPosition))
            position = newPosition;
        else
            return false;

        return reposition(Utils.coords(newPosition));
    }
    public bool reposition(Vector3 coords) {
        if(coords.Equals(Vector3.down))
            return false;
        
        transform.localPosition = coords;

        return true;
    }
    public bool objectIsAt(string position) => objectIsAt(Utils.coords(position));
    public bool objectIsAt(Vector3 coords) {
        if(Vector3.Distance(transform.localPosition, coords) < 0.001f)
            return reposition(coords);
        else
            return false;
    }
}
