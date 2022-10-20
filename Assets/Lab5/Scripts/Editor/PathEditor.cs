using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    private PathCreator _creator;
    private Path _path;


    public void OnEnable()
    {
        _creator = (PathCreator)target;

        if (_creator.Path == null)
        {
            _creator.CreatePath();
        }

        _path = _creator.Path;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Create New"))
        {
            Undo.RecordObject(_creator, "Create New");
            _creator.CreatePath();
            _path = _creator.Path;
        }

        if (GUILayout.Button("Toggle Closed"))
        {
            Undo.RecordObject(_creator, "Toggle Closed");
            _path.ToggleClosed();
        }

        bool autoSetControlPoints = GUILayout.Toggle(_path.AutoSetControlPoints, "Auto Set Control Points");

        if (autoSetControlPoints != _path.AutoSetControlPoints)
        { 
            Undo.RecordObject(_creator, "Toggle Auto Set Controls");
            _path.AutoSetControlPoints = autoSetControlPoints;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }
    }


    private void OnSceneGUI()
    {
        Input();
        Draw();
    }

    private void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(_creator, "Add Segment");
            _path.AddSegment(mousePos);
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.control)
        {
            float minDist = 100f;
            int closestIndex = -1;
            
            for(int i = 0; i < _path.NumPoints; i += 3)
            {
                float dist = Vector3.Distance(mousePos, _path[i]);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }

            if (closestIndex != -1)
            {
                Undo.RecordObject(_creator, "Delete Segment");
                _path.DeleteSegment(closestIndex);
            }
        }
    }

    private void Draw()
    {
        Handles.color = Color.black;
        for (int i = 0; i < _path.NumSegments; i++)
        {
            Vector3[] points = _path.GetPointsInSegment(i);

            Handles.DrawLine(points[1], points[0]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 5);
        }

        Handles.color = Color.red;
        for (int i = 0; i < _path.NumPoints; i++)
        {
            Vector3 newPos = Handles.FreeMoveHandle(_path[i], Quaternion.identity
                , .1f, Vector3.zero, Handles.CylinderHandleCap);

            if (_path[i] != newPos)
            {
                Undo.RecordObject(_creator, "Move Point");
                _path.MovePoint(i, newPos);
            }
        }
    }
}
