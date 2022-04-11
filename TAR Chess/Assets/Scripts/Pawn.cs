using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {
    public Board board;
    public bool white, alive;
    public string position, movePosition;
    public string[] possibleMoves;
    public List<string> pins;

    void Start() {
        possibleMoves = new string[4];
        pins = new List<string>();
        transform.localPosition = Utils.getLocalCoordsFromPosition(position);
        board.put((white? "w":"b") + "p", position);
        updatePossibleMoves();
        board.addPawnAttacksFromTile(white, position);
    }

    void Update() {
        if(board.needsUpdate(position)) {
            pins.Clear();
            foreach(string pin in board.pins) {
                if(!pin.Contains(position)) continue;
                else pins.Add(pin.Substring(0,2));
            }
            updatePossibleMoves();
        }
        if(!(movePosition is null) && movePosition.Length > 0) {
            if(Utils.pieceAt(transform, movePosition)) {
                board.movePiece(position, movePosition);
                position = movePosition;
                transform.localPosition = Utils.getLocalCoordsFromPosition(position);
                movePosition = null;
            }
            transform.localPosition = Utils.moveTowards(transform, movePosition);
        }
    }

    string pawnForwardMovePosition(bool fast = false) {
        return Utils.positionFrom(position, 0, (white? 1:-1)*(fast? 2:1));
    }
    string pawnTakeMovePosition(bool left) {
        int fileChange = white?1:-1;
        if(left) fileChange *= -1;
        return Utils.positionFrom(position, fileChange, white?1:-1);
    }

    void initiateMove(int index) {
        if(possibleMoves[index] is null) return;

        if(!board.movePiece(position, possibleMoves[index])) {
            possibleMoves[index] = null;
            return;
        }

        updatePossibleMoves();
    }

    public void updatePossibleMoves() {
        possibleMoves[0] = pawnForwardMovePosition();
        if(position[1] == (white? '2':'7'))
            possibleMoves[1] = pawnForwardMovePosition(true);
        else forbidMove(1);
        possibleMoves[2] = pawnTakeMovePosition(true);
        possibleMoves[3] = pawnTakeMovePosition(false);
        checkMoveSpace();
        checkPins();
    }
    private void checkMoveSpace() {
        if(!allowedMove(possibleMoves[0])) forbidMove(0);
        if(!allowedMove(possibleMoves[1])) forbidMove(1);
        if(!allowedCapture(possibleMoves[2]))
            forbidMove(2);
        if(!allowedCapture(possibleMoves[3]))
            forbidMove(3);
    }
    private void checkPins() {
        if(pins.Count == 0) return;
        if(pins.Count > 1) {
            for(int i=0; i<4; ++i)
                forbidMove(i);
            return;
        }

        string pinnerPosition = pins[0];
        List<string> betweens = Utils.getPositionsBetween(position,pinnerPosition);
        for(int i=0; i<2; ++i) {
            string move = possibleMoves[i];
            if(!(move is null) && !betweens.Contains(move)) forbidMove(i);
        }
        for(int i=2; i<4; ++i) {
            if(possibleMoves[i] != pinnerPosition) forbidMove(i);
        }
    }
    private bool allowedMove(string move) { 
        if(move is null) return false;
        string piece = board.pieceAt(move);
        return (piece is null);
    }
    private bool allowedCapture(string move) {
        if(move is null) return false;
        string piece = board.pieceAt(move);
        if(!(piece is null) && piece[0] == (white? 'b' : 'w'))
            return true;
        else return false;
    }

    private void forbidMove(int index) {
        possibleMoves[index] = null;
    }
}
