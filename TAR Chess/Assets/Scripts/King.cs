using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece {
    public bool hasMoved = false;

    void Update() {
        
    }

    public override bool validMove(string move) => validMove(Utils.file(move), Utils.rank(move));
    public override bool validMove(int file, int rank) {
        int cf = this.file(), cr = this.rank();
        if(cf == file && cr == rank)
            return false;
        return base.validMove(file,rank) &&
            Mathf.Abs(cf-file) < 2 &&
            Mathf.Abs(cr-rank) < 2
        ;
    }
}
