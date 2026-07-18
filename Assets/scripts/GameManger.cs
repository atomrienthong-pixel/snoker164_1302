using UnityEngine;

public class GameManger : MonoBehaviour
{
    [SerializeField]
    private int playerscore;
    public int PlayerScore { get { return playerscore; } set { playerscore = value; } }

    public static GameManger instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
