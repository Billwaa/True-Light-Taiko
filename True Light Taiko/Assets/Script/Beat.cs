using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour
{

    public float speed = 3;
    public Transform drumHit;
    public float hitTime;

    public SequenceManager manager;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void setIntercept(Transform drumHit, float speed, float time)
    {
        this.drumHit = drumHit;
        this.speed = speed;
        this.transform.position = new Vector3(drumHit.position.x, drumHit.position.y + speed * time, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position += new Vector3(0, -speed, 0) * Time.deltaTime;
        this.transform.position = new Vector3(drumHit.position.x, drumHit.position.y + (hitTime - manager.musicPlayer.time) * speed, 0);

        if (this.transform.position.y < drumHit.position.y)
        {
            this.GetComponent<SpriteRenderer>().color = Color.grey;
        }

        if (this.transform.position.y < -10)
        {
            Destroy(this.gameObject);
        }

        if (!manager.musicPlayer.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
