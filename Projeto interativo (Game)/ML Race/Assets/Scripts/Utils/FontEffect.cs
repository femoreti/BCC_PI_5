using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FontEffect : BaseMeshEffect
{
    private Vector2[] outlineOffsets = new Vector2[] {
        new Vector2(+1, +0).normalized ,
        new Vector2(+1, +1).normalized ,
        new Vector2(+0, +1).normalized ,
        new Vector2(-1, +1).normalized ,
        new Vector2(-1, +0).normalized ,
        new Vector2(-1, -1).normalized ,
        new Vector2(+0, -1).normalized ,
        new Vector2(+1, -1).normalized };

    public Color outline;
    public float outlineOffset;
    public Color shadow;
    public float shadowOffset;
    [Range(0, 360)]
    public float shadowAngle;
    public Color[] overlay = new Color[4];

    public override void ModifyMesh(VertexHelper prego)
    {
        if (!IsActive())
            return;

        List<UIVertex> uiVerts = new List<UIVertex>();
        prego.GetUIVertexStream(uiVerts);
        UIVertex[] original = uiVerts.ToArray();
        int originalLength = original.Length;

        uiVerts.Clear();
        UIVertex[] temp;

        if (shadowOffset != 0) //Desliga o shadow
        {
            temp = new UIVertex[originalLength];
            original.CopyTo(temp, 0);
            Vector3 offset = Quaternion.AngleAxis(shadowAngle - transform.eulerAngles.z, Vector3.forward) * Vector2.right * shadowOffset;
            for (int x = 0; x < temp.Length; x++)
            {
                temp[x].position += offset;
                Color c = shadow;
                c.a *= ((Color)original[x].color).a;
                temp[x].color = c;
            }
            uiVerts.AddRange(temp);
        }

        if (outlineOffset != 0) //Desliga o outline
        {
            for (int n = 0; n < outlineOffsets.Length; n++)
            {
                temp = new UIVertex[originalLength];
                original.CopyTo(temp, 0);
                for (int x = 0; x < temp.Length; x++)
                {
                    temp[x].position += (Vector3)(outlineOffsets[n] * outlineOffset);
                    Color c = outline;
                    c.a *= ((Color)original[x].color).a;
                    temp[x].color = c;
                }
                uiVerts.AddRange(temp);
            }
        }

        uiVerts.AddRange(original);

        if (overlay.Length > 0) //Desliga o overlay de cor
        {
            temp = new UIVertex[originalLength];
            original.CopyTo(temp, 0);
            for (int x = 0; x < temp.Length; x++)
            {
                Color c = overlay[x % overlay.Length];
                c.a *= ((Color)original[x].color).a;
                temp[x].color = c;
            }
            uiVerts.AddRange(temp);
        }

        prego.Clear();
        prego.AddUIVertexTriangleStream(uiVerts);
    }
}