using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierCurves
{

    public class Line2
    {
        public List<Vector2> Positions = new List<Vector2>();
        public List<Vector2> Control = new List<Vector2>();
    }

    /// <summary>
    /// All input code is in base class so the demo can just focus on the curves.
    /// </summary>
    public class DemoCurves : CurveInput
    {

        List<Line2> lines = new List<Line2>();

        protected override void OnCurveComplete(List<Vector2> control)
        {
            Line2 line;

            if (Parametric)
            {
                ParametricBezier2 curve = new ParametricBezier2(control);
                line = CreateFromParametricBezier(curve, 0.1f);
            }
            else
            {
                Bezier2 curve = new Bezier2(control);
                line = CreateFromBezier(curve, 0.1f);
            }

            lines.Add(line);

            ResetInput();
        }

        protected override void OnCurveCleared()
        {
            lines.Clear();
        }

        protected override void OnPostRender()
        {
            base.OnPostRender();

            Camera cam = Camera.current;
            if (cam == null) return;

            DrawLines.LineMode = LINE_MODE.LINES;
            Matrix4x4 m = Matrix4x4.identity;

            foreach(var line in lines)
            {
                DrawLines.Draw(cam, line.Control, Color.red, m);
                DrawLines.Draw(cam, line.Positions, Color.blue, m);
                DrawVertices.Draw(cam, 0.02f, line.Positions, Color.yellow, m);
            }
        }

        /// <summary>
        /// Creates a line with points more densely spaced at sharp curves.
        /// </summary>
        private Line2 CreateFromBezier(Bezier2 bezier, float spacing)
        {
            float length = bezier.Length(64);
            int count = (int)Mathf.Max(2, (length / spacing));

            Line2 line = new Line2();
            line.Control.AddRange(bezier.Control);

            for (int i = 0; i < count; i++)
            {
                float t = i / (count - 1.0f);
                line.Positions.Add(bezier.Position(t));
            }

            return line;
        }

        /// <summary>
        /// Creates a line with points evenly spaced.
        /// </summary>
        private Line2 CreateFromParametricBezier(ParametricBezier2 bezier, float spacing)
        {
            float length = bezier.Length(64);
            int count = (int)Mathf.Max(2, (length / spacing));

            Line2 line = new Line2();
            line.Control.AddRange(bezier.Control);

            for (int i = 0; i < count; i++)
            {
                float s = i / (count - 1.0f) * length;
                float t = bezier.Parametrize(s, length);

                line.Positions.Add(bezier.Position(t));
            }

            return line;
        }

    }

}
