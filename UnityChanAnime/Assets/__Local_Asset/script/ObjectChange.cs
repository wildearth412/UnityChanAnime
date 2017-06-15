using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectChange
{
    public class ObjectChange : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> gos = new List<GameObject>();

        public void SetStatus(int idx,bool status)
        {
            gos[idx].SetActive(status);
        }
    }
}