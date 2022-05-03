using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece {
    public bool hasMoved = false;
    void Update() {
        
    }

    public override bool validMove(string move) => validMove(Utils.file(move), Utils.rank(move));
    public override bool validMove(int file, int rank) {
        int cf=this.file(), cr=this.rank();
        if(cf != file && cr != rank)
            return false;
        if(cf == file && cr == rank)
            return false;
        for(int dist=1; dist<Mathf.Abs(file-cf); ++dist) {
            int f=(cf==file? cf:cf+dist), r=(cr==rank? cr:cr+dist);
            if(game.occupied(f, r))
                return false;
        }
        return base.validMove(file, rank);
    }
}
