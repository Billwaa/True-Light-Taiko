using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{

    private AudioSource musicPlayer;
    private AudioClip song;
    
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        song = Resources.Load<AudioClip>("Music/Satisfaction");
        Debug.Log(song.name);
        musicPlayer.clip = song;
    }

    public void RecordSequence()
    {
        musicPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
