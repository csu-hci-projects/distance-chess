using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public string[,] boardPosition = new string[8,8];
    public bool[,] tilesToUpdate = new bool[8,8];
    public string[,] attackersOf = new string[8,8];
    public Dictionary<bool, string> kingPosition = new Dictionary<bool, string>();
    public bool whitesMove = true;
    public string check = "";
    public string moveToMake = "";
    public List<string> PGN = new List<string>();
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
        
        if(Utils.validPiece(piece) && Utils.pieceType(piece) == 'K') {
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
        if(!Utils.validPiece(movingPiece))
            return false;
        if(!Utils.validPosition(toPosition))
            return false;

        bool moverWhite = Utils.pieceIsWhite(movingPiece);
        char moverType = Utils.pieceType(movingPiece);

        if(whitesMove != moverWhite)
            return false;
        else addPgnMove(movingPiece, fromPosition, toPosition);

        if(moverType == 'K') { // king cannot move into check
            if(attackingPosition(!moverWhite, toPosition))
                return false;
        }

        string capturedPiece = pieceAt(toPosition);
        if(Utils.validPiece(capturedPiece)) // then
        foreach(string attack in getAttacks(capturedPiece, toPosition))
            removeAttacker(Utils.pieceIsWhite(capturedPiece), attack, toPosition);

        // now we update the attacks on all affected positions
        foreach(string attack in getAttacks(moverType, moverWhite, fromPosition))
            removeAttacker(moverWhite, attack, fromPosition);
        foreach(string attack in getAttacks(moverType, moverWhite, toPosition))
            addAttacker(moverWhite, attack, toPosition);

        // find which pieces need updates because of this move
        signalUpdatesFromMove(fromPosition, toPosition);

        put(null, fromPosition);
        put(movingPiece, toPosition);
        whitesMove = !whitesMove;
        moveToMake = null;
        return true;
    }
        private List<string> getAttacks(string piece, string position) {
            return getAttacks(Utils.pieceType(piece), Utils.pieceIsWhite(piece), position);
        }
        private List<string> getAttacks(char moverType, bool moverWhite, string position) {
            if(moverType == 'K') // king
                return Utils.getKingAttacksFrom(position);
            if(moverType == 'Q')  // queen
                return Utils.getQueenAttacksFrom(this, moverWhite, position);
            if(moverType == 'R')  // rook
                return Utils.getRookAttacksFrom(this, moverWhite, position);
            if(moverType == 'N')  // knight
                return Utils.getKnightAttacksFrom(position);
            if(moverType == 'B')  // bishop
                return Utils.getBishopAttacksFrom(this, moverWhite, position);
            // pawn
            return Utils.getPawnAttacksFrom(moverWhite, position);
        }
        private void addPgnMove(string movingPiece, string fromPosition, string toPosition) {
            string pgnMove = "";
            char moverType = Utils.pieceType(movingPiece);
            char moverColor = Utils.pieceColor(movingPiece);
            if(moverType == 'P') {
                if(!Utils.sameFile(fromPosition, toPosition)) {
                    pgnMove += fromPosition[0] + "x";
                }
                
            } else {
                pgnMove += moverType;

                string alternateFiles = "", alternateRanks = "";
                string alternates = attackersOf[Utils.file(toPosition), Utils.rank(toPosition)];
                if(alternates is null) alternates = "";
                for(int attacker=0; attacker < alternates.Length / 3; ++attacker) {
                    if(alternates[attacker*3] != moverColor)
                        continue;
                    
                    string alternatePosition = alternates.Substring(attacker*3 + 1, 2);
                    if(alternatePosition.Equals(fromPosition))
                        continue;
                    string alternateAttacker = pieceAt(alternatePosition);
                    if(!Utils.validPiece(alternateAttacker) || Utils.pieceType(alternateAttacker) != moverType)
                        continue;

                    alternateFiles += Utils.file(alternatePosition);
                    alternateRanks += Utils.rank(alternatePosition);
                    break;
            }

            if(alternateFiles.Length > 0) // other attacker of same type exists
                if(alternateFiles.Contains("" + fromPosition[0])) {
                    if(alternateRanks.Contains("" + fromPosition[1])) // neither file nor rank unique
                        pgnMove += fromPosition;
                    else // rank is unique
                        pgnMove += fromPosition[1];
                } else // file is unique
                    pgnMove += fromPosition[0];
            }

            pgnMove += toPosition;
            PGN.Add(pgnMove);
        }

    public bool attackingPosition(bool attackerIsWhite, string position) {
        if(!Utils.validPosition(position))
            return false;
        int file=Utils.file(position), rank=Utils.rank(position);
        char color = attackerIsWhite? 'w':'B';
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
    public void removeAttacker(bool white, string attackedPosition, string attackingPosition) {
        // check for valid positions
        if (!Utils.validPosition(attackedPosition) || !Utils.validPosition(attackingPosition))
            return;

        int file=Utils.file(attackedPosition), rank=Utils.rank(attackedPosition);

        if(attackersOf[file,rank] is null)
            attackersOf[file,rank] = "";
        
        attackersOf[file,rank] = attackersOf[file,rank].Replace("w"+attackingPosition, "");
        attackersOf[file,rank] = attackersOf[file,rank].Replace("b"+attackingPosition, "");

        string otherKing = kingPosition[!white];
        if(!(otherKing is null) && otherKing.Equals(attackedPosition))
            check = check.Replace((!white? "w":"b") + attackingPosition, "");
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
            if(color == (white? 'w':'B'))
                return true;
        }
        return false;
    }
}
