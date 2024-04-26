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
    public bool spawned;

    public Node(float time, Drum drum)
    {
        this.time = time;
        this.drum = drum;
        this.spawned = false;
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

    [SerializeField]
    GameObject beatPrefab;

    [SerializeField]
    Transform drumHitLeft;

    [SerializeField]
    Transform drumHitRight;

    [SerializeField]
    GameObject drumHitLeftFX;

    [SerializeField]
    GameObject drumHitRightFX;

    [SerializeField]
    float beatSpeed = 3;

    [SerializeField]
    float beatSpawnTime = 10;

    private AudioSource musicPlayer;
    private AudioClip song;
    private bool recordMode;
    private bool playMode;
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
            float time = musicPlayer.time;
            int minute = (int) time / 60;
            int second = (int) time % 60;
            int ms = (int) ((time * 100) % 100);

            textTimer.text = $"{minute:00}:{second:00}:{ms:00}";

            // Playback Drum Sequence
            if (playMode && sequence.Count > 0)
            {
                Node node = sequence.First.Value;

                if (time >= node.time)
                {
                    if (node.drum == Drum.left)
                    {
                        //playDrumLeft();
                    }
                    else if (node.drum == Drum.right)
                    {
                        //playDrumRight();
                    }

                    sequence.RemoveFirst();
                }
            }

            // Spawm Drum Beat
            // Spawn All Nodes within 5 Seconds
            if (playMode && sequence.Count > 0)
            {
                
                foreach(Node node in sequence)
                {
                    if (node.time - time < beatSpawnTime)
                    {
                        if(node.spawned == false)
                        {
                            GameObject beatObj = GameObject.Instantiate(beatPrefab);
                            Beat beat = beatObj.GetComponent<Beat>();

                            if (node.drum == Drum.left)
                            {
                                beatObj.GetComponent<SpriteRenderer>().color = Color.red;
                                beat.setIntercept(drumHitLeft, beatSpeed, node.time - time);
                            }
                            else
                            {
                                beatObj.GetComponent<SpriteRenderer>().color = Color.yellow;
                                beat.setIntercept(drumHitRight, beatSpeed, node.time - time);
                            }

                            Node newNode = new Node(node.time, node.drum);
                            newNode.spawned = true;

                            sequence.Find(node).Value = newNode;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            if (recordMode && sequence != null && sequence.Count > 0)
            {
                recordMode = false;
                saveSequence();
            }

            if (playMode)
            {
                playMode = false;
                Debug.Log("--- Playback End ----");
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
        GameObject.Instantiate(drumHitLeftFX, drumHitLeft);

        if (recordMode)
        {
            float time = musicPlayer.time;
            Node node = new Node(time, Drum.left);
            sequence.AddLast(node);
            Debug.Log(node);
        }

    }

    public void playDrumRight()
    {
        drumRight.Play();
        GameObject.Instantiate(drumHitRightFX, drumHitRight);

        if (recordMode)
        {
            float time = musicPlayer.time;
            Node node = new Node(time, Drum.right);
            sequence.AddLast(node);
            Debug.Log(node);
        }
    }

    public void playMusic()
    {
        musicPlayer.Play();
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
        playMusic();
        playMode = true;
        
    }

}
