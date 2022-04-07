using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public string[,] position = new string[8,8];
    public string[,] attackersOf = new string[8,8];
    public Dictionary<bool, string> kingPosition;
    public List<string> pins;
    void Start() {
        kingPosition = new Dictionary<bool, string>();
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
            int forward = moverWhite? 1:-1;
            removeAttacker(moverWhite, Utils.positionFrom(fromPosition,-1,forward));
            removeAttacker(moverWhite, Utils.positionFrom(fromPosition,1,forward));
            addAttacker(moverWhite, Utils.positionFrom(toPosition,-1,forward));
            addAttacker(moverWhite, Utils.positionFrom(toPosition,1,forward));
        }
        return true;
    }

    public bool attackingPosition(bool attackerIsWhite, string position) {
        string attackers = attackersOf[Utils.file(position), Utils.rank(position)];
        if(attackers is null) return false;
        return attackers[
            attackerIsWhite? 1:3
        ] != '0';
    }
    public void addAttacker(bool white, string position) {
        if(position is null) return;
        int file=Utils.file(position), rank=Utils.rank(position);
        if(attackersOf[file,rank] is null) {
            attackersOf[file,rank] = white? "w1b0" : "w0b1";
            return;
        }
        char num = attackersOf[file,rank][white? 1:3];
        // a maximum of 9 pieces can attack a square at once,
        // save some extreme exceptions which are extremely unlikely
        if(num == '9') return;
        num++;
        attackersOf[file,rank] = updatedAttackers(attackersOf[file,rank], white, num);
    }
    public void removeAttacker(bool white, string position) {
        if(position is null) return;
        int file=Utils.file(position), rank=Utils.rank(position);
        string attackers = attackersOf[file,rank];
        if(attackers is null) {
            attackersOf[file,rank] = "w0b0";
            return;
        }

        char num = attackers[white? 1:3];
        if(num == '0') return;
        else num--;
        attackersOf[file,rank] = updatedAttackers(attackers, white, num);
    }
    private string updatedAttackers(string attackers, bool white, char num) {
        return white?
            "w" + new string(num,1) + attackers.Substring(2) :
            attackers.Substring(0,2) + "b" + new string(num,1)
        ;
    }
}
