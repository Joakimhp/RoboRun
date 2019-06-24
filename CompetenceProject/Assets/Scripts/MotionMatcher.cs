using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MotionMatcher : MonoBehaviour {

    [Header("Motionstate settings")]
    public bool generateHashtables = false;
    public int frames_IDLE = 30;
    public int frames_WALK = 40;
    public int frames_RUN = 37;
    //public int frames_FORWARD;
    //public int frames_LEFT;
    //public int frames_RIGHT;
    //public int frames_BACK;

    //private Rigidbody player_rb;
    private CharacterController player_cc;
    private PlayerMove player_movement;
    private Animator anim;
    private int framerate = 30;

    private char currentState = 'i';
    private char previousState = 'i';

    [Header("Animation settings")]
    public float idleThreshold = 0.08f;
    public float crossFadeTime = 0.2f;
    public bool isAttacking = false;
    public bool isDying;

    private Hashtable ht_IDLE = new Hashtable();
    private Hashtable ht_WALK = new Hashtable();
    private Hashtable ht_RUN = new Hashtable();
    //private Hashtable ht_FORWARD = new Hashtable();
    //private Hashtable ht_LEFT = new Hashtable();
    //private Hashtable ht_RIGHT = new Hashtable();
    //private Hashtable ht_BACK = new Hashtable();

    //private Hashtable clipLengths;

    private void Awake() {
        if (FindObjectsOfType<MotionMatcher>().Length > 1) {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start() {
        if (generateHashtables) {
            GenerateHashtables();
        }

        ReadHashtableFromFile("hashtables.bin");

        anim.speed = 1;
        anim.Play("idle");
    }

    private void FixedUpdate() {
        if (isAttacking || isDying)
            return;

        if (player_cc != null) {
            if (player_cc.velocity.magnitude < idleThreshold)
                currentState = 'i';
            else if (player_movement.isSprinting) {
                currentState = 'r';
            }
            else {
                currentState = 'w';
            }

            if (currentState != previousState) { //now we switch to a different animation
                SwitchAnimation(currentState, previousState);
            }

            previousState = currentState;
        }
    }

    public void ForceCurrentAnimation() {
        anim.CrossFade(GetNameForAnimation(currentState), 0.2f);
    }

    private void SwitchAnimation(char currentState, char previousState) {
        //Find out which frame we're currently at
        float normTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
        int currentFrame = (int)(normTime * GetTotalFramesForAnimation(previousState));

        if (currentFrame < 1) {
            currentFrame = 1;
        }

        //Find the corresponding frame in the animation and play that
        string currentMotionState = previousState.ToString() + currentFrame.ToString();
        string similarMotionState = (string)GetHashtableForAnimation(currentState)[currentMotionState];
        int similarMotionState_frame;

        //print("Current[" + currentMotionState + "], similar[" + similarMotionState + "]");

        try {
            similarMotionState_frame = System.Convert.ToInt32(similarMotionState.Substring(1));
        }
        catch (System.NullReferenceException e) {
            similarMotionState_frame = 1;
            //Debug.LogError("Caught motion state error, set similar frame = 1");
        }

        anim.CrossFade(GetNameForAnimation(currentState), crossFadeTime, 0, (float)similarMotionState_frame / (float)GetTotalFramesForAnimation(currentState));

        //print("Switched from " + currentMotionState + " to " + similarMotionState);
    }

    private void GenerateHashtables() {
        List<MotionState> ms_IDLE = GenerateMotionStates("idle");
        List<MotionState> ms_WALK = GenerateMotionStates("walking");
        List<MotionState> ms_RUN = GenerateMotionStates("running");
        //List<MotionState> ms_FORWARD = GenerateMotionStates("forward");
        //List<MotionState> ms_LEFT = GenerateMotionStates("left");
        //List<MotionState> ms_RIGHT = GenerateMotionStates("right");
        //List<MotionState> ms_BACK = GenerateMotionStates("back");
        List<MotionState> ms_ALL = new List<MotionState>();

        ms_ALL.AddRange(ms_IDLE);
        ms_ALL.AddRange(ms_WALK);
        ms_ALL.AddRange(ms_RUN);
        //ms_ALL.AddRange(ms_FORWARD);
        //ms_ALL.AddRange(ms_LEFT);
        //ms_ALL.AddRange(ms_RIGHT);
        //ms_ALL.AddRange(ms_BACK);

        foreach (MotionState m in ms_ALL) {
            if (m.anim_id != 'i') {
                FillOutHashtable(m, ms_IDLE, ht_IDLE);
            }

            if (m.anim_id != 'w') {
                FillOutHashtable(m, ms_WALK, ht_WALK);
            }

            if (m.anim_id != 'r') {
                FillOutHashtable(m, ms_RUN, ht_RUN);
            }

            /*if (m.anim_id != 'f') {
                FillOutHashtable(m, ms_FORWARD, ht_FORWARD);
            }

            if (m.anim_id != 'l') {
                FillOutHashtable(m, ms_LEFT, ht_LEFT);
            }

            if (m.anim_id != 'r') {
                FillOutHashtable(m, ms_RIGHT, ht_RIGHT);
            }

            if (m.anim_id != 'b') {
                FillOutHashtable(m, ms_BACK, ht_BACK);
            }*/
        }

        string path = Application.streamingAssetsPath + "\\" + "hashtables.bin";
        if (File.Exists(path))
            File.Delete(path);
        FileStream fs = File.Create(path);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, ht_IDLE);
        bf.Serialize(fs, ht_WALK);
        bf.Serialize(fs, ht_RUN);
        //bf.Serialize(fs, ht_FORWARD);
        //bf.Serialize(fs, ht_LEFT);
        //bf.Serialize(fs, ht_RIGHT);
        //bf.Serialize(fs, ht_BACK);

        fs.Close();
    }

    private void ReadHashtableFromFile(string path) {
        string totalPath = Application.streamingAssetsPath + "\\" + path;
        WWW file = new WWW(totalPath);

        //FileStream fs = File.Open(path, FileMode.Open);
        MemoryStream fs = new MemoryStream(file.bytes);
        BinaryFormatter bf = new BinaryFormatter();

        ht_IDLE = (Hashtable)bf.Deserialize(fs);
        ht_WALK = (Hashtable)bf.Deserialize(fs);
        ht_RUN = (Hashtable)bf.Deserialize(fs);
        //ht_FORWARD = (Hashtable)bf.Deserialize(fs);
        //ht_LEFT = (Hashtable)bf.Deserialize(fs);
        //ht_RIGHT = (Hashtable)bf.Deserialize(fs);
        //ht_BACK = new Hashtable();

        fs.Close();
    }

    private void FillOutHashtable(MotionState m, List<MotionState> candidateStates, Hashtable hashTable) {
        //calculate the dissimilarities between this and all motion states in the 'idle'-animation
        float[] dissimilarities = new float[candidateStates.Count];
        for (int i = 0; i < candidateStates.Count; i++) {
            dissimilarities[i] = CalculateDissimilarity(m, candidateStates[i]);
        }

        //then find the state with the smallest dissimilarity
        int indexOfLeastDissimilarity = dissimilarities.Length;
        float smallestDissimilarity = Mathf.Infinity;
        for (int i = 0; i < dissimilarities.Length; i++) {
            if (dissimilarities[i] < smallestDissimilarity) {
                indexOfLeastDissimilarity = i;
                smallestDissimilarity = dissimilarities[i];
            }
        }

        //add this motion state and the motion state that was picked as being the most similar (=smallest dissimilarity) to the IDLE-hash
        string key = m.anim_id.ToString() + m.frame.ToString();
        string value = candidateStates[indexOfLeastDissimilarity].anim_id.ToString() + candidateStates[indexOfLeastDissimilarity].frame.ToString();
        hashTable.Add(key, value);
    }

    private float CalculateDissimilarity(MotionState current, MotionState candidate) {
        float result = Mathf.Infinity;

        float beta_root = 0.5f;
        float beta_zero = 0.5f;

        float piece_1 = beta_root * Mathf.Pow((current.velocity.root_pos - candidate.velocity.root_pos).magnitude, 2f);
        float piece_2 = beta_zero * Mathf.Pow((current.velocity.root_rot * Vector3.forward - candidate.velocity.root_rot * Vector3.forward).magnitude, 2f);
        float piece_3 = 0f;
        float piece_4 = 0f;


        for (int i = 0; i < current.pose.joint_rots.Length; i++) {
            float weight = 0f;
            try {
                weight = (current.pose.joint_pos[i] - current.pose.joint_pos[i + 1]).magnitude;
            }
            catch (System.IndexOutOfRangeException e) {

            }

            piece_3 += weight * Mathf.Pow((current.pose.joint_rots[i] * Vector3.forward - candidate.pose.joint_rots[i] * Vector3.forward).magnitude, 2f);
            piece_4 += weight * Mathf.Pow((((current.velocity.joint_rots[i] * current.pose.joint_rots[i]) * Vector3.forward) - ((candidate.velocity.joint_rots[i] * candidate.pose.joint_rots[i]) * Vector3.forward)).magnitude, 2f);
        }

        /*print("1: " + piece_1);
        print("2: " + piece_2);
        print("3: " + piece_3);
        print("4: " + piece_4);
        */

        result = Mathf.Sqrt(piece_1 + piece_2 + piece_3 + piece_4);

        /*if (current.anim_id == 'i' && candidate.anim_id == 's') {
            print("1[" + piece_1 + "], 2[" + piece_2 + "], 3[" + piece_3 + "], 4[" + piece_4 + "]");
        }*/

        return result;
    }

    private List<MotionState> GenerateMotionStates(string clip) {
        List<MotionState> motionStates = new List<MotionState>();

        anim.Play(clip);
        anim.speed = 1;
        anim.Update(1);
        float clipLength = anim.GetCurrentAnimatorStateInfo(0).length;

        int totalFrames = (int)(clipLength * framerate);
        char anim_id = clip[0];

        for (int frame = 1; frame <= totalFrames; frame++) {
            MotionState m = new MotionState();
            Pose p = ConstructPose(clip, frame, totalFrames);
            Velocity v = new Velocity();

            m.anim_id = anim_id;
            m.frame = frame;

            //CONSTRUCTING THE VELOCITY V
            Pose p1 = ConstructPose(clip, frame + 1, totalFrames); //NOTE: This could be problematic when we reach the last frame (because there's no "frame + 1")
            v.root_pos = p1.root_pos - p.root_pos;
            v.root_rot = p1.root_rot * Quaternion.Inverse(p.root_rot);
            v.joint_rots = new Quaternion[p.joint_rots.Length];

            for (int i = 0; i < v.joint_rots.Length; i++) {
                v.joint_rots[i] = p1.joint_rots[i] * Quaternion.Inverse(p.joint_rots[i]);
            }

            //SAVING THE MOTION STATE
            m.pose = p;
            m.velocity = v;

            motionStates.Add(m);
        }

        return motionStates;
    }

    private Pose ConstructPose(string clip, int frame, int totalFrames) {
        Pose p = new Pose();
        //Transform hips = GameObject.FindWithTag("PlayerHips").transform;
        Transform hips = anim.GetBoneTransform(HumanBodyBones.Hips);

        anim.Play(clip, 0, (float)(frame) / (float)(totalFrames));
        anim.speed = 1;
        anim.Update(1);

        //p.root_pos = anim.GetBoneTransform(HumanBodyBones.Hips).position;
        //p.root_rot = anim.GetBoneTransform(HumanBodyBones.Hips).rotation;

        p.root_pos = hips.position;
        p.root_rot = hips.rotation;

        //Transform[] transforms = anim.GetBoneTransform(HumanBodyBones.Hips).gameObject.GetComponentsInChildren<Transform>();
        Transform[] transforms = hips.gameObject.GetComponentsInChildren<Transform>();
        List<Vector3> positions = new List<Vector3>();
        List<Quaternion> rotations = new List<Quaternion>();
        foreach (Transform t in transforms) {
            positions.Add(t.localPosition);
            rotations.Add(t.localRotation);
        }
        positions.RemoveAt(0);
        rotations.RemoveAt(0);

        p.joint_pos = positions.ToArray();
        p.joint_rots = rotations.ToArray();

        return p;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (player_cc == null) {
            print("Finding new player prefab from tag \"Player\"...");
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) {
                player_cc = player.GetComponent<CharacterController>();
                player_movement = player.GetComponent<PlayerMove>();
                anim = player.GetComponentInChildren<Animator>();
            }
            else {
                Debug.LogError("Could not find player object in scene!");
            }
        }
    }

    /*
	void Start () {
        Hashtable ht = new Hashtable();

        ht.Add("i1", "f367");
        ht.Add("l58", "r2002");

        string path = "testHashTable.bin";
        if (File.Exists(path)) //delete file if it already exists
            File.Delete(path);
        FileStream fs = File.Create(path);
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(fs, ht);

        print("Serialized hashtable test...");
        fs.Close();

        //DESERIALIZATION
        Hashtable deserialized = new Hashtable();
        fs = File.Open(path, FileMode.Open);
        deserialized = (Hashtable)bf.Deserialize(fs);

        print(deserialized["i1"]);
        print(deserialized["0"]);
	}*/

    #region Switch-functions for getting correct animation name/framecount/hashtable
    private string GetNameForAnimation(char anim_id) {
        switch (anim_id) {
            case 'i':
                return "idle";
            case 'w':
                return "walking";
            case 'r':
                return "running";
            /*case 'f':
                return "forward";
            case 'l':
                return "left";
            case 'r':
                return "right";
            case 'b':
                return "back";*/
            default:
                Debug.LogError("Invalid anim_id in GetTotalFramesForAnimation(anim_id)");
                return "idle";
        }
    }

    private Hashtable GetHashtableForAnimation(char anim_id) {
        switch (anim_id) {
            case 'i':
                return ht_IDLE;
            case 'w':
                return ht_WALK;
            case 'r':
                return ht_RUN;
            /*case 'f':
                return ht_FORWARD;
            case 'l':
                return ht_LEFT;
            case 'r':
                return ht_RIGHT;*/
            default:
                Debug.LogError("Invalid anim_id in GetTotalFramesForAnimation(anim_id)");
                return ht_IDLE;
        }
    }

    private int GetTotalFramesForAnimation(char anim_id) {
        switch (anim_id) {
            case 'i':
                return frames_IDLE;
            case 'w':
                return frames_WALK;
            case 'r':
                return frames_RUN;
            /*case 'f':
                return frames_FORWARD;
            case 'l':
                return frames_LEFT;
            case 'r':
                return frames_RIGHT;*/
            default:
                Debug.LogError("Invalid anim_id in GetTotalFramesForAnimation(anim_id)");
                return frames_IDLE;
        }
    }
    #endregion

    #region Custom structs
    public struct MotionState
    {
        public char anim_id;
        public int frame;
        public Pose pose;
        public Velocity velocity;
    }

    public struct Pose
    {
        public Vector3 root_pos;
        public Quaternion root_rot;
        public Vector3[] joint_pos;
        public Quaternion[] joint_rots;
    }

    public struct Velocity
    {
        public Vector3 root_pos;
        public Quaternion root_rot;
        public Quaternion[] joint_rots;
    }
    #endregion
}
