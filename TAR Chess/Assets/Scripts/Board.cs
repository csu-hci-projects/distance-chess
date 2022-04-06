using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    const string FILE = "ABCDEFGH";
    const string RANK = "12345678";

    public string[,] position;
    void Start() {
        position = new string[8,8];
    }

    // Update is called once per frame
    void Update() {
        
    }

    public string pieceAt(string position) {
        return pieceAt(file(position),rank(position));
    }
    public string pieceAt(int file, int rank) {
        if(file < 0 || file > 7 || rank < 0 || rank > 7) return "INV";
        else return position[file,rank];
    }

    public void put(string value, string location) {
        
    }

    public static int file(string position) {
        return FILE.IndexOf(position[0]);
    }

    public static int rank(string position) {
        return RANK.IndexOf(position[1]);
    }
}
