using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierCurves
{

    public class DrawVertices : DrawBase
    {

        public static void Draw(Camera camera, float size, IList<Vector3> vertices, IList<Color> colors, Matrix4x4 localToWorld)
        {
            if (camera == null || vertices == null) return;
            m_vertices.Clear();

            int count = vertices.Count;
            for (int i = 0; i < count; i++)
                m_vertices.Add(vertices[i]);

            Draw(camera, size, m_vertices, colors, localToWorld);
        }

        public static void Draw(Camera camera, float size, IList<Vector3> vertices, Color color, Matrix4x4 localToWorld)
        {
            if (camera == null || vertices == null) return;
            m_vertices.Clear();
            m_colors.Clear();

            int count = vertices.Count;
            for (int i = 0; i < count; i++)
            {
                m_vertices.Add(vertices[i]);
                m_colors.Add(color);
            }

            Draw(camera, size, m_vertices, m_colors, localToWorld);
        }

        public static void Draw(Camera camera, float size, IList<Vector2> vertices, Color color, Matrix4x4 localToWorld)
        {
            if (camera == null || vertices == null) return;
            m_vertices.Clear();
            m_colors.Clear();

            int count = vertices.Count;
            for (int i = 0; i < count; i++)
            {
                m_vertices.Add(vertices[i]);
                m_colors.Add(color);
            }

            Draw(camera, size, m_vertices, m_colors, localToWorld);
        }

        private static void Draw(Camera camera, float size, IList<Vector4> vertices, IList<Color> colors, Matrix4x4 localToWorld)
        {
            if (camera == null || vertices == null) return;
            if (vertices.Count != colors.Count) return;

            GL.PushMatrix();

            GL.LoadIdentity();
            GL.modelview = camera.worldToCameraMatrix * localToWorld;
            GL.LoadProjectionMatrix(camera.projectionMatrix);

            LineMaterial.SetPass(0);
            GL.Begin(GL.QUADS);

            switch (Orientation)
            {
                case DRAW_ORIENTATION.XY:
                    DrawXY(size, vertices, colors);
                    break;

                case DRAW_ORIENTATION.XZ:
                    DrawXZ(size, vertices, colors);
                    break;
            }

            GL.End();

            GL.PopMatrix();
        }

        private static void DrawXY(float size, IList<Vector4> vertices, IList<Color> colors)
        {
            float half = size * 0.5f;
            for (int i = 0; i < vertices.Count; i++)
            {
                float x = vertices[i].x;
                float y = vertices[i].y;
                float z = vertices[i].z;

                GL.Color(colors[i]);
                GL.Vertex3(x + half, y + half, z);
                GL.Vertex3(x + half, y - half, z);
                GL.Vertex3(x - half, y - half, z);
                GL.Vertex3(x - half, y + half, z);
            }
        }

        private static void DrawXZ(float size, IList<Vector4> vertices, IList<Color> colors)
        {
            float half = size * 0.5f;
            for (int i = 0; i < vertices.Count; i++)
            {
                float x = vertices[i].x;
                float y = vertices[i].y;
                float z = vertices[i].z;

                GL.Color(colors[i]);
                GL.Vertex3(x + half, y, z + half);
                GL.Vertex3(x + half, y, z - half);
                GL.Vertex3(x - half, y, z - half);
                GL.Vertex3(x - half, y, z + half);
            }
        }

    }

}
