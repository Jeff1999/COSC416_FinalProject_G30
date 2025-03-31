using UnityEngine;

public class ButtonUI : MonoBehaviour
{
    public static bool isActive = false;

    public void NewGameButton(){
        isActive = true;
        Debug.Log("New Game Button Pressed");
    }
}
