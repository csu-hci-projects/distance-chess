using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public string[,] position = new string[8,8];
    public Dictionary<bool, string> kingPosition;
    public List<string> pins;
    void Start() {
        kingPosition = new Dictionary<bool, string>();
        pins = new List<string>();
        Debug.Log("Position B2 between A3-C1? " + Utils.positionBetween("A3","B2","C1"));
        Debug.Log("Position B6 between A5-D8? " + Utils.positionBetween("A5","B6","D8"));
        Debug.Log("Position B4 between A3-C1? " + Utils.positionBetween("A3","B4","C1"));
    }

    // Update is called once per frame
    void Update() {
        
    }

    public string pieceAt(string position) {
        return pieceAt(Utils.file(position),Utils.rank(position));
    }
    public string pieceAt(int file, int rank) {
        if(file < 0 || file > 7 || rank < 0 || rank > 7) return "INV";
        else return position[file,rank];
    }

    public void put(string value, string position) {
        put(value,Utils.file(position),Utils.rank(position));
    }
    public void put(string value, int file, int rank) {
        if(file < 0 || file > 7 || rank < 0 || rank > 7) return;
        position[file,rank] = value;
    }
}
