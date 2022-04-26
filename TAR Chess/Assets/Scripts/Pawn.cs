using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour {
    public Board board;
    public bool white, alive;
    public string position, movePosition;
    public List<string> possibleMoves = new List<string>(4);
    public string pin;

    void Start() {
        for(int i=0; i<4; ++i) possibleMoves.Add(null);
        pin = null;
        transform.localPosition = Utils.getLocalCoordsFromPosition(position);
        board.put(Utils.piece(white, 'p'), position);
        updatePossibleMoves();
        foreach(string attack in Utils.getPawnAttacksFrom(white, position))
            board.addAttacker(white, attack, position);
    }

    void Update() {
        movePosition = Utils.pawnMoveFromPGN(board.moveToMake);
        if(!(movePosition is null)) {
            if(board.whitesMove != white || !possibleMoves.Contains(movePosition))
                movePosition = null;
            else if(board.moveToMake.Contains("x") && position[0] != board.moveToMake[0])
                movePosition = null;
        }

        if(board.needsUpdate(position)) {
            pin = Utils.getPin(board.pins, position);
            // update the pawn's moveset
            updatePossibleMoves();
        }
        if(Utils.updateMove(board, transform, position, movePosition)) {
            position = Utils.position(Utils.file(movePosition), Utils.rank(movePosition));
            movePosition = null;
        }
    }

    // simple method to get position from a forward move
    string pawnForwardMovePosition(bool fast = false) {
        return Utils.positionFrom(position, 0, (white? 1:-1)*(fast? 2:1));
    }

    // in progress, may not be used
    void initiateMove(int index) {
        if(possibleMoves[index] is null) return;

        if(!board.movePiece(position, possibleMoves[index])) {
            possibleMoves[index] = null;
            return;
        }

        updatePossibleMoves();
    }

    // repopulate the possible moves array with legal move positions from current position
    public void updatePossibleMoves() {
        // first, generate all positions that could feasibly be reached
        possibleMoves[0] = pawnForwardMovePosition();
        if(position[1] == (white? '2':'7')) // if the pawn is in its starting square, add a fast forward move
            possibleMoves[1] = pawnForwardMovePosition(true);
        else forbidMove(1); // otherwise forbid it
        // get possible attack squares
        possibleMoves[2] = Utils.getPawnAttackPosition(true, white, position);
        possibleMoves[3] = Utils.getPawnAttackPosition(false, white, position);

        // now, update move space to remove illegal move positions
        checkMoveSpace();
        checkPins();
    }

    // check the current move space's legality without pin rules
    private void checkMoveSpace() {
        // first, check if the forward moves are blocked
        if(!allowedMove(possibleMoves[0]))
            forbidMove(0);
        if(!allowedMove(possibleMoves[1]))
            forbidMove(1);
        // then, check attacked squares: if they are occupied by other colored pieces
        if(!allowedCapture(possibleMoves[2]))
            forbidMove(2);
        if(!allowedCapture(possibleMoves[3]))
            forbidMove(3);
    }

    // check the current move space's validity under pin rules
    private void checkPins() {
        // if there are no pins on this piece, no work is needed
        if(pin is null)
            return;

        // get positions between this piece and pinning piece
        List<string> betweens = Utils.getPositionsBetween(position, pin);

        for(int i=0; i<2; ++i) {
            // now, ensure that possible moves only contains positions within the pin
            string move = possibleMoves[i];
            if(Utils.validPosition(move) && !betweens.Contains(move))
                forbidMove(i);
        }
        for(int i=2; i<4; ++i) {
            // capture is only allowed if this pawn captures the pinning piece
            if(possibleMoves[i] != pin)
                forbidMove(i);
        }
    }
    private bool allowedMove(string move) {
        if(!Utils.validPosition(move)) // false if the move square doesn't exist
            return false;
        string piece = board.pieceAt(move);
        return (!Utils.validPiece(piece)); // true if there is no piece at the move position
    }
    private bool allowedCapture(string move) {
        if(!Utils.validPosition(move)) // false if the square doesn't exist
            return false;
        string piece = board.pieceAt(move);
        if(!Utils.validPiece(piece)) // false if there is no piece at move position
            return false;
        else // true if piece on tile has opposite color (logical xor operator `^`)
            return Utils.pieceIsWhite(piece) ^ white;
    }

    private void forbidMove(int index) {
        possibleMoves[index] = null;
    }
}
