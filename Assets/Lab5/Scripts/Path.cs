using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Path
{
    [SerializeField] private List<Vector3> points;
    [SerializeField] private bool isClosed;
    [SerializeField] private bool autoSetControlPoints;

    public Path(Vector3 centre)
    {
        points = new List<Vector3>()
        {
            centre + Vector3.left,
            centre + (Vector3.left + Vector3.up) * 0.5f,
            centre + (Vector3.right + Vector3.down) * 0.5f,
            centre + Vector3.left
        };
    }

    public int NumPoints => points.Count;
    public Vector3 this[int i] => points[i];
    public int NumSegments => points.Count / 3;


    public bool AutoSetControlPoints
    {
        get { return autoSetControlPoints; }

        set
        {
            if (autoSetControlPoints != value)
            {
                autoSetControlPoints = value;

                if (autoSetControlPoints)
                {
                    AutoSetAllControlPoints();
                }
            }
        }
    }

    public void AddSegment(Vector3 anchorPos)
    {
        points.Add(points[^1] * 2 - points[^2]);
        points.Add((points[^1] + anchorPos) * 0.5f);
        points.Add(anchorPos);

        if (autoSetControlPoints)
        {
            AutoSetAllAffectedControlPoints(points.Count - 1);
        }
    }

    public Vector3[] GetPointsInSegment(int index)
    {
        return new Vector3[]
        {
            points[LoopIndex(index * 3)],
            points[LoopIndex(index * 3 + 1)],
            points[LoopIndex(index * 3 + 2)],
            points[LoopIndex(index * 3 + 3)]
        };
    }

    public void MovePoint(int index, Vector3 pos)
    {
        Vector3 move = pos - points[index];

        if (index % 3 == 0 || !autoSetControlPoints)
        {
            points[index] = pos;

            if (autoSetControlPoints)
            {
                AutoSetAllAffectedControlPoints(index);
            }
            else
            {
                if (index % 3 == 0)
                {
                    if (index + 1 < points.Count || isClosed)
                    {
                        points[LoopIndex(index + 1)] += move;
                    }

                    if (index - 1 >= 0 || isClosed)
                    {
                        points[LoopIndex(index - 1)] += move;
                    }
                }
                else
                {
                    bool isAnchor = (index + 1) % 3 == 0;

                    int correspondingIndex = isAnchor ? index + 2 : index - 2;
                    int anchorIndex = isAnchor ? index + 1 : index - 1;


                    if (correspondingIndex >= 0 && correspondingIndex < points.Count || isClosed)
                    {
                        float dist = ((points[LoopIndex(anchorIndex)]) - points[LoopIndex(correspondingIndex)])
                            .magnitude;
                        Vector3 dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                        points[LoopIndex(correspondingIndex)] = points[LoopIndex(anchorIndex)] + dir * dist;
                    }
                }
            }
        }
    }

    public void DeleteSegment(int index)
    {
        if (NumSegments > 2 || !isClosed && NumSegments > 1)
        {
            if (index == 0)
            {
                if (isClosed)
                {
                    points[points.Count - 1] = points[2];
                }

                points.RemoveRange(0, 3);
            }
            else if (index == points.Count - 1 && !isClosed)
            {
                points.RemoveRange(index - 2, 3);
            }
            else
            {
                points.RemoveRange(index - 1, 3);
            }
        }
    }

    public void ToggleClosed()
    {
        isClosed = !isClosed;

        if (isClosed)
        {
            points.Add(points[^1] * 2 - points[^2]);
            points.Add(points[0] * 2 - points[1]);

            if (autoSetControlPoints)
            {
                AutoSetAnchorControlPoints(0);
                AutoSetAnchorControlPoints(points.Count-3);
            }
        }
        else
        {
            points.RemoveRange(points.Count-2, 2);

            if (autoSetControlPoints)
            {
                AutoSetStartAndEndControls();
            }
        }
    }

    public int LoopIndex(int index)
    {
        return (index + points.Count) % points.Count;
    }
    
    private void AutoSetAnchorControlPoints(int index)
    {
        Vector3 anchorPos = points[index];
        Vector3 dir = Vector3.zero;

        float[] distances = new float[2];

        if (index - 3 >= 0 || isClosed)
        {
            Vector3 offset = points[LoopIndex(index - 3)] - anchorPos;
            dir += offset.normalized;
            distances[0] = offset.magnitude;
        }
        
        if (index + 3 >= 0 || isClosed)
        {
            Vector3 offset = points[LoopIndex(index + 3)] - anchorPos;
            dir -= offset.normalized;
            distances[1] = -offset.magnitude;
        }
        
        dir.Normalize();

        for (int i = 0; i < 2; i++)
        {
            int controlIndex = index + i * 2 - 1;

            if (controlIndex >= 0 && controlIndex < points.Count || isClosed)
            {
                points[LoopIndex(controlIndex)] = anchorPos + dir * distances[i] * 0.5f;
            }
        }
    }

    private void AutoSetAllAffectedControlPoints(int index)
    {
        for (int i = index - 3; i <= index + 3; i += 3)
        {
            if (i >= 0 && i < points.Count || isClosed)
            {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }
        
        AutoSetStartAndEndControls();
    }

    private void AutoSetAllControlPoints()
    {
        for (int i = 0; i < points.Count; i+= 3)
        {
            AutoSetAnchorControlPoints(i);
        }
        
        AutoSetStartAndEndControls();
    }

    private void AutoSetStartAndEndControls()
    {
        if (isClosed) return;
        
        points[1] = (points[0] + points[2]) * 0.5f;
        points[^2] = (points[^1] + points[^3]) * 0.5f;
    }
}
