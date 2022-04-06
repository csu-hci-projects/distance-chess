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
        possibleMoves[0] = pawnForwardMove();
        possibleMoves[1] = getMoveTile(0,white?2:-2);
        possibleMoves[2] = pawnTakeMove(true);
        possibleMoves[3] = pawnTakeMove(false);
    }

    void Update() {
        
    }

    string pawnForwardMove() { return getMoveTile(0,white?1:-1); }
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
}
