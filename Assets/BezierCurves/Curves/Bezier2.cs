﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BezierCurves
{
    /// <summary>
    /// A bezier curve of arbitrary degree using a Bernstein Polynominal.
    /// Based on this code...
    /// https://www.codeproject.com/Articles/25237/Bezier-Curves-Made-Simple
    /// </summary>
    public class Bezier2 : Bezier
    {

        /// <summary>
        /// The curves degree. 1 is linear, 2 quadratic, etc.
        /// </summary>
        public int Degree { get { return Control.Length - 1; } }

        /// <summary>
        /// The control points.
        /// </summary>
        public Vector2[] Control { get; private set; }

        public Bezier2(BEZIER_DEGREE degree) 
            : this((int)degree)
        {

        }

        public Bezier2(int degree)
        {
            if (degree > MAX_DEGREE || degree < MIN_DEGREE)
                throw new ArgumentException(string.Format("Degree can not be greater than {0} or less than {1}.", MAX_DEGREE, MIN_DEGREE));

            Control = new Vector2[degree + 1];
        }

        public Bezier2(IList<Vector2> control)
        {
            int degree = control.Count - 1;
            if (degree > MAX_DEGREE || degree < MIN_DEGREE)
                throw new ArgumentException(string.Format("Degree can not be greater than {0} or less than {1}.", MAX_DEGREE, MIN_DEGREE));

            int count = control.Count;
            Control = new Vector2[count];
            for (int i = 0; i < count; i++)
                Control[i] = control[i];
        }

        /// <summary>
        /// The position on the curve at t.
        /// </summary>
        /// <param name="t">Number between 0 and 1.</param>
        public Vector2 Position(float t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;

            int n = Control.Length;
            int degree = Degree;
            Vector2 p = new Vector2();

            for (int i = 0; i < n; i++)
            {
                float basis = Bernstein(degree, i, t);
                p += basis * Control[i];
            }

            return p;
        }

        /// <summary>
        /// The tangent on the curve at t.
        /// </summary>
        /// <param name="t">Number between 0 and 1.</param>
        public Vector2 Tangent(float t)
        {
            Vector2 d = FirstDerivative(t);
            d.Normalize();
            return d;
        }

        /// <summary>
        /// The normal on the curve at t.
        /// </summary>
        /// <param name="t">Number between 0 and 1.</param>
        public Vector2 Normal(float t)
        {
            Vector2 d = FirstDerivative(t);
            d.Normalize();
            return new Vector2(d.y, -d.x);
        }

        /// <summary>
        /// The first derivative on the curve at t.
        /// </summary>
        /// <param name="t">Number between 0 and 1.</param>
        public Vector2 FirstDerivative(float t)
        {
            if (t < 0) t = 0;
            if (t > 1) t = 1;

            int n = Control.Length;
            int degree = Degree;
            float inv = 1.0f / degree;
            Vector2 d = new Vector2();

            for (int i = 0; i < n - 1; i++)
            {
                float basis = Bernstein(degree - 1, i, t);
                d += basis * inv * (Control[i + 1] - Control[i]);
            }

            return d * 4.0f;
        }

        /// <summary>
        /// Fills the array with positions on the curve.
        /// </summary>
        public void GetPositions(IList<Vector2> points)
        {
            int count = points.Count;
            int n = Control.Length;
            int degree = Degree;

            float t = 0;
            float step = 1.0f / (count - 1.0f);

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    float basis = Bernstein(degree, j, t);
                    points[i] += basis * Control[j];
                }

                t += step;
            }
        }

        /// <summary>
        /// Arc length of curve via intergration.
        /// </summary>
        public float Length(int steps, float tmax = 1.0f)
        {
            if (tmax <= 0) return 0;
            if (tmax > 1) tmax = 1;

            if(Degree == 1)
                return Vector2.Distance(Control[0], Control[1]) * tmax;
            else
            {
                steps = Math.Max(steps, 2);
                float len = 0;
                Vector2 previous = Position(0);

                for (int i = 1; i < steps; i++)
                {
                    float t = i / (steps - 1.0f) * tmax;
                    Vector2 p = Position(t);

                    len += Vector2.Distance(previous, p);
                    previous = p;
                }

                return len;
            }
        }

        /// <summary>
        /// Returns the position at t using DeCasteljau's algorithm.
        /// Same as Position(t) but slower.
        /// </summary>
        public Vector2 DeCasteljau(float t)
        {
            int count = Control.Length;
            Vector2[] Q = new Vector2[count];
            Array.Copy(Control, Q, count);

            for (int k = 1; k < count; k++)
            {
                for (int i = 0; i < count - k; i++)
                    Q[i] = (1.0f - t) * Q[i] + t * Q[i + 1];
            }

            return Q[0];
        }

        /// <summary>
        /// Splits the bezier at t and returns the two curves.
        /// </summary>
        /// <param name="t">Position to split (0 to 1).</param>
        /// <param name="b0">The curve from 0 to t.</param>
        /// <param name="b1">The curve from t to 1.</param>
        public void Split(float t, out Bezier2 b0, out Bezier2 b1)
        {
            int count = Control.Length;
            Vector2[] Q = new Vector2[count];
            Array.Copy(Control, Q, count);

            b0 = new Bezier2(Degree);
            b1 = new Bezier2(Degree);

            b0.Control[0] = Control[0];
            b1.Control[count - 1] = Control[count - 1];

            for (int k = 1; k < count; k++)
            {
                int len = count - k;
                for (int i = 0; i < len; i++)
                    Q[i] = (1.0f - t) * Q[i] + t * Q[i + 1];

                b0.Control[k] = Q[0];
                b1.Control[len - 1] = Q[len - 1];
            }
        }

    }
}
