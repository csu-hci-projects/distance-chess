using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {
    public Board board;
    public bool white, alive;
    public string position;
    public string[] possibleMoves;
    const string FILE = "ABCDEFGH", RANK = "12345678";

    void Start() {
        board.put("wp", position);
        possibleMoves = new string[4];
        updatePossibleMoves();
    }

    void Update() {
        
    }

    string pawnForwardMove(bool fast = false) {
        return getMoveTile(0,(white? 1:-1)*(fast? 2:1));
    }
    string pawnTakeMove(bool left) {
        int fileChange = white?1:-1;
        if(left) fileChange *= -1;
        return getMoveTile(fileChange, white?1:-1);
    }

    string getMoveTile(int fileChange, int rankChange) {
        int file = getFile() + fileChange;
        int rank = getRank() + rankChange;
        if(file < 0 || file > 7 || rank < 0 || rank > 7)
            return null;
        else return
            FILE.Substring(file,1) +
            RANK.Substring(rank,1);
    }

    int getFile() { return Board.file(position); }
    int getRank() { return Board.rank(position); }

    public void updatePossibleMoves() {
        possibleMoves[0] = pawnForwardMove();
        if(position[1] == (white? '2':'7'))
            possibleMoves[1] = pawnForwardMove(true);
        else possibleMoves[1] = null;
        possibleMoves[2] = pawnTakeMove(true);
        possibleMoves[3] = pawnTakeMove(false);
        checkMoveSpace();
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
