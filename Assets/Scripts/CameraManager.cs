using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraManager : MonoBehaviour {
    public List<Camera> cameras;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            var currentIndex = cameras.FindIndex(c => c.gameObject.activeSelf);
            var nextIndex = (currentIndex + 1) % cameras.Count;
            cameras[currentIndex].gameObject.SetActive(false);
            cameras[nextIndex].gameObject.SetActive(true);
            Debug.Log(cameras[nextIndex].name + " selected");
        }
    }
}
