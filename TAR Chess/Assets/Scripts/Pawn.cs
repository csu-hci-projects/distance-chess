using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {
    public Board board;
    public bool white, alive;
    public string position;
    public string[] possibleMoves;

    void Start() {
        possibleMoves = new string[4];
        float init_x = (float) Utils.file(position);
        float init_z = (float) Utils.rank(position);
        transform.localPosition = new Vector3(init_x, 0, init_z);
        board.put((white? "w":"b") + "p", position);
        updatePossibleMoves();
    }

    void Update() {
        
    }

    string pawnForwardMove(bool fast = false) {
        return Utils.positionFrom(position, 0, (white? 1:-1)*(fast? 2:1));
    }
    string pawnTakeMove(bool left) {
        int fileChange = white?1:-1;
        if(left) fileChange *= -1;
        return Utils.positionFrom(position, fileChange, white?1:-1);
    }


    public void updatePossibleMoves() {
        possibleMoves[0] = pawnForwardMove();
        if(position[1] == (white? '2':'7'))
            possibleMoves[1] = pawnForwardMove(true);
        else possibleMoves[1] = null;
        possibleMoves[2] = pawnTakeMove(true);
        possibleMoves[3] = pawnTakeMove(false);
        checkMoveSpace();
        ++(Utils.numPiecesUpdated);
    }
    private void checkMoveSpace() {
        if(!allowedMove(possibleMoves[0])) possibleMoves[0] = null;
        if(!allowedMove(possibleMoves[1])) possibleMoves[1] = null;
        if(!allowedCapture(possibleMoves[2]))
            possibleMoves[2] = null;
        if(!allowedCapture(possibleMoves[3]))
            possibleMoves[3] = null;
    }
    private bool allowedMove(string move) { 
        if(move is null) return false;
        string piece = board.pieceAt(move);
        return (piece is null);
    }
    private bool allowedCapture(string move) {
        if(move is null) return false;
        string piece = board.pieceAt(move);
        if(!(piece is null) && piece[0] == (white? 'b' : 'w'))
            return true;
        else return false;
    }
}
