using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public enum Drum
{
    left,
    right
}

public class Node {

    public float time;
    public Drum drum;

    public Node(float time, Drum drum)
    {
        this.time = time;
        this.drum = drum;
    }

    override public string ToString()
    {
        return $"{this.time:.00}-{this.drum}";
    }

}

public class SequenceManager : MonoBehaviour
{

    [SerializeField]
    TMP_Text textTimer;

    [SerializeField]
    AudioSource drumLeft;

    [SerializeField]
    AudioSource drumRight;
    


    private AudioSource musicPlayer;
    private AudioClip song;
    private float T0;
    private LinkedList<Node> sequence;

    
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        textTimer.text = ""+0;

        song = Resources.Load<AudioClip>("Music/Satisfaction");
        Debug.Log(song.name);
        musicPlayer.clip = song;

    }

    public void RecordSequence()
    {
        musicPlayer.Play();
        sequence = new LinkedList<Node>();
        T0 = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(musicPlayer.isPlaying)
        {
            float time = Time.fixedTime - T0;
            int minute = (int) time / 60;
            int second = (int) time % 60;
            int ms = (int) ((time * 100) % 100);

            textTimer.text = $"{minute:00}:{second:00}:{ms:00}";
        }
        else
        {
            if(sequence != null && sequence.Count > 0)
            {
                StreamWriter writer = new StreamWriter($"{song.name}.txt");

                Debug.Log("------ sequence ------");
                foreach (Node node in sequence) {
                    Debug.Log(node);
                    writer.WriteLine(node);
                }

                writer.Close();
                sequence = null;
            }
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playDrumLeft();

        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            playDrumRight();
        }

        
        
    }


    public void playDrumLeft()
    {
        drumLeft.Play();
        float time = Time.fixedTime - T0;
        Node node = new Node(time, Drum.left);
        sequence.AddLast(node);
        Debug.Log(node);
    }

    public void playDrumRight()
    {
        drumRight.Play();
        float time = Time.fixedTime - T0;
        Node node = new Node(time, Drum.right);
        sequence.AddLast(node);
        Debug.Log(node);
    }

    public void stopMusic()
    {
        musicPlayer.Stop();
    }


}
