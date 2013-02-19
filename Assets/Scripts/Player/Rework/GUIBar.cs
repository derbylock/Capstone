using UnityEngine;
using System.Collections;

public class GUIBar : ScriptableObject {
  public Rect bgPlacement = new Rect();
  public Rect barPlacement = new Rect();
  public Texture2D background;
  public Color bgColor;
  public Texture2D foreground;
  public Color fgColor;

  public void SetBarScale(Vector2 position, Vector2 scale) {
    barPlacement.x = position.x;
    barPlacement.y = position.y;
    barPlacement.width = Screen.width * scale.x;
    barPlacement.height = Screen.height * scale.y;
  }

  public void SetBackgroundBuffer(float percentageOfBar) {
    float hBuffer = barPlacement.width * percentageOfBar;
    float vBuffer = barPlacement.height * percentageOfBar;
    bgPlacement.x = barPlacement.x - hBuffer / 2;
    bgPlacement.y = barPlacement.y - vBuffer / 2;
    bgPlacement.width = barPlacement.width + hBuffer;
    bgPlacement.height = barPlacement.height + vBuffer;
  }

  public void Draw() {
    //GUI.
  }
}
