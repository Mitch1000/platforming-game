using UnityEngine;

public class LimitFrameRate : MonoBehaviour
{
  // Start is called before the first frame update
  void Awake()
  {
    QualitySettings.vSyncCount = 0;

    Application.targetFrameRate = 60;
  }
}

