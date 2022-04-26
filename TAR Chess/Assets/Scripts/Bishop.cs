using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : MonoBehaviour {
    public Board board;
    public bool white;
    public string position, movePosition, pin = null;
    public List<string> possibleMoves = new List<string>();
    void Start() {
        board.put(Utils.piece(white, 'B'), position);
        // add guarding info to each pawn to the bishop's sides
        foreach(string attack in Utils.getBishopAttacksFrom(board, white, position)) {
            board.addAttacker(white, attack, position);
        }
    }

    void Update() {
        movePosition = Utils.backPieceMoveFromPGN(board.moveToMake, "B");
        if(!(movePosition is null))
            movePosition = Utils.validateMovePosition(
                movePosition, white, board, possibleMoves, position, "B"
            );

        if(board.needsUpdate(position)) {
            if(Utils.pieceColor(board.pieceAt(position)) != (white? 'w':'b'))
                gameObject.SetActive(false);
            updatePossibleMoves();
        }
        
        if(Utils.updateMove(board, transform, position, movePosition)) {
            position = Utils.position(Utils.file(movePosition), Utils.rank(movePosition));
            movePosition = null;
        }
    }

    void updatePossibleMoves() {
        possibleMoves = Utils.getBishopAttacksFrom(board, white, position);
        List<string> illegalMoves = new List<string>();
        foreach(string move in possibleMoves) {
            string piece = board.pieceAt(move);
            if(Utils.validPiece(piece) && Utils.pieceColor(piece) == (white? 'w':'b'))
                illegalMoves.Add(move);
        }
        if(Utils.validPosition(pin)) { // if this piece is pinned
            List<string> betweens = Utils.getPositionsBetween(position, pin);
            foreach(string move in possibleMoves) {
                if(move.Equals(pin)) // move is legal if it captures the pinning piece
                    continue;
                if(betweens.Contains(move)) // move is legal if it is within the pin
                    continue;
                illegalMoves.Add(move); // otherwise, the move is illegal
            }
        }

        // remove all the illegal moves
        foreach(string move in illegalMoves)
            possibleMoves.Remove(move);
    }
}
