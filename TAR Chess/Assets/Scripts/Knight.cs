using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour {
    public Board board;
    public string position, movePosition = null;
    public List<string> possibleMoves = new List<string>();
    public string pin = null;
    public bool white;
    private bool justMoved = false;

    void Start() {
        board.put(Utils.piece(white, 'n'), position);
        updatePossibleMoves();
        foreach(string attack in Utils.getKnightAttacksFrom(position)) {
            board.addAttacker(white, attack, position);
        }
    }

    void Update() {
        movePosition = Utils.backPieceMoveFromPGN(board.moveToMake, "N");
        if(!(movePosition is null))
            movePosition = Utils.validateMovePosition(
                movePosition, white, board, possibleMoves, position, "N"
            );

        if(board.needsUpdate(position) || justMoved) {
            if(Utils.pieceColor(board.pieceAt(position)) != (white? 'w':'b'))
                gameObject.SetActive(false);
            updatePossibleMoves();
            pin = Utils.getPin(board.pins, pin);
            justMoved = false;
        }
        
        if(Utils.updateMove(board, transform, position, movePosition)) {
            position = Utils.position(Utils.file(movePosition), Utils.rank(movePosition));
            movePosition = null;
            justMoved = true;
        }
    }

    public void updatePossibleMoves() {
        if(Utils.validPosition(pin)) { // if the knight is pinned, there is no possible move for the knight
            possibleMoves.Clear();
            return;
        }

        possibleMoves = Utils.getKnightAttacksFrom(position);
        List<string> illegalMoves = new List<string>();

        foreach(string move in possibleMoves) {
            string piece = board.pieceAt(move);
            if(!Utils.validPiece(piece))
                // no pin at this point, so always a valid move for the knight if square is not occupied
                continue;
            if(Utils.pieceColor(piece) != (white? 'w':'b'))
                // no pin, so knight can capture as long as other piece is opposite color
                continue;
            // at this point, we know the position is occupied by a piece of the same color
            // so, it is an illegal move
            illegalMoves.Add(move);
        }

        // clear illegal moves from possible moves
        foreach(string move in illegalMoves)
            possibleMoves.Remove(move);
    }
}
