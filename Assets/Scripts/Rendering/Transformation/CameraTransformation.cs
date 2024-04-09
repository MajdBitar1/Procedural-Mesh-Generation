using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransformation : Transformation
{
    public float focallength = 1f;
   public override Matrix4x4 Matrix
    {
        get
        {
            Matrix4x4 mat = new Matrix4x4();
            mat.SetRow( 0, new Vector4(focallength, 0f, 0f, 0f));
            mat.SetRow( 1, new Vector4(0f, focallength, 0f, 0f));
            mat.SetRow( 2, new Vector4(0f, 0f, 0f, 0f));
            mat.SetRow( 3, new Vector4(0f, 0f, 1f, 0f));
            return mat;
        }
    }
}
