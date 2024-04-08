using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;
using Unity.Mathematics;
using static Unity.Mathematics.math;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedMultiStreamProceduralMesh : MonoBehaviour
{
    void OnEnable()
    {
        int vertexattribcount = 4;
        int vertexcount = 4;
        int triangleIndexCount = 6;

        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshdata = meshDataArray[0];


        // MULTI STREAM APPROACH, WE USE 4 STREAMS TO STORE POSITION, NORMAL, TANGENT AND UV DATA OF EACH VERTEX
        var vertexattributes = new NativeArray<VertexAttributeDescriptor>
            (vertexattribcount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        vertexattributes[0] = new VertexAttributeDescriptor(dimension: 3);
        vertexattributes[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3, stream: 1);
        vertexattributes[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent,VertexAttributeFormat.Float16, dimension: 4, stream: 2);
        vertexattributes[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, dimension: 2, stream: 3);

        meshdata.SetVertexBufferParams(vertexcount, vertexattributes);
        vertexattributes.Dispose();

        NativeArray<float3> positions = meshdata.GetVertexData<float3>();
        positions[0] = 0f;
        positions[1] = right();
        positions[2] = up();
        positions[3] = float3(1f,1f,0f);

        NativeArray<float3> normals = meshdata.GetVertexData<float3>(1);
        normals[0] = normals[1] = normals[2] = normals[3] = back();

        // Usually tangents data are float4 and UV data is float2,
        // since we are not using a very details model
        // we can use half4 and half2 to save memory
        // we can't do that for normal and position data since
        // the values must be a multiple of 4
        // but switching them will cause their total size to be three times 2 bytes which is 6 bytes which is not a multiple of 4

        half h0 = half(0f), h1 = half(1f);

        NativeArray<half4> tangents = meshdata.GetVertexData<half4>(2);
        tangents[0] = tangents[1] = tangents[2] = tangents[3] = half4(h1, h0, h0, half(-1f));

        NativeArray<half2> uv = meshdata.GetVertexData<half2>(3);
        uv[0] = h0;
        uv[1] = half2(h1,h0);
        uv[2] = half2(h0,h1);
        uv[3] = h1;

        meshdata.SetIndexBufferParams(triangleIndexCount, IndexFormat.UInt32); //using UInt32 allows for more indices than UInt16, but uses more memory

        NativeArray<uint> triangleIndices = meshdata.GetIndexData<uint>(); // we can use ushort if we know we have less than 65536 vertices in our mesh similar to UInt32 and 16
        triangleIndices[0] = 0;
        triangleIndices[1] = 2;
        triangleIndices[2] = 1;
        triangleIndices[3] = 1;
        triangleIndices[4] = 2;
        triangleIndices[5] = 3;

        var bounds = new Bounds(new Vector3(.5f, .5f), new Vector3(1f, 1f));

        meshdata.subMeshCount = 1;
        meshdata.SetSubMesh(0, new SubMeshDescriptor(0, triangleIndexCount)
            {
            bounds = bounds,
            vertexCount = vertexcount
            }, MeshUpdateFlags.DontRecalculateBounds);

        var m_mesh = new Mesh()
        {
            bounds = bounds,
            name = "AdvancedMultiStreamProceduralMesh"
        };
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, m_mesh);   
        GetComponent<MeshFilter>().mesh = m_mesh;
    }
}
