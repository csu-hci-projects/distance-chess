using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece {

    void Update() {
        
    }

    public override bool validMove(string move) => validMove(Utils.file(move), Utils.rank(move));
    public override bool validMove(int file, int rank) {
        int cf=this.file(), cr=this.rank();
        if(file == -1 || rank == -1) return false;
        int df=Mathf.Abs(cf - file), dr=Mathf.Abs(cr - rank);
        return base.validMove(file, rank) && (
            (df == 1 && dr == 2) ||
            (df == 2 && dr == 1)
        );
    }
}
