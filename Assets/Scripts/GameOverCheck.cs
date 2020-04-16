using UnityEngine;
using System.Collections;
using System.Linq;

public class GameOverCheck : MonoBehaviour {
    public Collider roomCollider;
    public GameObject doneUI;



    private void Start() {
        StartCoroutine(CheckDone());
    }


    private IEnumerator CheckDone() {
        while(!Done()) {
            yield return new WaitForSeconds(1);
        }

        doneUI.SetActive(true);
    }

    private bool Done() {
        var collisions = Physics.OverlapBox(roomCollider.bounds.center, roomCollider.bounds.extents);
        var count = collisions.Count(c => c.GetComponent<Rigidbody>() != null);
        Debug.Log("collisions.length: " + collisions.Length);
        return count == 0;
    }

}
