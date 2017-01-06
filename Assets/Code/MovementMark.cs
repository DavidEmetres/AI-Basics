using UnityEngine;
using System.Collections;

public class MovementMark : MonoBehaviour {

    float timer;

    public float duration;

	void Start () {
        timer = 0f;
	}
	
	void Update () {
        timer += Time.deltaTime;

        if (timer >= duration)
            Destroy(this.gameObject);  
	}
}
