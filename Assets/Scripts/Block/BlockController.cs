using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MatchingCubes.Block
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private List<int> _Columns;
        [SerializeField] private GameObject _BlockPrefab;

        private List<List<GameObject>> _cubes;

        private int _childIndex;
        
        public bool IsActive { get; private set; }

        private void Start()
        {
            CreatBlock();
            IsActive = true;
        }


// #if UNITY_EDITOR
//         private void OnValidate()
//         {
//             EditorApplication.delayCall += CreatBlock;
//         }
// #endif


        public void  CreatBlock()
        {
            _cubes = new List<List<GameObject>>();
            _childIndex = 0;
            
            for (var i = 0; i < _Columns.Count; i++)
                CreateColumn(i, _Columns[i]);
            
            for (var i = transform.childCount - 1; i >= _childIndex; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }


        private void CreateColumn(int columnIndex, int rowCount)
        {
            if (columnIndex >= _cubes.Count) _cubes.Add(new List<GameObject>());

            for (var i = 0; i < rowCount; i++)
            {
                if (i >= _cubes[columnIndex].Count) AddCube(columnIndex, i);
            }
        }

        private void AddCube(int x, int y)
        {
            GameObject block;
            if (transform.childCount > _childIndex)
                block = transform.GetChild(_childIndex).gameObject;
            else
                block = Instantiate(_BlockPrefab, transform);
            
            _childIndex++;
            
            block.transform.localPosition = new Vector3(x, y - .5f, 0);
            _cubes[x].Add(block);
        }


        public int CalculateHeight(GameObject cube)
        {
            foreach (var column in _cubes)
            {
                for (var j = 0; j < column.Count; j++)
                {
                    if (column[j] == cube)
                    {
                        IsActive = false;
                        return column.Count;
                    }
                }
            }

            throw new NotImplementedException();
        }


        public void DestroyBlocks(Vector3 centerPoint)
        {
            IsActive = false;
            for (var i = 0; i < transform.childCount; i++)
            {
                var unit = transform.GetChild(i).gameObject;
                unit.GetComponent<BoxCollider>().isTrigger = false;
                unit.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                var forceDirection = (unit.transform.position - centerPoint).normalized; 
                unit.GetComponent<Rigidbody>().AddForce(forceDirection * 1000);
            }
        }
    }
}