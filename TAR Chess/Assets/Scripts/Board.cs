using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public string[,] boardPosition = new string[8,8];
    public string[,] attackersOf = new string[8,8];
    public Dictionary<bool, string> kingPosition;
    public string check = "";
    public List<string> pins;
    void Start() {
        kingPosition = new Dictionary<bool, string>();
        kingPosition.Add(true, "E1");
        kingPosition.Add(false, "E8");
        pins = new List<string>();
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
    public bool put(string value, string position) {
        if(!Utils.validPosition(position))
            return false;
        
        if(!(value is null || value.Length == 0)) {
            if(value[1] == 'k') // if the piece is a king, update the king position
                kingPosition[value[0] == 'w'] = position;
        } else if(!Utils.validPiece(value))
            return false;

        boardPosition[Utils.file(position), Utils.rank(position)] = value;
        
        return true;
    }

    // used for pins primarily; TODO for non-king-pawn scripting
    public bool needsUpdate(string position) {
        return false;
    }

    public bool movePiece(string fromPosition, string toPosition) {
        if(!Utils.validPosition(fromPosition) || !Utils.validPosition(toPosition))    
            return false;
        string movingPiece = pieceAt(fromPosition);
        if(movingPiece is null) return false;
        if(toPosition is null) return false;

        bool moverWhite = movingPiece[0] == 'w';
        char moverType = movingPiece[1];

        if(moverType == 'k' && attackingPosition(!moverWhite, toPosition))
            return false;
        if(moverType == 'p') {
            add_pawnAttacksFromTile(moverWhite, toPosition);
            remove_pawnAttacksFromTile(moverWhite, fromPosition);
        }

        put("", fromPosition);
        put(movingPiece, toPosition);
        return true;
    }
        public void add_pawnAttacksFromTile(bool white, string position) {
            int forward = white? 1:-1;
            string
                left = Utils.positionFrom(position,-1,forward),
                right = Utils.positionFrom(position,1,forward);
            addAttacker(white, left, position);
            addAttacker(white, right, position);
        }
        public void remove_pawnAttacksFromTile(bool white, string position) {
            int forward = white? 1:-1;
            string
                left = Utils.positionFrom(position,-1,forward),
                right = Utils.positionFrom(position,1,forward);
            removeAttacker(left, position);
            removeAttacker(right, position);
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

        if(kingPosition is null) return;
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
}
