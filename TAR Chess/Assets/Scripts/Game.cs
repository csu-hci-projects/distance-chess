using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public Utils util;
    public Utils.PieceColor playerColor = Utils.white;
    public string engineMoves = "";
    public int engineMoveIndex = 0;
    private string engineMove = "";
    private List<Piece> pieces = new List<Piece>(32);
    public Piece[,] board = new Piece[8,8];
    public const float moveTime = 0.1f;

    void Start() {
        
    }

    void Update() {
        if(engineMoveIndex + engineMove.Length < engineMoves.Length) {
            engineMove = engineMoves.Substring(engineMoveIndex);
        }
        executeMoves();
    }

    public void addPiece(Piece piece) {
        pieces.Add(piece);
        board[piece.file(), piece.rank()] = piece;
    }
    public void kill(string position) => kill(Utils.file(position), Utils.rank(position));
    public void kill(int file, int rank) {
        if(file<0 || file>7 || rank<0 || rank>7)
            return;
        Piece p = board[file,rank];
        if(!(p is null))
            p.die();
        board[file,rank] = null;
    }

    public bool occupied(int file, int rank) {
        return !(board[file,rank] is null);
    }

    public bool validMove() {
        if(engineMove.Length < 2)
            return false;
        if(engineMove[0] == '-' && Utils.validPosition(engineMove.Substring(1))) // kill piece at pos
            return true;
        const string PIECEENUM = "rnsqk";
        if(validateMove(Utils.validPosition(engineMove)))
            return true;
        if(PIECEENUM.Contains(engineMove.Substring(0,1))) {
            return validateMove(Utils.validPosition(engineMove.Substring(1)));
        }

        if("abcdefgh".Contains(engineMove.Substring(0,1))) {
            char mtype = engineMove[1];
            if("abcdefgh".Contains(""+mtype)) // pawn capture
                return validateMove(Utils.validPosition(engineMove.Substring(1)));
            if(mtype == '=') // pawn promotion, ex. e-pawn to queen would be `e=q`
                return validateMove(PIECEENUM.Contains(engineMove.Substring(2)));
        }

        return false;

    }
        private bool validateMove(bool predicate) {
            if(predicate) {
                engineMoveIndex = engineMoves.Length;
                return true;
            } else
                return false;
        }
    
    Dictionary<Piece, string> moves = new Dictionary<Piece, string>();
    Dictionary<Piece, string> done = new Dictionary<Piece, string>();
    void executeMoves() {
        getMoves();
        foreach(KeyValuePair<Piece, string> move in moves) {
            Piece piece = move.Key;
            if(piece.glideTo(move.Value)) done.Add(move.Key, move.Value);
        }
        foreach(Piece p in done.Keys) {
            moves.Remove(p);
            signalAllPiecesForUpdates();
        }
        done.Clear();
    }
    void getMoves() {
        if(engineMove.Length < 2)
            return;
        if(moves.Count == 0) {
            foreach(Piece piece in pieces) {
                if(appliesToPiece(piece)) {
                    moves.Add(piece, getMovePosition(piece));
                }
            }
        }

        if(moves.Count > 0) {
            engineMove = "";
            engineMoveIndex = engineMoves.Length;
        }
    }
    bool appliesToPiece(Piece piece) {
        if(Utils.validPosition(engineMove)) {
            if(piece.type != Utils.pawn)
                return false;
            if(piece.file() != Utils.file(engineMove))
                return false;
            return piece.validMove(engineMove);
        } else if(engineMove[0] >= 'a' && engineMove[0] <= 'h') {
            if(piece.type != Utils.pawn)
                return false;
            if(piece.position[0] != engineMove[0])
                return false;
            return piece.validMove(engineMove.Substring(1));
        }

        return false;
    }
    string getMovePosition(Piece piece) {
        if(engineMove is null || engineMove.Length < 2)
            return null;
        if(engineMove.Length == 3) {
            char file = engineMove[0];
            if(file >= 'a' && file <= 'h') { // it's a pawn
                if(engineMove[1] == '=') { // it's a promotion
                    string rank = playerColor == Utils.white? "1":"8";
                    return file + rank;
                }
            }
            if(engineMove.Contains("o")) {
                string rank = (piece.color == Utils.white? "1":"8");
                if(engineMove.Equals("o o"))
                    return (piece.type == Utils.king? "c":"d") + rank;
                if(engineMove.Equals("ooo"))
                    return (piece.type == Utils.king? "g":"f") + rank;
            }
        }
        return engineMove.Substring(engineMove.Length - 2);
    }
    void signalAllPiecesForUpdates() {
        foreach(Piece p in pieces)
            p.updated = false;
    }
}
