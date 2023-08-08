using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    [Header("Obstacles")]
    [SerializeField] BirdController bird;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] GameObject fullBlockPrefab;
    [SerializeField] float[] obstacleX = { -1.75f, 0.25f, 2.25f};
    List<GameObject> obstacles = new List<GameObject>();
    float currentZ;
    [SerializeField] float obstacleDistance = 20f;
    [SerializeField] float maxHeight = 6f;
    [SerializeField] float minHeight = 3f;
    [Header("Background")]
    [SerializeField] GameObject[] ground;
    [SerializeField] GameObject endWall;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI topListText;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button submitButton;
    int currentGround = 0;
    int currentScore = 0;
    float groundZ;
    float defaultGroundZ = 40f;
    Coroutine spawnRoutine;
    Coroutine cleanRoutine;
    static WorldManager _instance;
    public static WorldManager Instance{
        get{
            if (_instance == null){
                _instance = FindObjectOfType<WorldManager>();
            }
            return _instance;
        }
    }
    void Awake(){
        if (_instance == null) _instance = this;
        else Destroy(gameObject);
        submitButton.onClick.AddListener(SubmitScore);
    }

    void SubmitScore(){
        if (string.IsNullOrWhiteSpace(nameInput.text)) return;
        TopList.Add(nameInput.text, currentScore);
        TopList.Save();
        nameInput.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        topListText.text = TopList.GetList();
        nameInput.text = "";
    }
    void Start(){
        TopList.Load();
        topListText.text = "";
        obstacles = new List<GameObject>();
        currentZ = defaultGroundZ;
        groundZ = defaultGroundZ;
        spawnRoutine = StartCoroutine(SpawnObstacle());
        cleanRoutine = StartCoroutine(CleanLoop());
        currentGround = 0;
        currentScore = 0;
        scoreText.text = "Score: " + currentScore;
        nameInput.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
    }

    IEnumerator SpawnObstacle(){
        while (!bird.IsDead)
        {
            if (currentZ - bird.transform.position.z < 40f){
                int block = Random.Range(0, 3);
                bool doubleblock = Random.value > 0.75f;
                for (int i = 0; i < 3; i++)
                {
                    float randomY = Random.Range(minHeight, maxHeight);
                    Vector3 spawnPosition = new Vector3(obstacleX[i], randomY, currentZ);
                    var o = Instantiate((block == i && !doubleblock) || (doubleblock && block != i) ? fullBlockPrefab : obstaclePrefab, spawnPosition, Quaternion.identity);
                    obstacles.Add(o);    
                }
                
                currentZ += obstacleDistance;
            } 
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator CleanLoop(){
        while (!bird.IsDead)
        {
            if (obstacles.Count > 0 && obstacles[0] != null)
            {
                if (obstacles[0].transform.position.z < bird.transform.position.z - 10f)
                {
                    Destroy(obstacles[0]);
                    obstacles.RemoveAt(0);
                }
            }
            if (groundZ < bird.transform.position.z)
            {
                groundZ += 40f;
                ground[currentGround].transform.Translate(Vector3.forward * ground.Length * 40);
                endWall.transform.Translate(Vector3.forward * 40, Space.World);
                currentGround++;
                if (currentGround > ground.Length - 1) currentGround = 0;
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void Restart(){
        if(spawnRoutine != null) StopCoroutine(spawnRoutine);
        if (cleanRoutine != null) StopCoroutine(cleanRoutine);
        foreach(var o in obstacles){
            Destroy(o);
        }
        obstacles.Clear();
        for(int i = 0; i < ground.Length; i++){
            ground[i].transform.localPosition = new Vector3(0, 0, i * 40);
        }
        currentScore = 0;
        scoreText.text = currentScore.ToString();
        topListText.text = "";
        Start();
    }

    public void AddScore(){
        currentScore++;
        scoreText.text = currentScore.ToString();
    }
    public void BirdDead(){
        if (TopList.GetsOnList(currentScore)){
            topListText.text += "New Top Score!\n";
            nameInput.gameObject.SetActive(true);
            nameInput.text = "";
            submitButton.gameObject.SetActive(true);
        }
        TopList.Save();
        topListText.text += TopList.GetList();
    }
}
