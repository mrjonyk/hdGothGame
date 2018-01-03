using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionMod : MonoBehaviour {

    private string[] availableObjects = { "cube", "stair" };
    private GameObject[] blockList;
    private Sprite[] blockIcons;
    private Sprite emptyIcon;
    private Transform[] inventorySlots;

    public Vector3 spawnOffset = Vector3.one;
    private Vector3 spawnPoint;
    private string triangle = "Jump";
    private string circle = "Fire2";

    public float forwardDistance = 1;
    private GameObject marker;

    public Canvas canvas;
    public int inventorySize = 10;

    // Use this for initialization
    void Start() {

        marker = Instantiate(Resources.Load<GameObject>("boxMarker"));
        marker.SetActive(false);

        blockList = new GameObject[availableObjects.Length];
        blockIcons = new Sprite[availableObjects.Length];

        emptyIcon = Resources.Load<Sprite>("empty_icon");

        inventorySlots = new Transform[inventorySize];

        for (int a =0; a<inventorySize; a++) {
            Transform invIcon = Instantiate(canvas.transform.Find("Image"), canvas.transform);

            invIcon.gameObject.SetActive(true);
            invIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(50+(a*50),50);

            inventorySlots[a] = Instantiate(invIcon, invIcon.parent);
            inventorySlots[a].GetComponent<Image>().sprite = emptyIcon;

            inventorySlots[a].transform.position += new Vector3(0, 0, 0.1f);
        }

        for (int a = 0; a < availableObjects.Length; a++) {
            blockList[a] = loadBlock(availableObjects[a]);
            blockIcons[a] = Resources.Load<Sprite>(availableObjects[a] + "_icon");

            inventorySlots[a].GetComponent<Image>().sprite = blockIcons[a];

        }

        inventorySlots[currentBlock].transform.localScale = new Vector3(1.5f,1.5f,1f);

    }

    private GameObject loadBlock(string blockName) {
        GameObject block = Instantiate(Resources.Load<GameObject>(blockName));

        Transform collider = block.transform.Find(blockName + "_collider");
        collider.gameObject.SetActive(false);
        MeshCollider colliderMesh = block.AddComponent<MeshCollider>();

        colliderMesh.sharedMesh = collider.GetComponent<MeshFilter>().sharedMesh;

        block.SetActive(false);
        return block;
    }
    // Update is called once per frame

    private Vector3 targetPosition = Vector3.zero;
    int currentBlock = 0;
    void Update () {

        if (Input.GetButtonDown("L1") && currentBlock < blockList.Length-1) {
            inventorySlots[currentBlock].transform.localScale = new Vector3(1f, 1f, 1f);
            currentBlock ++;
            inventorySlots[currentBlock].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            Debug.Log(currentBlock);
        }
        else {
            if (Input.GetButtonDown("R1") && currentBlock > 0) {
                inventorySlots[currentBlock].transform.localScale = new Vector3(1f, 1f, 1f);
                currentBlock--;
                inventorySlots[currentBlock].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                Debug.Log(currentBlock);
            }
        }

        Vector3 rayPosition = transform.position + (transform.forward * forwardDistance);


        if (marker.activeSelf) {
            Vector3 currPos = rayPosition;
            currPos.y += 10;
            RaycastHit hit;
            Debug.DrawRay(currPos,Vector3.down);
            if (Physics.Raycast(currPos, Vector3.down, out hit)) {
                targetPosition = new Vector3(Mathf.Round(hit.point.x), Mathf.Round(hit.point.y+0.01f), Mathf.Round(hit.point.z));
                
            }
            marker.transform.position = targetPosition;
            if (Input.GetButtonDown(circle)) {
                Instantiate(blockList[currentBlock], targetPosition, blockList[currentBlock].transform.rotation).SetActive(true);
            }
        }
        spawnPoint = transform.position + spawnOffset;

        if (Input.GetButtonDown(triangle)) {
            if (marker.activeSelf) {
                marker.SetActive(false);
            }
            else {
                marker.SetActive(true);
            }
            
        }
	}
}
