using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece {

    void Update() {
        
    }

    public override bool validMove(string move) => validMove(Utils.file(move), Utils.rank(move));
    public override bool validMove(int file, int rank) {
        return base.validMove(file, rank) && 
            (allowedIfBishop(file,rank) || allowedIfRook(file, rank))
        ;
    }

    bool allowedIfBishop(int file, int rank) {
        int cf=this.file(), cr=this.rank();
        if(cf == file || Mathf.Abs(cf-file) != Mathf.Abs(cr-rank))
            return false;
        int fdir = file-cf > 0? 1:-1,
            rdir = rank-cr > 0? 1:-1;
        int f=cf+fdir, r=cr+rdir;
        while(f != file) {
            if(game.occupied(f,r))
                return false;
            f += fdir;
            r += rdir;
        }
        return true;
    }
    bool allowedIfRook(int file, int rank) {
        int cf=this.file(), cr=this.rank();
        if(cf != file && cr != rank)
            return false;
        if(cf == file && cr == rank)
            return false;
        for(int dist=1; dist<Mathf.Abs(file-cf); ++dist) {
            if(game.occupied((cf==file? cf:cf+dist), (cr==rank? cr:cr+dist)))
                return false;
        }
        return true;
    }
}
