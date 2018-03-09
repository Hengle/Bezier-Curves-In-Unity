using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierCurves
{

    public enum DRAW_ORIENTATION { XY, XZ };

    public class DrawBase
    {
        public static DRAW_ORIENTATION Orientation = DRAW_ORIENTATION.XY;

        protected static List<Vector4> m_vertices = new List<Vector4>();
        protected static List<Color> m_colors = new List<Color>();

        private static Material m_lineMaterial;
        protected static Material LineMaterial
        {
            get
            {
                if (m_lineMaterial == null)
                    m_lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                return m_lineMaterial;
            }
        }

    }

}
