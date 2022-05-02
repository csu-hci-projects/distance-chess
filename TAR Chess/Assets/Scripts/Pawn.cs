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
            possibleMoves.Add(Piece.Position(f, r+fwd));
            if(validMove(f, r+2*fwd))
                possibleMoves.Add(Piece.Position(f, r+2*fwd));
        }
        if(validMove(f-1, r+fwd))
            possibleMoves.Add(Piece.Position(f-1, r+fwd));
        if(validMove(f+1, r+fwd))
            possibleMoves.Add(Piece.Position(f+1, r+fwd));
    }

    public override bool validMove(string move) => validMove(Piece.file(move), Piece.rank(move));
    public override bool validMove(int file, int rank) {
        int cf=this.file(), cr=this.rank();
        if(file<0 || file>7 || rank<0 || rank>7)
            return false;
        Debug.Log("Move "+Position(file,rank)+" is on board.");
        if(game.occupied(file,rank))
            return false;
        Debug.Log("Tile empty");
        if(file == cf)
            return (
                (rank == cr + forward()) ||
                (!hasMoved && validMove(cf, cr+forward()) && rank == cr + 2*forward())
            );
        Debug.Log("Checking captures.");
        if(Mathf.Abs(cf-file) == 1)
            return rank == cr + forward();
        
            return false;
    }

    int forward(int distance = 1) => (color==PieceColor.white? 1:-1)*distance;
}
