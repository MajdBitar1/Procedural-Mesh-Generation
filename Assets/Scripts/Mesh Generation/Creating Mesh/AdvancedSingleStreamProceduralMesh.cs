using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Mathematics.math;
using System.Runtime.InteropServices;
using UnityEditor.ShaderGraph;

public class AdvancedSingleStreamProceduralMesh : MonoBehaviour
{
    [StructLayout(LayoutKind.Sequential)] // TO BE SURE THAT THE STRUCT IS LAYED OUT IN MEMORY AS WE WANT SINCE UNITY MAY MODIFY THAT TO OPTIMIZE THE CODE(MEMO USAGE)
    struct Vertex
    {
        public float3 position, normal;
        public half4 tangent;
        public half2 uv0;
    }
    void OnEnable()
    {
        int vertexattribcount = 4;
        int vertexcount = 4;
        int triangleIndexCount = 6;

        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshdata = meshDataArray[0];


        // SINGLE STREAM APPROACH, WE USE 4 STREAMS TO STORE POSITION, NORMAL, TANGENT AND UV DATA OF EACH VERTEX
        var vertexattributes = new NativeArray<VertexAttributeDescriptor>
            (vertexattribcount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        vertexattributes[0] = new VertexAttributeDescriptor(dimension: 3, stream: 0);
        vertexattributes[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
        vertexattributes[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, VertexAttributeFormat.Float16, dimension: 4);
        vertexattributes[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, dimension: 2);

        vertexattributes.Dispose();

        NativeArray<Vertex> vertices = meshdata.GetVertexData<Vertex>();

        half h0 = half(0f), h1 = half(1f);

        var vertex = new Vertex
        {
            normal = back(),
            tangent = half4(h1, h0, h0, half(-1f))
        };

        vertex.position = 0f;
        vertex.uv0 = h0;
        vertices[0] = vertex;

        vertex.position = right();
        vertex.uv0 = half2(h1, h0);
        vertices[1] = vertex;

        vertex.position = up();
        vertex.uv0 = half2(h0, h1);
        vertices[2] = vertex;

        vertex.position = float3(1f, 1f, 0f);
        vertex.uv0 = h1;
        vertices[3] = vertex;

        meshdata.SetVertexBufferParams(vertexcount, vertexattributes);
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
