using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTraveling : MonoBehaviour
{
    [SerializeField]
    private float travelingSpeed = 20f;

    [SerializeField]
    private GameObject tunnelSegmentPrefab;
    [SerializeField]
    private GameObject tunnelSegmentPrefabLit;

    [SerializeField]
    private Vector3 frontSegmentStartPos;
    [SerializeField]
    private Vector3 midSegmentStartPos;
    [SerializeField]
    private Vector3 backSegmentStartPos;

    private Transform frontSegment;
    private Transform midSegment;
    private Transform backSegment;
    private int segmentCounter = 0;

    private bool starting = false;
    private bool stopping = false;

    [SerializeField]
    private Animator trainAnimator;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance().PlayMusic("train_ambience");
        frontSegment = Instantiate(tunnelSegmentPrefab, frontSegmentStartPos, Quaternion.identity).transform;
        midSegment = Instantiate(tunnelSegmentPrefab, midSegmentStartPos, Quaternion.identity).transform;
        backSegment = Instantiate(tunnelSegmentPrefab, backSegmentStartPos, Quaternion.identity).transform;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
            Stop();

        // starting and stopping
        if (stopping)
        {
            travelingSpeed -= Time.deltaTime * 2f;
            if (travelingSpeed <= 0)
            {
                StartCoroutine(OpenDoors());
                travelingSpeed = 0;
                stopping = false;
            }
        }
        else if (starting)
        {
            travelingSpeed += Time.deltaTime * 2f;
            if (travelingSpeed >= 20f)
            {
                travelingSpeed = 20f;
                starting = false;
            }
        }

        // moving segments along
        frontSegment.transform.Translate(Vector3.forward * travelingSpeed * Time.deltaTime);
        midSegment.transform.Translate(Vector3.forward * travelingSpeed * Time.deltaTime);
        backSegment.transform.Translate(Vector3.forward * travelingSpeed * Time.deltaTime);

        // spawn new segment
        if (frontSegment.transform.position.z > 40f)
        {
            // chance to trigger a screenshake
            if (Random.Range(0f, 1f) > 0.6f)
                StartCoroutine(ScreenShake());

            Destroy(frontSegment.gameObject);
            frontSegment = midSegment;
            midSegment = backSegment;
            // every fourth segment has lights
            if (segmentCounter % 4 == 0)
                backSegment = Instantiate(tunnelSegmentPrefabLit, backSegmentStartPos, Quaternion.identity).transform;
            else
                backSegment = Instantiate(tunnelSegmentPrefab, backSegmentStartPos, Quaternion.identity).transform;
            segmentCounter++;
        }
    }

    IEnumerator OpenDoors()
    {
        trainAnimator.SetBool("openDoors", true);
        AudioManager.Instance().PlaySound("open_door");
        yield return new WaitForSeconds(10f);
        AudioManager.Instance().PlaySound("open_door");
        trainAnimator.SetBool("openDoors", false);
        Continue();
    }

    IEnumerator ScreenShake()
    {
        if (stopping || starting)
            yield return 0;

        AudioManager.Instance().PlaySound("kachunk");
        Vector3 camPos = Camera.main.transform.localPosition;
        for (int i = 0; i < 5; i++)
        {
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x + Random.insideUnitCircle.x * 0.01f, Camera.main.transform.localPosition.y + Random.insideUnitCircle.y * 0.01f, camPos.z);
            yield return new WaitForSeconds(0.02f);
            Camera.main.transform.localPosition = camPos;
        }
    }

    public void Stop()
    {
        stopping = true;
        AudioManager.Instance().PlayMusic("empty");
    }

    public void Continue()
    {
        starting = true;
        AudioManager.Instance().PlayMusic("train_ambience");
    }
}
