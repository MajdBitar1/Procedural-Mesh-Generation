using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;

using UnityEngine;
using Unity.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    [SerializeField, Range(1,50)]
    int resolution = 1;
    Mesh mesh;

    static MeshJobScheduleDelegate[] jobs =
    {
        MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
        MeshJob<SharedSquareGrid, SingleStream>.ScheduleParallel
    };

    public enum MeshType
    {
        SquareGrid, SharedSquareGrid
    };
    [SerializeField]
    MeshType meshType;
    private void Awake()
    {
        mesh = new Mesh {  name = "Procedural Mesh" };
        GetComponent<MeshFilter>().mesh = mesh;
    }
    private void OnValidate() => enabled = true;

    void Update()
    {
        GenerateMesh();
        enabled = false;
    }
    void GenerateMesh()
    {
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshdata = meshDataArray[0];
        // simply replace Multistream with SingleStream
        // to change MeshStream implementation
        jobs[(int)meshType](mesh, meshdata, resolution, default).Complete();
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }
}
