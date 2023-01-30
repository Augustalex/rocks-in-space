using System;
using TMPro;
using UnityEngine;

namespace BuildMenu
{
    public class BuildMenuPane : MonoBehaviour
    {
        private TMP_Text _header;

        private void Awake()
        {
            _header = GetComponentInChildren<TMP_Text>();
        }

        private void Start()
        {
            UpdatePosition();
        }

        public void Show()
        {
            UpdatePosition();

            _header.text = "Build";
        }

        public void Hide()
        {
            _header.text = "";
        }

        private void Update()
        {
            // UpdatePosition();
        }

        private void UpdatePosition()
        {
            transform.position = Input.mousePosition;
        }

        public void Test()
        {
            Debug.Log("TEST");
        }
    }

}