using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Games.MauMau.Source {
    public class EventLog : MonoBehaviour {
        
        [SerializeField] private TextMeshProUGUI logLinePrefab;
        [SerializeField] private int maxNumberOfLines = 11;
        [SerializeField] private bool showTimestamps = false;
        

        private readonly List<TextMeshProUGUI> _lines = new List<TextMeshProUGUI>();
        private readonly ConcurrentQueue<string> _lineBuffer = new ConcurrentQueue<string>();

        private bool _reorderRequired; 
        
        private void Update () {
            if (_lineBuffer.TryDequeue(out var line)) {
                CommitLine(line);
            }
            else if (_reorderRequired) {
                OrderChildren();
            }
        }

        public void AddLine (string line) {
            _lineBuffer.Enqueue(line);
        }

        private void CommitLine (string line) {
            TextMeshProUGUI newLine;
            if (_lines.Count < maxNumberOfLines) {
                newLine = Instantiate(logLinePrefab, transform);
            }
            else {
                newLine = _lines[0];
                _lines.RemoveAt(0);
                _reorderRequired = true;
            }
            
            var lineText = $" {line}";
            AddTimestamps(ref lineText);
            newLine.text = lineText;
            _lines.Add(newLine);
        }

        private void AddTimestamps (ref string line) {
            if (!showTimestamps) return;
            var timeString = $"{DateTime.Now:T}";
            line = $" [{timeString}]{line}";
        }

        private void OrderChildren () {
            for (var i = 0; i < _lines.Count; i++) {
                _lines[i].transform.SetSiblingIndex(i);
            }

            _reorderRequired = false;
        }
        
    }
}