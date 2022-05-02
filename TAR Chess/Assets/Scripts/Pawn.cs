using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {
    public bool hasMoved = false;

    void Update() {
        if(!updated) {
            updatePossibleMoves();
            updated = true;
        }
    }

    public override void updatePossibleMoves() {
        possibleMoves.Clear();
        int f=file(), r=rank(), fwd=forward();
        if(validMove(f, r+fwd)) {
            possibleMoves.Add(positionFromPiece(0, fwd));
            if(validMove(f, r+2*fwd))
                possibleMoves.Add(positionFromPiece(0, 2*fwd));
        }
        if(validMove(f-1, r+fwd))
            possibleMoves.Add(positionFromPiece(-1, fwd));
        if(validMove(f+1, r+fwd))
            possibleMoves.Add(positionFromPiece(1, fwd));
    }

    public override bool validMove(string move) => validMove(Utils.file(move), Utils.rank(move));
    public override bool validMove(int file, int rank) {
        int cf=this.file(), cr=this.rank();
        if(file<0 || file>7 || rank<0 || rank>7)
            return false;
        if(game.occupied(file,rank))
            return false;
        if(file == cf)
            return (
                (rank == cr + forward()) ||
                (!hasMoved && validMove(cf, cr+forward()) && rank == cr + 2*forward())
            );
        if(Mathf.Abs(cf-file) == 1)
            return rank == cr + forward();
        
            return false;
    }
}
