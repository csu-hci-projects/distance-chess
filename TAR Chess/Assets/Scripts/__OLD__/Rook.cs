using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldScripts {
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
            if(board.needsUpdate(position)) {
                string newPiece = board.pieceAt(position);
                if(!(newPiece is null) && Utils.pieceColor(board.pieceAt(position)) != (white? 'w':'b'))
                    gameObject.SetActive(false);
                updatePossibleMoves();
            }
            
            movePosition = Utils.backPieceMoveFromPGN(board.moveToMake, "R");
            if(!(movePosition is null)) {
                movePosition = Utils.validateMovePosition(
                    movePosition, white, board, possibleMoves, position, "R"
                );
            }

            if(Utils.updateMove(board, transform, position, movePosition)) {
                position = Utils.position(Utils.file(movePosition), Utils.rank(movePosition));
                firstMove = false;
                movePosition = null;
            }
        }

        public void updatePossibleMoves() {
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
}
