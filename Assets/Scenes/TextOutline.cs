﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextOutline : BaseMeshEffect
{
    public Color OutlineColor = Color.white;
    [Range(0, 6)]
    public int OutlineWidth = 0;

    private static List<UIVertex> m_VetexList = new List<UIVertex>();


    protected override void Awake()
    {
        base.Awake();

        var shader = Shader.Find("TSF Shaders/UI/OutlineEx");
        base.graphic.material = new Material(shader);

        var v1 = base.graphic.canvas.additionalShaderChannels;
        var v2 = AdditionalCanvasShaderChannels.Tangent;
        if ((v1 & v2) != v2)
        {
            base.graphic.canvas.additionalShaderChannels |= v2;
        }
        this._Refresh();
    }


#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        if (base.graphic.material != null)
        {
            this._Refresh();
        }
    }
#endif


    private void _Refresh()
    {
        base.graphic.material.SetColor("_OutlineColor", this.OutlineColor);
        base.graphic.material.SetInt("_OutlineWidth", this.OutlineWidth);
        base.graphic.SetVerticesDirty();
    }


    public override void ModifyMesh(VertexHelper vh)
    {
        vh.GetUIVertexStream(m_VetexList);

        this._ProcessVertices();

        vh.Clear();
        vh.AddUIVertexTriangleStream(m_VetexList);
    }


    private void _ProcessVertices()
    {
        // TODO: 处理顶点
        for (int i = 0, count = m_VetexList.Count - 3; i <= count; i += 3)
        {
            var v1 = m_VetexList[i];
            var v2 = m_VetexList[i + 1];
            var v3 = m_VetexList[i + 2];
            // 计算原顶点坐标中心点
            //
            var minX = _Min(v1.position.x, v2.position.x, v3.position.x);
            var minY = _Min(v1.position.y, v2.position.y, v3.position.y);
            var maxX = _Max(v1.position.x, v2.position.x, v3.position.x);
            var maxY = _Max(v1.position.y, v2.position.y, v3.position.y);
            var posCenter = new Vector2(minX + maxX, minY + maxY) * 0.5f;
            // 计算原始顶点坐标和UV的方向
            //
            Vector2 triX, triY, uvX, uvY;
            Vector2 pos1 = v1.position;
            Vector2 pos2 = v2.position;
            Vector2 pos3 = v3.position;
            if (Mathf.Abs(Vector2.Dot((pos2 - pos1).normalized, Vector2.right))
                > Mathf.Abs(Vector2.Dot((pos3 - pos2).normalized, Vector2.right)))
            {
                triX = pos2 - pos1;
                triY = pos3 - pos2;
                uvX = v2.uv0 - v1.uv0;
                uvY = v3.uv0 - v2.uv0;
            }
            else
            {
                triX = pos3 - pos2;
                triY = pos2 - pos1;
                uvX = v3.uv0 - v2.uv0;
                uvY = v2.uv0 - v1.uv0;
            }
            // 为每个顶点设置新的Position和UV
            //
            v1 = _SetNewPosAndUV(v1, this.OutlineWidth, posCenter, triX, triY, uvX, uvY);
            v2 = _SetNewPosAndUV(v2, this.OutlineWidth, posCenter, triX, triY, uvX, uvY);
            v3 = _SetNewPosAndUV(v3, this.OutlineWidth, posCenter, triX, triY, uvX, uvY);
            // 应用设置后的UIVertex
            //
            m_VetexList[i] = v1;
            m_VetexList[i + 1] = v2;
            m_VetexList[i + 2] = v3;
        }
    }

    private static UIVertex _SetNewPosAndUV(UIVertex pVertex, int pOutLineWidth,
    Vector2 pPosCenter,
    Vector2 pTriangleX, Vector2 pTriangleY,
    Vector2 pUVX, Vector2 pUVY)
    {
        // Position
        var pos = pVertex.position;
        var posXOffset = pos.x > pPosCenter.x ? pOutLineWidth : -pOutLineWidth;
        var posYOffset = pos.y > pPosCenter.y ? pOutLineWidth : -pOutLineWidth;
        pos.x += posXOffset;
        pos.y += posYOffset;
        pVertex.position = pos;
        // UV
        var uv = pVertex.uv0;
        uv += pUVX / pTriangleX.magnitude * posXOffset * (Vector2.Dot(pTriangleX, Vector2.right) > 0 ? 1 : -1);
        uv += pUVY / pTriangleY.magnitude * posYOffset * (Vector2.Dot(pTriangleY, Vector2.up) > 0 ? 1 : -1);
        pVertex.uv0 = uv;

        return pVertex;
    }


    private static float _Min(float pA, float pB, float pC)
    {
        return Mathf.Min(Mathf.Min(pA, pB), pC);
    }


    private static float _Max(float pA, float pB, float pC)
    {
        return Mathf.Max(Mathf.Max(pA, pB), pC);
    }
}
