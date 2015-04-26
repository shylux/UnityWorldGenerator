using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Lobby : MonoBehaviour {
    public GameObject MazeLength;
    public GameObject MazeWidth;
    public GameObject MazeLaneLength;

    void Start() {
        MazeLength.GetComponent<InputField>().text = GameProperties.MazeLength.ToString();
        MazeWidth.GetComponent<InputField>().text = GameProperties.MazeWidth.ToString();
        MazeLaneLength.GetComponent<InputField>().text = GameProperties.MaxLaneLength.ToString();
    }

    public void onLengthChange(string s) {
        GameProperties.MazeLength = int.Parse(s);
    }
    public void onWidthChange(string s) {
        GameProperties.MazeWidth = int.Parse(s);
    }
    public void onLaneLengthChange(string s) {
        GameProperties.MaxLaneLength = int.Parse(s);
    }

	public void onStartButtonClick() {
		Application.LoadLevel ("Maze");
	}
}
