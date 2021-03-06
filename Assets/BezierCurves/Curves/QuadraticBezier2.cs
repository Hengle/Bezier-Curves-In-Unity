﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BezierCurves
{
    /// <summary>
    /// A bezier curve of quadratic degree using a polynominal.
    /// </summary>
    public class QuadraticBezier2
    {

        /// <summary>
        /// The control points.
        /// </summary>
        public Vector2 C0 { get; set; }
        public Vector2 C1 { get; set; }
        public Vector2 C2 { get; set; }

        public QuadraticBezier2()
        {

        }

        public QuadraticBezier2(Vector2 c0, Vector2 c1, Vector2 c2)
        {
            C0 = c0;
            C1 = c1;
            C2 = c2;
        }

        /// <summary>
        /// The length of the curve.
        /// </summary>
        public float Length
        {
            get
            {
                float ax = C0.x - 2.0f * C1.x + C2.x;
                float az = C0.y - 2.0f * C1.y + C2.y;
                float bx = 2.0f * (C1.x - C0.x);
                float bz = 2.0f * (C1.y - C0.y);

                float A = 4.0f * (ax * ax + az * az);
                float B = 4.0f * (ax * bx + az * bz);
                float C = bx * bx + bz * bz;

                float Sabc = 2.0f * (float)Math.Sqrt(A + B + C);
                float A2 = (float)Math.Sqrt(A);
                float A32 = 2.0f * A * A2;
                float CSQ = 2.0f * (float)Math.Sqrt(C);
                float BA = B / A2;

                return (A32 * Sabc + A2 * B * (Sabc - CSQ) + (4 * C * A - B * B) * (float)Math.Log((2 * A2 + BA + Sabc) / (BA + CSQ))) / (4 * A32);
            }
        }

        /// <summary>
        /// The position on the curve at t.
        /// </summary>
        /// <param name="t">Number between 0 and 1.</param>
        public Vector2 Position(float t)
        {
            if (t < 0.0f) t = 0.0f;
            if (t > 1.0f) t = 1.0f;

            float t1 = 1.0f - t;

            Vector2 p = new Vector2();
            p.x = t1 * (t1 * C0.x + t * C1.x) + t * (t1 * C1.x + t * C2.x);
            p.y = t1 * (t1 * C0.y + t * C1.y) + t * (t1 * C1.y + t * C2.y);

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
            if (t < 0.0) t = 0.0f;
            if (t > 1.0) t = 1.0f;

            float t1 = 1.0f - t;

            Vector2 p = new Vector2();
            p.x = 2.0f * t1 * (C1.x - C0.x) + 2.0f * t * (C2.x - C1.x);
            p.y = 2.0f * t1 * (C1.y - C0.y) + 2.0f * t * (C2.y - C1.y);

            return p;
        }

        /// <summary>
        /// The closest point on the curve to the point p.
        /// </summary>
        public Vector2 Closest(Vector2 p)
        {
            float px = C0.x - p.x;
            float pz = C0.y - p.y;
            float ax = C1.x - C0.x;
            float az = C1.y - C0.y;
            float bx = C0.x - 2.0f * C1.x + C2.x;
            float bz = C0.y - 2.0f * C1.y + C2.y;

            float a = bx * bx + bz * bz;
            float b = 3 * (ax * bx + az * bz);
            float c = 2 * (ax * ax + az * az) + px * bx + pz * bz;
            float d = px * ax + pz * az;

            var roots = Polynomial3d.Solve(a, b, c, d);

            float min = float.PositiveInfinity;
            Vector2 closest = new Vector2(min, min);

            for (int i = 0; i < roots.real; i++)
            {
                float t = (float)roots[i];

                Vector2 v = Position(t);
                float dist = (v - p).sqrMagnitude;
                if (dist < min)
                {
                    min = dist;
                    closest = v;
                }
            }

            return closest;
        }

        /// <summary>
        /// If the segment ab intersects the curve.
        /// </summary>
        public bool Intersects(Vector2 a, Vector2 b)
        {
            //coefficients of quadratic
            Vector2 c2 = C0 + C1 * -2.0f + C2;
            Vector2 c1 = C0 * -2.0f + C1 * 2.0f;

            //Convert line to normal form: ax + by + c = 0
            //Find normal to line: negative inverse of original line's slope
            Vector2 n = new Vector2(a.y - b.y, b.x - a.x);

            //c coefficient for normal form of line
            float c = a.x * b.y - b.x * a.y;

            //Transform coefficients to line's coordinate system and find roots of cubic
            var roots = Polynomial3d.Solve(1, Vector2.Dot(n, c2), Vector2.Dot(n, c1), Vector2.Dot(n, C0) + c);

            Vector2 min, max;
            min.x = Math.Min(a.x, b.x);
            min.y = Math.Min(a.y, b.y);

            max.x = Math.Max(a.x, b.x);
            max.y = Math.Max(a.y, b.y);

            for (int i = 0; i < roots.real; i++)
            {
                float t = (float)roots[i];
                if (t < 0.0f || t > 1.0f) continue;

                Vector2 v0 = Position(t);

                if (a.x == b.x)
                {
                    if (min.y <= v0.y && v0.y <= max.y)
                        return true;
                }
                else if (a.y == b.y)
                {
                    if (min.x <= v0.x && v0.x <= max.x)
                        return true;
                }
                else if (min.x <= v0.x && v0.x <= max.x && min.y <= v0.y && v0.y <= max.y)
                    return true;
            }

            return false;

        }

    }
}
