using Unity.Mathematics;
using UnityEngine;

namespace ProceduralMeshes
{
    public interface IMeshStreams {

        void Setup(Mesh.MeshData data,Bounds bounds, int vertexCount, int indexCount);
        void SetVertex(int index, Vertex vertex);
        void SetTrinagle(int index, int3 triangle);
    }
}