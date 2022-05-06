using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece {
    public bool hasMoved = false;
    void Update() {
        
    }

    public override bool validMove(string move) => validMove(Utils.file(move), Utils.rank(move));
    public override bool validMove(int file, int rank) {
        if(game.occupied(file,rank))
            return false;
        int cf=this.file(), cr=this.rank();
        if(cf != file && cr != rank)
            return false;
        if(cf == file && cr == rank)
            return false;
        int maxDist = Mathf.Max( Mathf.Abs(file - cf), Mathf.Abs(rank - cr) );
        int dir = maxDist;
        if(cf == file)
            dir /= rank - cr;
        else
            dir /= file - cf;
        for(int dist=1; dist<maxDist; ++dist) {
            int f = cf + dist*dir, r = cr + dist*dir;
            if(cf == file) f = file;
            else r = rank;

            if(game.occupied(f, r))
                return false;
        }
        return true;
    }
}
