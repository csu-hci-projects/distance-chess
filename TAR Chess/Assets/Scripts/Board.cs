using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public string[,] boardPosition = new string[8,8];
    public bool[,] tilesToUpdate = new bool[8,8];
    public string[,] attackersOf = new string[8,8];
    public Dictionary<bool, string> kingPosition = new Dictionary<bool, string>();
    public string check = "";
    public List<string> pins;
    void Start() {
        pins = new List<string>();
        signalUpdatesFromMove();
    }

    // Update is called once per frame
    void Update() {
        
    }

    // returns piece information of the piece located at `position`
    public string pieceAt(string position) {
        if(!Utils.validPosition(position))
            return null;
        
        return boardPosition[Utils.file(position), Utils.rank(position)];
    }

    // puts piece information `value` at `position`
    public bool put(string piece, string position) {
        if(!Utils.validPosition(position))
            return false;
        
        if(Utils.validPiece(piece) && Utils.pieceType(piece) == 'k') {
            // if the piece is a king, update the king position
            kingPosition[Utils.pieceIsWhite(piece)] = position;
        }

        boardPosition[Utils.file(position), Utils.rank(position)] = piece;
        
        return true;
    }

    // used for pins primarily; TODO for non-king-pawn scripting
    public bool needsUpdate(string position) {
        int file = Utils.file(position), rank = Utils.rank(position);
        if(tilesToUpdate[file, rank]) {
            tilesToUpdate[file, rank] = false;
            return true;
        } else {
            return false;
        }
    }
    public void signalForUpdate(string position) {
        if(!Utils.validPosition(position))
            return;
        else
            tilesToUpdate[Utils.file(position), Utils.rank(position)] = true;
    }
    public void signalUpdatesFromMove(string fromPosition ="", string toPosition ="") {
        // trivial solution to updating pieces: force every piece to update
        // not very efficient, may want replacing
        for(int file=0; file<8; ++file) {
            for(int rank=0; rank<8; ++rank) {
                signalForUpdate(Utils.position(file,rank));
            }
        }
    }

    // returns true iff move is legal, and thus updated the board data
    public bool movePiece(string fromPosition, string toPosition) {
        if(!Utils.validPosition(fromPosition) || !Utils.validPosition(toPosition))    
            return false;
        string movingPiece = pieceAt(fromPosition);
        if(!Utils.validPiece(movingPiece)) return false;
        if(!Utils.validPosition(toPosition)) return false;

        bool moverWhite = Utils.pieceIsWhite(movingPiece);
        char moverType = Utils.pieceType(movingPiece);

        List<string> oldAttacks, newAttacks;
        if(moverType == 'k') { // king
            if(attackingPosition(!moverWhite, toPosition))
                return false;

            oldAttacks = Utils.getKingAttacksFrom(fromPosition);
            newAttacks = Utils.getKingAttacksFrom(toPosition);
        }
        else if(moverType == 'q') { // queen
            oldAttacks = new List<string>();
            newAttacks = new List<string>();
        }
        else if(moverType == 'r') { // rook
            oldAttacks = new List<string>();
            newAttacks = new List<string>();
        }
        else if(moverType == 'n') { // knight
            oldAttacks = Utils.getKnightAttacksFrom(fromPosition);
            newAttacks = Utils.getKnightAttacksFrom(toPosition);
        }
        else if(moverType == 'b') { // bishop
            oldAttacks = new List<string>();
            newAttacks = new List<string>();
        }
        else { // if(moverType == 'p') // pawn
            oldAttacks = Utils.getPawnAttacksFrom(moverWhite, fromPosition);
            newAttacks = Utils.getPawnAttacksFrom(moverWhite, toPosition);
        }

        foreach(string attack in oldAttacks)
            removeAttacker(attack, fromPosition);
        foreach(string attack in newAttacks)
            addAttacker(moverWhite, attack, toPosition);

        // find which pieces need updates because of this move
        signalUpdatesFromMove(fromPosition, toPosition);

        put(null, fromPosition);
        put(movingPiece, toPosition);
        return true;
    }

    public bool attackingPosition(bool attackerIsWhite, string position) {
        if(!Utils.validPosition(position))
            return false;
        int file=Utils.file(position), rank=Utils.rank(position);
        char color = attackerIsWhite? 'w':'b';
        string attackers = attackersOf[file,rank];
        if(attackers is null) attackersOf[file,rank] = "";
        for(int atkPce=0; atkPce < attackers.Length / 3; ++atkPce) {
            if(attackers[atkPce*3] == color)
                return true;
            else continue;
        }
        return false;
    }
    public void addAttacker(bool white, string attackedPosition, string attackingPosition) {
        // check for valid positions
        if (!Utils.validPosition(attackedPosition) || !Utils.validPosition(attackingPosition))
            return;

        int file=Utils.file(attackedPosition), rank=Utils.rank(attackedPosition);
        
        if(attackersOf[file,rank] is null)
            attackersOf[file,rank] = "";
        
        // check for adding duplicate attacker
        if(attackersOf[file,rank].Contains(attackingPosition)) return;
        
        // otherwise append attacking color & tile
        attackersOf[file,rank] += (white? "w":"b") + attackingPosition;

        // now check to see if attacked tile has the other color's king
        if(!kingPosition.ContainsKey(!white)) return;
        string otherKing = kingPosition[!white];
        if(!(otherKing is null) && otherKing.Equals(attackedPosition))
            check = (!white? "w":"b") + attackingPosition;
    }
    public void removeAttacker(string attackedPosition, string attackingPosition) {
        // check for valid positions
        if (!Utils.validPosition(attackedPosition) || !Utils.validPosition(attackingPosition))
            return;

        int file=Utils.file(attackedPosition), rank=Utils.rank(attackedPosition);

        if(attackersOf[file,rank] is null)
            attackersOf[file,rank] = "";
        
        attackersOf[file,rank] = attackersOf[file,rank].Replace("w"+attackingPosition, "");
        attackersOf[file,rank] = attackersOf[file,rank].Replace("b"+attackingPosition, "");
    }
    public bool isAttackedBy(bool white, string position) {
        if(!Utils.validPosition(position))
            return false;
        string attackers = attackersOf[
            Utils.file(position), Utils.rank(position)
        ];
        if(attackers is null)
            return false;

        for(int piece=0; piece < attackers.Length / 3; ++piece) {
            char color = attackers[piece * 3];
            if(color == (white? 'w':'b'))
                return true;
        }
        return false;
    }
}
