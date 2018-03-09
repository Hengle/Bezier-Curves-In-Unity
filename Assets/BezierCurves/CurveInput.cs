using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierCurves
{

    public abstract class CurveInput : MonoBehaviour
    {

        public BEZIER_DEGREE Degree = BEZIER_DEGREE.QUADRATIC;

        protected List<Vector2> Points { get; private set; }

        private List<int> Indices { get; set; }

        protected Color LineColor = Color.red;

        protected bool MadeCurve { get; set; }

        protected bool Parametric { get; set; }

        protected float SnapPoint { get; set; }

        protected virtual void OnCurveComplete(List<Vector2> control)
        {

        }

        protected virtual void OnCurveCleared()
        {

        }

        protected virtual void OnLeftClick(Vector2 point)
        {

        }

        protected virtual void Start()
        {
            SnapPoint = 10;
        }

        protected virtual void Update()
        {

            bool leftMouseClicked = Input.GetMouseButtonDown(0);

            if (leftMouseClicked)
            {
                Vector2 point = GetMousePosition();
                OnLeftClick(point);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                uint i = (uint)Degree + 1;
                if (i < 1) i = 5;
                if (i > 5) i = 1;
                Degree = (BEZIER_DEGREE)i;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                uint i = (uint)Degree - 1;
                if (i < 1) i = 5;
                if (i > 5) i = 1;
                Degree = (BEZIER_DEGREE)i;
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                Parametric = !Parametric;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetInput();
                OnCurveCleared();
            }
            else if (!MadeCurve)
            {
                Vector2 point = GetMousePosition();

                if (leftMouseClicked)
                {
                    if (Points == null)
                    {
                        CreatePoints();
                        AddPoint(point);
                        AddPoint(point);
                    }
                    else
                    {
                        int degree = (int)Degree;
                        if (Points.Count == degree+1)
                        {
                            MadeCurve = true;
                            OnCurveComplete(Points);
                        }
                        else
                        {
                            AddPoint(point);
                        }
                    }
                }
                else
                {
                    MoveLastPoint(point);
                }

            }

        }

        protected virtual void OnGUI()
        {
            int textLen = 400;
            int textHeight = 25;

            GUI.Label(new Rect(10, 10, textLen, textHeight), "Up/Down arrow to change curve degree.");
            GUI.Label(new Rect(10, 30, textLen, textHeight), "Tab to change to parametric and back.");
            GUI.Label(new Rect(10, 50, textLen, textHeight), "Space to clear");
            GUI.Label(new Rect(10, 70, textLen, textHeight), "Degree = " + Degree);
            GUI.Label(new Rect(10, 90, textLen, textHeight), "Parametric = " + Parametric);

            GUI.Label(new Rect(10, 130, textLen, textHeight), "Click to draw control points...");
        }

        protected virtual void OnPostRender()
        {
            Camera cam = Camera.current;
            if (cam == null) return;
            if (Points == null) return;
            if (Indices == null) return;

            Matrix4x4 m = Matrix4x4.identity;

            DrawLines.LineMode = LINE_MODE.LINES;
            DrawLines.Draw(cam, Points, LineColor, m, Indices);
            DrawVertices.Draw(cam, 0.02f, Points, Color.yellow, m);
        }

        protected void ResetInput()
        {
            MadeCurve = false;
            Points = null;
            Indices = null;
        }

        private Vector2 GetMousePosition()
        {
            Vector3 p = Input.mousePosition;

            Camera cam = GetComponent<Camera>();
            p = cam.ScreenToWorldPoint(p);

            if (SnapPoint > 0.0f)
            {
                p.x = Mathf.Round(p.x * SnapPoint) / SnapPoint;
                p.y = Mathf.Round(p.y * SnapPoint) / SnapPoint;
            }

            return p;
        }

        private void CreatePoints()
        {
            Points = new List<Vector2>();
            Indices = new List<int>();
        }

        private void AddPoint(Vector2 point)
        {
            if (Points == null) return;

            Points.Add(point);

            int count = Points.Count;
            if (count == 1) return;

            Indices.Add(count - 2);
            Indices.Add(count - 1);
        }

        private void MoveLastPoint(Vector2 point)
        {
            if (Points == null) return;

            int count = Points.Count;
            if (count == 0) return;

            Points[count - 1] = point;
        }

    }
}
