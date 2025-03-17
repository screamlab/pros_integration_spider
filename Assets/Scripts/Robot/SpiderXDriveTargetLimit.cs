/*
set upperlimit and lowerlimit for 16 joints' AritulationBody.xDrive.target
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderXDriveTargetLimit : MonoBehaviour
{
    public ArticulationBody[] articulationBodies;

    private Dictionary<string, float> target_absLimit = new Dictionary<string, float>
    {
        { "Shoulder_Abduction", 90.0f },
        { "Shoulder_Adduction", 20.0f },
        { "Calf_extension", 60.0f }
    };

    void Start()
    {
        if (articulationBodies.Length != 16)
        {
            Debug.LogWarning("ArticulationBody is not correctly assigned.");
        }
        else
        {
            float[] upperLimit = new float[]
            {
                target_absLimit["Shoulder_Abduction"], target_absLimit["Calf_extension"], //LFF
                target_absLimit["Shoulder_Abduction"], target_absLimit["Calf_extension"], //LF
                target_absLimit["Shoulder_Adduction"], target_absLimit["Calf_extension"],  //LB
                target_absLimit["Shoulder_Adduction"], target_absLimit["Calf_extension"],  //LBB
                target_absLimit["Shoulder_Adduction"], target_absLimit["Calf_extension"],  //RFF
                target_absLimit["Shoulder_Adduction"], target_absLimit["Calf_extension"],  //RF
                target_absLimit["Shoulder_Abduction"], target_absLimit["Calf_extension"], //RB
                target_absLimit["Shoulder_Abduction"], target_absLimit["Calf_extension"], //RBB
            };

            float[] lowerLimit = new float[]
            {
                -target_absLimit["Shoulder_Adduction"], -target_absLimit["Calf_extension"],  //LFF
                -target_absLimit["Shoulder_Adduction"], -target_absLimit["Calf_extension"],  //LF
                -target_absLimit["Shoulder_Abduction"], -target_absLimit["Calf_extension"], //LB
                -target_absLimit["Shoulder_Abduction"], -target_absLimit["Calf_extension"], //LBB
                -target_absLimit["Shoulder_Abduction"], -target_absLimit["Calf_extension"], //RFF
                -target_absLimit["Shoulder_Abduction"], -target_absLimit["Calf_extension"], //RF
                -target_absLimit["Shoulder_Adduction"], -target_absLimit["Calf_extension"],  //RB 
                -target_absLimit["Shoulder_Adduction"], -target_absLimit["Calf_extension"],  //RBB       
            };

            for (int i = 0; i < articulationBodies.Length; i ++){
                ArticulationDrive drive = articulationBodies[i].xDrive;
                drive.upperLimit = upperLimit[i];
                drive.lowerLimit = lowerLimit[i];
                articulationBodies[i].xDrive = drive;
            }
        }

        return;
    }

}

