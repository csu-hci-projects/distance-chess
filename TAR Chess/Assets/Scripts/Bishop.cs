using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece {

    void Update() {
        
    }

    public override bool validMove(string move) => validMove(Utils.file(move), Utils.rank(move));
    public override bool validMove(int file, int rank) {
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
        return base.validMove(file, rank);
    }
}
