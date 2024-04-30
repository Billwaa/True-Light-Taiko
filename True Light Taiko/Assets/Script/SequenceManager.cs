using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.Video;

public enum Drum
{
    left,
    right
}

public class Node {

    public float time;
    public Drum drum;
    public bool spawned;
    public Beat beat;

    public Node(float time, Drum drum)
    {
        this.time = time;
        this.drum = drum;
        this.spawned = false;
        beat = null;
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
    TMP_Text textScore;

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
    GameObject videoBackground;

    [SerializeField]
    float beatSpeed = 3;

    [SerializeField]
    float beatSpawnTime = 10;

    [SerializeField]
    float beatDetectionTime = 0.25f;

    [SerializeField]
    float perfectScoreTime = 0.05f;

    public AudioSource musicPlayer;
    private AudioClip song;
    private bool recordMode;
    private bool playMode;
    private LinkedList<Node> sequence;
    private LinkedList<Node> sequence2;
    private LinkedList<Node> sequenceLeft;
    private LinkedList<Node> sequenceRight;

    private VideoPlayer videoPlayer;

    private float score;
    
    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        textTimer.text = ""+0;

        videoPlayer = videoBackground.GetComponent<VideoPlayer>();
        //videoPlayer.clip = Resources.Load<VideoClip>("Video/Website-promo-2-16-5");
        videoBackground.GetComponent<SpriteRenderer>().enabled = false;

        //song = Resources.Load<AudioClip>("Music/True Light March");
        //Debug.Log(song.name);
        //musicPlayer.clip = song;
        ChangeSong("Satisfaction");

        recordMode = false;

    }


    public void ChangeSong(int num)
    {
        Debug.Log(num);

        switch (num)
        {
            case 0:
                ChangeSong("Satisfaction");
                break;
            case 1:
                ChangeSong("True Light March");
                break;
            case 2:
                ChangeSong("True Light Girls");
                break;
            case 3:
                ChangeSong("Ha Ha True Light");
                break;
        }
    }

    public void ChangeSong(string songName)
    {


        song = Resources.Load<AudioClip>($"Music/{songName}");
        Debug.Log(song.name);
        musicPlayer.clip = song;
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
 
            if (playMode && sequence2.Count > 0)
            {
                Node node = sequence2.First.Value;

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

                    sequence2.RemoveFirst();
                }
            }

            // Spawm Drum Beat
            // Spawn All Nodes within 3 Seconds
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
                            beat.manager = this;
                            beat.hitTime = node.time;

                            if (node.drum == Drum.left)
                            {
                                beatObj.GetComponent<SpriteRenderer>().color = Color.red;
                                beat.setIntercept(drumHitLeft, beatSpeed, node.time - time);

                                foreach (Node node2 in sequenceLeft)
                                {
                                    if (node.time == node2.time)
                                    {
                                        Node nodeT = new Node(node2.time, node2.drum);
                                        nodeT.spawned = true;
                                        nodeT.beat = beat;
                                        sequenceLeft.Find(node2).Value = nodeT;
                                        break;
                                    }
                                }

                            }
                            else
                            {
                                beatObj.GetComponent<SpriteRenderer>().color = Color.yellow;
                                beat.setIntercept(drumHitRight, beatSpeed, node.time - time);

                                foreach (Node node2 in sequenceRight)
                                {
                                    if (node.time == node2.time)
                                    {
                                        Node nodeT = new Node(node2.time, node2.drum);
                                        nodeT.spawned = true;
                                        nodeT.beat = beat;
                                        sequenceRight.Find(node2).Value = nodeT;
                                        break;
                                    }
                                }
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


            // Shred Missed Beat
            if (playMode)
            {
                while (sequenceLeft.Count > 0)
                {
                    Node node = sequenceLeft.First.Value;

                    if (time - node.time > beatDetectionTime / 2)
                    {
                        sequenceLeft.RemoveFirst();
                    }
                    else
                    {
                        break;
                    }

                }

                while (sequenceRight.Count > 0)
                {
                    Node node = sequenceRight.First.Value;

                    if (time - node.time > beatDetectionTime / 2)
                    {
                        sequenceRight.RemoveFirst();
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

            videoBackground.GetComponent<SpriteRenderer>().enabled = false;

        }


        // Detect Key Hit

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            playDrumLeft();

        }

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            playDrumRight();
        }

        
        
    }

