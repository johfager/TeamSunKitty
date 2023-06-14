using System.Collections;
using System.Collections.Generic;
using PowerScript;
using PowerTools.Quest;
using UnityEditor;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GameObject PiecesHolder;
    public GameObject[] Pieces;
    private bool _winConditionTriggered = false;

    [SerializeField]
    public GameObject PopItem_key;  //make object available

    [SerializeField]
    public int totalPieces =0;
    
    [SerializeField]
    public int numCompleted =0;
    [SerializeField] private AudioCue puzzleWinSound; //will update to check if registered correct placement


    // Start is called before the first frame update
    private void Start()
    {
        //Make count of how many pieces we drag into the scen as "pieces" in the holder
        totalPieces=PiecesHolder.transform.childCount;

        Pieces= new GameObject[totalPieces]; // will autofill on play

        for( int i=0; i <Pieces.Length; i++)
        {
            Pieces[i]= PiecesHolder.transform.GetChild(i).gameObject;
        }

    }

    // Update is called once per frame
    private void Update()
    {

        //Check win condition
        if (numCompleted>=totalPieces & !_winConditionTriggered)
        {   
            _winConditionTriggered = true;
            StartCoroutine(HandleWinCondition());        }
        else
        {
            PopItem_key.SetActive(false); //revert automatically hide key
        }
            
    }
    private IEnumerator HandleWinCondition() {
        SystemAudio.Play(puzzleWinSound);
        yield return new WaitForSeconds(1);
        QuestScript.Globals.m_runePuzzleFinishedBedRoom = true;
        C.Player.Room = R.Bedroom;
        //key reward appear at Pop layer
    }
}
