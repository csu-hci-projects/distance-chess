using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {
    public bool hasMoved = false;
    public override void updatePossibleMoves() {
        possibleMoves.Clear();
        int forward = color == PieceColor.white? 1:-1;
        string move = positionFromPiece(0, forward);
        if(!(move is null) && !game.occupied(file(), rank() + forward)) {
            possibleMoves.Add(move);
            move = positionFromPiece(0, forward*2);
            if(!hasMoved && !(move is null) && !game.occupied(file(), rank() + forward*2))
                possibleMoves.Add(move);
        }
        move = positionFromPiece(-1, forward);
        if(!(move is null) && !game.occupied(file() - 1, rank() + forward))
            possibleMoves.Add(move);
        move = positionFromPiece(1, forward);
        if(!(move is null) && !game.occupied(file() + 1, rank() + forward))
            possibleMoves.Add(move);
    }
}
