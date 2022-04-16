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
        board.put(Utils.piece(white, 'p'), position);
        updatePossibleMoves();
        board.add_pawnAttacksFromTile(white, position);
    }

    void Update() {
        if(board.needsUpdate(position)) {
            pins.Clear();
            foreach(string pin in board.pins) {
                // if the pin has nothing to do with this piece, skip it
                if(!pin.Contains(position))
                    continue;
                else // otherwise add it to the local list of pins (pawns cannot pin, guaranteed to be a pin *on* this piece)
                    pins.Add(pin.Substring(0,2));
            }
            // update the pawn's moveset
            updatePossibleMoves();
        }
        // check for a move position
        if(Utils.validPosition(movePosition)) {
            // if the piece has completed its move animation
            if(Utils.pieceAt(transform, movePosition)) {
                // update the board position
                board.movePiece(position, movePosition);
                // update local position
                position = Utils.position(Utils.file(movePosition),Utils.rank(movePosition));
                transform.localPosition = Utils.getLocalCoordsFromPosition(position);
                // reset move position
                movePosition = null;
            }
            // otherwise, move towards the new position
            transform.localPosition = Utils.moveTowards(transform, movePosition);
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
        if(pins.Count == 0)
            return;

        string pinnerPosition = pins[0];
        // get positions between this piece and pinning piece
        List<string> betweens = Utils.getPositionsBetween(position,pinnerPosition);

        for(int i=0; i<2; ++i) {
            // now, ensure that possible moves only contains positions within the pin
            string move = possibleMoves[i];
            if(Utils.validPosition(move) && !betweens.Contains(move))
                forbidMove(i);
        }
        for(int i=2; i<4; ++i) {
            // capture is only allowed if this pawn captures the pinning piece
            if(possibleMoves[i] != pinnerPosition)
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
