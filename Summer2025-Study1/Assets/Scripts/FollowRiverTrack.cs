using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Cinemachine
{
    public class FollowRiverTrack : MonoBehaviour
    {

        public CinemachinePath path;
        public GameObject vr_cam;

        private CInemachinePath.PositionUnits position_units = CinemachinePath.PositionUnits.PathUnits;
        
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            SetCartPosition(path.FindClosestPoint(vr_cam.transform.position, 0, -1, 10));
        }

        void SetCartPosition(float distance_along_path)
        {
            float m_position = path.StandardizeUnit(distance_along_path, position_units);
            transform.position = path.EvaluatePositionAtUnit(m_position, position_units);
            transform.rotation = path.EvaluateOrientationAtUnit(m_position, position_units);
        }
    }
}

