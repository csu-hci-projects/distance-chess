using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : MonoBehaviour {
    public Board board;
    public bool white;
    public string position;
    public string[] possibleMoves = new string[10];
    public bool firstMove = true;
    public bool isInCheck = false;

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(board.needsUpdate(position)) {
            isInCheck = board.check.Contains(white? "w":"b");
            updatePossibleMoves();
        }
    }

    public void updatePossibleMoves() {
        // moves index 0-7 mean single tile moves
        possibleMoves[0] = Utils.positionFrom(position, -1, -1);
        possibleMoves[1] = Utils.positionFrom(position, 0, -1);
        possibleMoves[2] = Utils.positionFrom(position, 1, -1);
        possibleMoves[3] = Utils.positionFrom(position, 1, 0);
        possibleMoves[4] = Utils.positionFrom(position, 1, 1);
        possibleMoves[5] = Utils.positionFrom(position, 0, 1);
        possibleMoves[6] = Utils.positionFrom(position, -1, 1);
        possibleMoves[7] = Utils.positionFrom(position, -1, 0);

        // index 8 is castle kingside, 9 castle queenside
        if(!firstMove) {
            possibleMoves[8] = null;
            possibleMoves[9] = null;
        } else {
            possibleMoves[8] = Utils.positionFrom(position, 2, 0);
            possibleMoves[9] = Utils.positionFrom(position, -2, 0);
        }

        checkNormalMoves();
        checkCastles();
    }

    public void checkNormalMoves() {
        for(int i=0; i<8; ++i) {
            string movePosition = possibleMoves[i];
            if(movePosition is null) // nothing to check if move position doesn't exist on the board
                continue;
            string piece = board.pieceAt(movePosition);
            if(piece is null) { // if there's no piece there
                if(board.isAttackedBy(!white, movePosition)) // would the move put the king into check
                    possibleMoves[i] = null; // the move is illegal
                continue; // otherwise it's a legal move
            }
            
            // here, there is a piece in the move position
            bool pieceIsWhite = Utils.pieceIsWhite(piece);
            if(pieceIsWhite ^ white) { // if the piece is the other color
                bool guarded = board.isAttackedBy(!white, movePosition);
                if(guarded) { // and it is guarded, it's an illegal move
                    possibleMoves[i] = null;
                }
                // if it's not guarded, it can be taken and we don't need to nullify the move
            } else { // if the piece is the same color, it's an illegal move
                possibleMoves[i] = null;
            }
            // otherwise, the piece can be taken and is a legal move
        }
    }
    public void checkCastles() {
        if(!firstMove)
            return;

        bool kingside = canCastle(true), queenside = canCastle(false);
        if(!kingside)
            possibleMoves[8] = null;
        if(!queenside)
            possibleMoves[9] = null;
    }
    private bool canCastle(bool kingside) {
        // first check checks: king cannot castle across or into check
        if(board.isAttackedBy(!white, Utils.positionFrom(position, kingside? 1:-1, 0)))
            return false;
        if(board.isAttackedBy(!white, Utils.positionFrom(position, kingside? 2:-2, 0)))
            return false;
        return true;
    }
}
