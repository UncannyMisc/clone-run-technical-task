using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AlCaTrAzzGames.Utilities;

public class AestheticGenerator : Singleton<AestheticGenerator>
{
    public GameObject aestheticPrefab;
    public Vector2 yRange;
    public float xDistanceInFront;

    public Vector2 sizeRange;
    public Vector2 rotationRange;

    public Vector2 creationRangeBounds;

    [SerializeField]
    private float currentCreationRange;
    [SerializeField]
    private float travelledDistance;
    [SerializeField]
    private float lastLocation;

    //TODO, would be better to have a pool of these and recycle them
    Queue<GameObject> aestheticSquares = new Queue<GameObject>();
    float aestheticBehindToRegenDistance = 15f;

    bool reset = false;

    void Start(){
        Reset();
    }

    void Update(){
        if(activePlayer == null||reset){
            reset = false;
            return;
        }

        float thisDistance = activePlayer.transform.position.x - lastLocation;
        travelledDistance += thisDistance;

        if(travelledDistance > currentCreationRange){
            GenerateAesthetic();
            travelledDistance -= currentCreationRange;
        }

        lastLocation = activePlayer.transform.position.x;

        CleanAestheticSquares();
    }


    Player activePlayer;

    void GenerateAesthetic(){
        currentCreationRange = Random.Range(creationRangeBounds.x, creationRangeBounds.y);

        GameObject newAesthetic = GameObject.Instantiate(aestheticPrefab);
        aestheticSquares.Enqueue(newAesthetic);
        newAesthetic.transform.SetParent(transform);
        newAesthetic.transform.position = new Vector3(lastLocation + xDistanceInFront, Random.Range(yRange.x, yRange.y), 0f);

        float size = Random.Range(sizeRange.x, sizeRange.y);
        newAesthetic.transform.localScale = new Vector3(size, size, size);

        float rot = Random.Range(rotationRange.x, rotationRange.y);
        newAesthetic.transform.localRotation = Quaternion.Euler(0f, 0f, rot);
    }

    void CleanAestheticSquares()
    {
        for(int i = 0; i < aestheticSquares.Count; i++)
        {
            if(aestheticSquares.Peek().transform.position.x < (activePlayer.transform.position.x - aestheticBehindToRegenDistance))
            {
                GameObject lastsquare = aestheticSquares.Dequeue();
                Destroy(lastsquare);
            }
        }

    }

    // this needs to be hooked up to the gameover screen
    public IEnumerator CleanAllAestheticSquaresAfterDelay(float delay = 0.35f)
    {
        yield return new WaitForSeconds(delay);

        CleanAllAestheticSquares();
    }

    void CleanAllAestheticSquares()
    {
        while(aestheticSquares.Count>0){
            GameObject lastsquare = aestheticSquares.Dequeue();
            Destroy(lastsquare);
        }
    }

    public void SetActivePlayer(Player p){
        activePlayer = p;
        lastLocation = activePlayer.transform.position.x;
    }

    public void Reset(){
        reset = true;
        CleanAllAestheticSquares();
        lastLocation = 0;
        travelledDistance = 0;
        currentCreationRange = Random.Range(creationRangeBounds.x, creationRangeBounds.y);
    }

}
