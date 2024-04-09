using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;

namespace ProceduralMeshes.Streams
{
    public struct MultiStream : IMeshStreams {

        [NativeDisableContainerSafetyRestriction]
        NativeArray<float3> stream0,stream1;
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float4> stream2;
        [NativeDisableContainerSafetyRestriction]
        NativeArray<float2> stream3;

        [NativeDisableContainerSafetyRestriction]
        NativeArray<TriangleUInt16> triangles;
        public void Setup (Mesh.MeshData meshdata,Bounds bounds, int vertexCount, int indexCount)
        {
            var descriptor = new NativeArray<VertexAttributeDescriptor> (
                4,Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            descriptor[0] = new VertexAttributeDescriptor(dimension: 3);
            descriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal,stream: 1,dimension: 3);
            descriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, stream: 2,dimension: 4);
            descriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 3,dimension: 2);
            meshdata.SetVertexBufferParams(vertexCount, descriptor);
            descriptor.Dispose();

            meshdata.SetIndexBufferParams(indexCount, IndexFormat.UInt16); 
            meshdata.subMeshCount = 1;
            meshdata.SetSubMesh(
                0, new SubMeshDescriptor(0, indexCount)
                {
                    bounds = bounds,
                    vertexCount = vertexCount
                },
                MeshUpdateFlags.DontRecalculateBounds | MeshUpdateFlags.DontValidateIndices
                );
            stream0 = meshdata.GetVertexData<float3>();
            stream1 = meshdata.GetVertexData<float3>(1);
            stream2 = meshdata.GetVertexData<float4>(2);
            stream3 = meshdata.GetVertexData<float2>(3);
            triangles = meshdata.GetIndexData<ushort>().Reinterpret<TriangleUInt16>(2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetVertex (int index, Vertex vertex)
        {
            stream0[index] = vertex.position;
            stream1[index] = vertex.normal;
            stream2[index] = vertex.tangent;
            stream3[index] = vertex.uv0;
        }

        public void SetTriangle (int index, int3 triangle) => triangles[index] = triangle;
    
    }
}