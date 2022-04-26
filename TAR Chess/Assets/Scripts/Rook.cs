using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : MonoBehaviour {
    public Board board;
    public King king;
    public bool white;
    public string position, movePosition, pin = null;
    public List<string> possibleMoves = new List<string>();

    public bool kingside;
    public bool firstMove = true;

    void Start() {
        board.put(Utils.piece(white, 'r'), position);
    }

    // Update is called once per frame
    void Update() {
        movePosition = Utils.rookMoveFromPGN(board.moveToMake, this);
        if(!(movePosition is null)) {
            movePosition = Utils.validateMovePosition(
                movePosition, white, board, possibleMoves, position, "R"
            );
            if(!(movePosition is null) && board.moveToMake.Contains("o")) {
                bool concerningThisRook = board.moveToMake.Equals(
                    (kingside? "o o":"ooo")
                );
                bool isALegalMove = king.canCastle(kingside);

                if(!(concerningThisRook && isALegalMove))
                    movePosition = null;
            }
        }

        if(board.needsUpdate(position)) {
            if(Utils.pieceColor(board.pieceAt(position)) != (white? 'w':'b'))
                gameObject.SetActive(false);
            updatePossibleMoves();
        }
        
        if(Utils.updateMove(this)) {
            position = Utils.position(Utils.file(movePosition), Utils.rank(movePosition));
            firstMove = false;
            movePosition = null;
        }
    }

    void updatePossibleMoves() {
        possibleMoves = Utils.getRookAttacksFrom(board, white, position);
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
