using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTransformation : Transformation
{
    public Vector3 position;

    public override Matrix4x4 Matrix
    {
        get
        {
            Matrix4x4 mat = new Matrix4x4();
            mat.SetRow(0, new Vector4 (1f,0f,0f, position.x));
            mat.SetRow(1, new Vector4(0f, 1f, 0f, position.y));
            mat.SetRow(2, new Vector4(0f, 0f, 1f, position.z));
            mat.SetRow(3, new Vector4(0f, 0f, 0f, 1));
            return mat;
        }
    }
}
