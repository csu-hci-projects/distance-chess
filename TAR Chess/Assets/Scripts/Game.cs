using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public Piece.PieceColor playerColor = Piece.PieceColor.white;
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
    public void kill(string position) => kill(Piece.file(position), Piece.rank(position));
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
        const string PIECEENUM = "rnsqk";
        if(validateMove(Piece.validPosition(engineMove)))
            return true;
        if(PIECEENUM.Contains(engineMove.Substring(0,1))) {
            return validateMove(Piece.validPosition(engineMove.Substring(1)));
        }

        if("abcdefgh".Contains(engineMove.Substring(0,1))) {
            char mtype = engineMove[1];
            if("abcdefgh".Contains(""+mtype))
                return validateMove(Piece.validPosition(engineMove.Substring(1)));
            if(mtype == '=')
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
        }
    }
    void getMoves() {
        if(engineMove.Length == 0)
            return;
        if(moves.Count == 0) {
            foreach(Piece piece in pieces) {
                if(appliesToPiece(piece)) {
                    moves.Add(piece, getMovePosition(piece));
                }
            }
        }
        
        engineMove = "";
        engineMoveIndex = engineMoves.Length;
    }
    bool appliesToPiece(Piece piece) {
        string movePosition = getMovePosition(piece);
        if(Piece.validPosition(movePosition)) {
            if(piece.type != Piece.PieceType.pawn)
                return false;
            if(piece.file() != Piece.file(movePosition))
                return false;
            return piece.validMove(movePosition);
        }

        return false;
    }
    string getMovePosition(Piece piece) {
        if(engineMove is null || engineMove.Length < 2)
            return null;
        return engineMove.Substring(engineMove.Length - 2);
    }
}