    public void calcScore(Node node, float time)
    {
        float halfTime = beatDetectionTime / 2.0f;
        float dTime = Mathf.Abs(time - node.time);

        float scoreRatio = 0;

        if (dTime < perfectScoreTime)
        {
            scoreRatio = 1;
        }
        else
        {
            scoreRatio = (halfTime - dTime + perfectScoreTime) / halfTime;
        }

        float dScore = scoreRatio / sequence.Count * 100.0f;
        
        score += dScore;
        Debug.Log($"Hit! +{dScore:0.0}% = {score:.0}%");
        textScore.text = $"{score:.0}%";

        
    }

    public void playDrumLeft()
    {

        float time = musicPlayer.time;

        drumLeft.Play();
        GameObject.Instantiate(drumHitLeftFX, drumHitLeft);


        if (recordMode)
        {
            Node node = new Node(time, Drum.left);
            sequence.AddLast(node);
            Debug.Log(node);
        }

        if (playMode && sequenceLeft.Count > 0)
        {
            Node node = sequenceLeft.First.Value;

            if (Mathf.Abs(time - node.time) < beatDetectionTime / 2)
            { 
                calcScore(node, time);
                sequenceLeft.First.Value.beat.GetComponent<SpriteRenderer>().color = Color.grey;
                sequenceLeft.RemoveFirst();
            }
        }

    }

    public void playDrumRight()
    {

        float time = musicPlayer.time;

        drumRight.Play();
        GameObject.Instantiate(drumHitRightFX, drumHitRight);


        if (recordMode)
        {
            Node node = new Node(time, Drum.right);
            sequence.AddLast(node);
            Debug.Log(node);
        }

        if (playMode && sequenceRight.Count > 0)
        {
            Node node = sequenceRight.First.Value;

            if (Mathf.Abs(time - node.time) < beatDetectionTime/2)
            {
                calcScore(node, time);
                sequenceRight.First.Value.beat.GetComponent<SpriteRenderer>().color = Color.grey;
                sequenceRight.RemoveFirst();
            }
        }
    }

    public void playMusic()
    {
        musicPlayer.Play();
    }

    public void stopMusic()
    {
        musicPlayer.Stop();
        videoBackground.GetComponent<SpriteRenderer>().enabled = false;

    }

    public void saveSequence()
    {
        if (sequence != null && sequence.Count > 0)
        {
            //string path = "Assets/Resources/Sequence/" + song.name + ".txt";
            string path = "Ext/" + song.name + ".txt";
            Debug.Log(path);
            
            StreamWriter writer = new StreamWriter(path);

            Debug.Log("------ Save Sequence ------");
            foreach (Node node in sequence)
            {
                // Debug.Log(node);
                writer.WriteLine(node);
            }

            writer.Close();

            Debug.Log("---- Sequence Saved! -----");
            Debug.Log("NEED TO RESTART GAME TO LOAD SEQUENCE... SORRY THIS IS THE ONLY FOR WEBGL TO WORK. A BIT MORE WORK TO LOAD FROM RESOURCES.");
        }
        else
        {
            Debug.Log("Sequence is Null!");
        }
    }

    public void loadSequence(string fileName)
    {
        //Debug.Log($"Sequence /{song.name}");
        //TextAsset sequenceRaw = Resources.Load<TextAsset>($"Sequence/{song.name}");
        //Debug.Log(sequenceRaw.text);
        //string[] raw = sequenceRaw.text.Split("\n");

        string path = Path.GetFullPath(".") + "/Ext/" + fileName;
        Debug.Log(path);
        StreamReader reader = new StreamReader(path);
        sequence = new LinkedList<Node>();

        Debug.Log("----- Load Sequence -----");
        while (reader.Peek() > 0)
        //for (int i = 0; i < raw.Length; i++)
        {
            string tmp = reader.ReadLine();
            //string tmp = raw[i];
            Debug.Log(tmp);
            string[] str = tmp.Split('-');

            if (str.Length == 2)
            {
                float time = float.Parse(str[0]);
                Drum drum = (Drum)System.Enum.Parse(typeof(Drum), str[1]);

                Node node = new Node(time, drum);

                sequence.AddLast(node);
            }
        }
        //reader.Close();
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
        videoBackground.GetComponent<SpriteRenderer>().enabled = true;

        // Load Sequence
        sequence2 = new LinkedList<Node>();
        sequenceLeft = new LinkedList<Node>();
        sequenceRight = new LinkedList<Node>();

        foreach (Node node in sequence)
        {
            sequence2.AddLast(node);

            if (node.drum == Drum.left)
            {
                sequenceLeft.AddLast(node);
            }
            else
            {
                sequenceRight.AddLast(node);
            }
        }

        // Reset Score
        score = 0;
        textScore.text = $"{score:.0}%";



    }

}
