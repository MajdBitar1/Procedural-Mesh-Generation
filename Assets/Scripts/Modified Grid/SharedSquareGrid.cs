using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{
    public struct SharedSquareGrid : IMeshGenerator
    {
        public int Resolution { get; set; }
        public int VertexCount => 4 * Resolution * Resolution;

        public int IndexCount => 6 * Resolution * Resolution;

        public int JobLength => 1 * Resolution;

        public void Execute<S>(int z, S streams) where S : struct, IMeshStreams
        {
            int vi = 4 * Resolution * z, ti = 2 * Resolution * z;

            for (int x = 0; x < Resolution; x++, vi +=4, ti +=2)
            {
                //var coordinates = float4(x, x + 1f, z, z + 1f) / Resolution - 0.5f;
                var xCoordiantes = float2(x,x+1f) / Resolution - 0.5f;
                var zCoordinates = float2(z,z+1f) / Resolution - 0.5f;

                var vertex = new Vertex();

                vertex.position.x = xCoordiantes.x;
                vertex.position.z = zCoordinates.x;
                vertex.normal.y = 1f;
                vertex.tangent.xw = float2(1f, -1f);
                streams.SetVertex(vi + 0, vertex);

                vertex.uv0 = float2(1f, 0f);
                vertex.position.x = xCoordiantes.y;
                streams.SetVertex(vi + 1, vertex);

                vertex.uv0 = float2(0f, 1f);
                vertex.position.x = xCoordiantes.x;
                vertex.position.z = zCoordinates.y;
                streams.SetVertex(vi + 2, vertex);

                vertex.position.x = xCoordiantes.y;
                vertex.uv0 = 1f;
                streams.SetVertex(vi + 3, vertex);

                streams.SetTrinagle(ti + 0, vi + int3(0, 2, 1));
                streams.SetTrinagle(ti + 1, vi + int3(1, 2, 3));
            }
        }
        public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f,0f,1f));
    };
}
