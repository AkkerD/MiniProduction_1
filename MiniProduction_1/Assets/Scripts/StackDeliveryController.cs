﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackDeliveryController : Manager<StackDeliveryController> {
    [SerializeField]
    GameObject Stacks;
    int[] stackCombinations;
    int stackHomesFound = 0;
    [SerializeField]
	GameObject[] colliders;
    List<GameObject> childStacks;    


    GameObject chosenStack = null;
    GameObject receivingSleeve = null;

    Plane objPlane;
    Vector3 m0;
    Vector3 startPosition;

    public LayerMask raycastStackSelection;
    public LayerMask raycastSleeveSelection;
    public bool stacksCreated;

    private void Start()
    {
        childStacks = new List<GameObject>();
        stacksCreated = false;
    }

    void Update()
    {
        moveStacksOnScreen();
    }

    Ray GenerateMouseRay()
    {
        Vector3 mousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 mousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);

        Vector3 mousePosF = Camera.main.ScreenToWorldPoint(mousePosFar);
        Vector3 mousePosN = Camera.main.ScreenToWorldPoint(mousePosNear);

        return new Ray(mousePosN, mousePosF - mousePosN);
    }

    public void CombinedStackWithSleeve(int stackNumber, int sleeveNumber)
    {
        stackCombinations[stackNumber] = sleeveNumber;
        stackHomesFound++;
        if (stackHomesFound == stackCombinations.Length)
        {
            AllStackCombined();
        }
    }
    void AllStackCombined()
    {
        ScoringController.Instance.CalculateReport(stackCombinations);
    }

    public void ShowStacks()
    {
        Debug.Log("ShowStacks");
        stackCombinations = new int[ContractController.Instance.GetCurrentContract().GetNumberOfStacks()];
        int stackHomesFound = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].SetActive(false);
        }                

        for (int i = 0; i < ContractController.Instance.GetCurrentContract().GetNumberOfStacks(); i++)
        {
            GameObject stack = (GameObject)Instantiate(Resources.Load("Stack"));
            stack.name = "Stack";
            stack.transform.SetParent(Stacks.transform);
            stack.transform.position = new Vector3(Stacks.transform.position.x, Stacks.transform.position.y+(i*0.05f), Stacks.transform.position.z);
            stack.GetComponent<StackObject>().stackNumber = i;
            childStacks.Add(stack);
            colliders[i].SetActive(true);            
        }

        stacksCreated = true;
    }

    private void moveStacksOnScreen()
    {
        if (childStacks.Count > 0 && Input.touchCount == 1)
        {
            if (Input.GetMouseButtonDown(0) && chosenStack == null)
            {
                Ray mouseRay = GenerateMouseRay();
                RaycastHit hit;
                Debug.DrawRay(mouseRay.origin, mouseRay.direction);
                if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, raycastStackSelection))
                {
                    chosenStack = hit.transform.gameObject;

                    if (chosenStack != null && chosenStack.name == "Stack")
                    {
                        Debug.Log(chosenStack.name);

                        startPosition = chosenStack.transform.position;

                        objPlane = new Plane(Camera.main.transform.forward * -1, chosenStack.transform.position);

                        // calc mouse offset
                        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                        float rayDistance;
                        objPlane.Raycast(mRay, out rayDistance);
                        m0 = chosenStack.transform.position - mRay.GetPoint(rayDistance);
                    }
                }
            }
            else if (Input.GetMouseButton(0) && chosenStack != null && chosenStack.name == "Stack")
            {
                Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                float rayDistance;

                if (objPlane.Raycast(mRay, out rayDistance))
                {
                    chosenStack.transform.position = mRay.GetPoint(rayDistance) + m0;
                }
            }
            else if (Input.GetMouseButtonUp(0) && chosenStack != null && chosenStack.name == "Stack")
            {
                Ray mouseRay = GenerateMouseRay();
                RaycastHit hit;
                Debug.DrawRay(mouseRay.origin, mouseRay.direction);
                if (Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, raycastSleeveSelection))
                {
                    receivingSleeve = hit.transform.gameObject;

                    if (receivingSleeve != null && receivingSleeve.layer == 10)
                    {
                        Debug.Log(receivingSleeve.name + " received stack");
                        Debug.Log(chosenStack.GetComponent<StackObject>().stackNumber);
                        CombinedStackWithSleeve(chosenStack.GetComponent<StackObject>().stackNumber, receivingSleeve.name.ToCharArray()[6]);
                        chosenStack.SetActive(false);
                        childStacks.Remove(chosenStack);
                        chosenStack = null;
                    }
                    else
                    {
                        Debug.Log("No sleeve chosen");
                        chosenStack.transform.position = startPosition;
                        chosenStack = null;
                    }
                }
            }
            else
            {
                chosenStack = null;
            }
        }
    }
}
