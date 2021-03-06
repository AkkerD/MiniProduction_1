﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawn : MonoBehaviour {

    public float SliderValX=20;
    public float SliderValY = 20;
    Cloth ClothComp;
    Rigidbody RigidBod;
    public bool FallsFromSide;
    public SkinnedMeshRenderer SMRendereder;
    public GameObject Zipper;
    public bool FallsFromLeft = false;
  SlimeControl ZipperScript;


    void Start()
    {

      // SMRendereder = gameObject.GetComponent<SkinnedMeshRenderer>();
       // SMRendereder.enabled = false;

        if (gameObject.GetComponent<Cloth>() !=null) {
        ClothComp = gameObject.GetComponent<Cloth>();
        ClothComp.enabled = false;
        } else if (gameObject.GetComponent<Rigidbody>() != null)
        {
            RigidBod = gameObject.GetComponent<Rigidbody>();
            RigidBod.isKinematic = true;
            SMRendereder.enabled = true;
        }

      

        ZipperScript = Zipper.GetComponent<SlimeControl>();
        SMRendereder.enabled = false;
    }

    // Update is called once per frame
    void Update () {


        if (RigidBod == null)
        {


            if (ZipperScript.ZipperValY < transform.position.y && FallsFromSide)
            {

                SMRendereder.enabled = true;
                ClothComp.enabled = true;
                if (ClothComp.externalAcceleration.x > 0)
                {
                    ClothComp.externalAcceleration += new Vector3(-0.2f, 0, 0);
                }


            } else if (ZipperScript.ZipperValY < transform.position.y && !FallsFromSide && ZipperScript.ZipperValX < transform.position.x)
            {

                SMRendereder.enabled = true;
                ClothComp.enabled = true;
                if (ClothComp.externalAcceleration.x > 0)
                {
                    ClothComp.externalAcceleration += new Vector3(-0.2f, 0, 0);
                }


            }
        } 
        else if (RigidBod != null)
        {

            if (ZipperScript.ZipperValY < transform.position.y && FallsFromSide)
            {
                //RigidBod.isKinematic = false;
                FallsFromLeft = true;
            } else if (ZipperScript.ZipperValY < transform.position.y && !FallsFromSide && ZipperScript.ZipperValX < transform.position.x)
            {
                //RigidBod.isKinematic = false;
                FallsFromLeft = true;
            }

            if(ZipperScript.ZipperValY > transform.position.y+1.5f && ZipperScript.ZipperValX < transform.position.x && FallsFromLeft)
            {

                RigidBod.isKinematic = false;
                SMRendereder.enabled = true;
            }

        }
    }
}
