using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    Rigidbody rb;
    [Header("Movement")]
    [SerializeField] int speed = 10;
    [SerializeField] float jumpForce = 300f;
    [SerializeField] float[] lanes = { -2f, 0f, 2f };
    [SerializeField] int currentLane = 1; 
    public int CurrentLane => currentLane;
    bool isDead = false; public bool IsDead => isDead;
    [SerializeField] bool isChangingLanes = false; 
    public bool IsChangingLanes => isChangingLanes;
    [Header("Misc")]
    [SerializeField] Vector3 SpawnPos;
    [SerializeField] Vector3 CameraPos;
    [SerializeField] ParticleSystem jumpParticle;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Obstacle")
        {
            isDead = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            Camera.main.gameObject.transform.parent = null;
            Vector3 randomDir = Random.value > 0.5f ? Vector3.left : Vector3.right;
            rb.AddExplosionForce(10f, other.transform.position-2*randomDir, 10f, 1f, ForceMode.Impulse);
            Debug.Log("Collided with obstacle " + other.gameObject.name);
            WorldManager.Instance.BirdDead();
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Obstacle")
        {
            WorldManager.Instance.AddScore();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        transform.Translate(Vector3.forward*speed*Time.deltaTime);
    }

    IEnumerator MoveToLane(int lane)
    {
        // Vector3 targetPosition = transform.position;
        // targetPosition.x = lanes[lane];
        // transform.position = targetPosition;
        while (Mathf.Abs(lanes[lane] - transform.position.x) > 0.01f)
        {
            isChangingLanes = true;
            Vector3 targetPosition = transform.position;
            targetPosition.x = Mathf.MoveTowards(transform.position.x, lanes[lane], 10f * Time.deltaTime);
            transform.position = targetPosition;
            yield return new WaitForFixedUpdate();
        }
        isChangingLanes = false;
        currentLane = lane;
        yield return null;
    }

    public void Jump(){
        rb.AddForce(Vector3.up * jumpForce);
        jumpParticle.Play();
    }
    public void MoveLeft(){
        if (currentLane > 0 && !isChangingLanes)
        {
            StartCoroutine(MoveToLane(currentLane - 1));
        }
    }
    public void MoveRight(){
        if (currentLane < lanes.Length - 1 && !isChangingLanes)
        {
            StartCoroutine(MoveToLane(currentLane + 1));
        }
    }
    public void Restart(){
        if (!isDead) return;
        isDead = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionZ;
        transform.position = SpawnPos;
        transform.rotation = Quaternion.identity;
        currentLane = 1;
        isChangingLanes = false;
        Camera.main.transform.parent = transform;
        Camera.main.transform.localPosition = CameraPos;
        WorldManager.Instance.Restart();
    }
}
