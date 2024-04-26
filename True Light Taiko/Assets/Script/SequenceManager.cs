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
    private bool recordMode;
    private LinkedList<Node> sequence;

    
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        textTimer.text = ""+0;

        song = Resources.Load<AudioClip>("Music/Satisfaction");
        Debug.Log(song.name);
        musicPlayer.clip = song;
        recordMode = false;

    }

    public void RecordSequence()
    { 
        playMusic();
        sequence = new LinkedList<Node>();
        recordMode = true;
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
            if (sequence != null && sequence.Count > 0)
            {
                saveSequence();
                sequence = null;
                loadSequence(song.name+".txt");
                printSequence();
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

        if (recordMode)
        {
            float time = Time.fixedTime - T0;
            Node node = new Node(time, Drum.left);
            sequence.AddLast(node);
            Debug.Log(node);
        }

    }

    public void playDrumRight()
    {
        drumRight.Play();

        if (recordMode)
        {
            float time = Time.fixedTime - T0;
            Node node = new Node(time, Drum.right);
            sequence.AddLast(node);
            Debug.Log(node);
        }
    }

    public void playMusic()
    {
        musicPlayer.Play();
        T0 = Time.fixedTime;
    }

    public void stopMusic()
    {
        musicPlayer.Stop();
    }

    public void saveSequence()
    {
        if (sequence != null && sequence.Count > 0)
        {
            StreamWriter writer = new StreamWriter($"{song.name}.txt");

            Debug.Log("------ Save Sequence ------");
            foreach (Node node in sequence)
            {
                // Debug.Log(node);
                writer.WriteLine(node);
            }

            writer.Close();

            Debug.Log("---- Sequence Saved! -----");
        }
        else
        {
            Debug.Log("Sequence is Null!");
        }
    }

    public void loadSequence(string fileName)
    {
        StreamReader reader = new StreamReader(fileName);
        sequence = new LinkedList<Node>();

        Debug.Log("----- Load Sequence -----");
        while (reader.Peek() > 0)
        {
            string tmp = reader.ReadLine();
            string[] str = tmp.Split('-');

            float time = float.Parse(str[0]);
            Drum drum = (Drum)System.Enum.Parse(typeof(Drum), str[1]);

            Node node = new Node(time, drum);

            sequence.AddLast(node);
        }
        reader.Close();
        Debug.Log("------ End -------");
    }

    public void printSequence()
    {
        if (sequence != null && sequence.Count > 0)
        {

            Debug.Log("------ Print Sequence ------");

            foreach (Node node in sequence)
            {
                Debug.Log(node);
            }

            Debug.Log("---- End -----");
        }
        else
        {
            Debug.Log("Sequence is Null!");
        }
    }


    public void playBack()
    {
        Debug.Log("----- PLAYBACK -----");
        loadSequence(song.name + ".txt");
        
        
    }

}
