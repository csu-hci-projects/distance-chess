using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public string[,] position = new string[8,8];
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

    public string pieceAt(string position) {
        return pieceAt(Utils.file(position),Utils.rank(position));
    }
    public string pieceAt(int file, int rank) {
        if(file < 0 || file > 7 || rank < 0 || rank > 7) return null;
        else return position[file,rank];
    }

    public void put(string value, string position) {
        put(value,Utils.file(position),Utils.rank(position));
        if(value[1] == 'k') kingPosition[value[0] == 'w'] = position;
    }
    public void put(string value, int file, int rank) {
        if(file < 0 || file > 7 || rank < 0 || rank > 7) return;
        position[file,rank] = value;
    }

    // used for pins primarily; TODO for non-king-pawn scripting
    public bool needsUpdate(string position) {
        return false;
    }

    public bool movePiece(string fromPosition, string toPosition) {
        if(fromPosition is null) return false;
        string movingPiece = pieceAt(fromPosition);
        if(movingPiece is null) return false;
        if(toPosition is null) return false;

        bool moverWhite = movingPiece[0] == 'w';
        char moverType = movingPiece[1];

        if(moverType == 'k' && attackingPosition(!moverWhite, toPosition))
            return false;
        if(moverType == 'p') {
            removePawnAttacksFromTile(moverWhite, fromPosition);
            addPawnAttacksFromTile(moverWhite, toPosition);
        }
        return true;
    }
        public void addPawnAttacksFromTile(bool white, string position) {
            int forward = white? 1:-1;
            string
                left = Utils.positionFrom(position,-1,forward),
                right = Utils.positionFrom(position,1,forward);
            addAttacker(white, left, position);
            addAttacker(white, right, position);
        }
        public void removePawnAttacksFromTile(bool white, string position) {
            int forward = white? 1:-1;
            string
                left = Utils.positionFrom(position,-1,forward),
                right = Utils.positionFrom(position,1,forward);
            removeAttacker(left, position);
            removeAttacker(right, position);
        }

    public bool attackingPosition(bool attackerIsWhite, string position) {
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
        if (
            attackedPosition is null || attackedPosition.Length != 2 ||
            attackingPosition is null || attackingPosition.Length != 2
        ) return;

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
        if (
            attackedPosition is null || attackedPosition.Length != 2 ||
            attackingPosition is null || attackingPosition.Length != 2
        ) return;

        int file=Utils.file(attackedPosition), rank=Utils.rank(attackedPosition);

        if(attackersOf[file,rank] is null)
            attackersOf[file,rank] = "";
        
        attackersOf[file,rank] = attackersOf[file,rank].Replace("w"+attackingPosition, "");
        attackersOf[file,rank] = attackersOf[file,rank].Replace("b"+attackingPosition, "");
    }
}
