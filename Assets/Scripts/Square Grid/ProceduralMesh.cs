using ProceduralMeshes;
using ProceduralMeshes.Generators;
using ProceduralMeshes.Streams;
using UnityEngine;
using Unity.Rendering;
using System.Reflection.Emit;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    [SerializeField, Range(1,10)]
    int resolution = 1;
    Mesh mesh;

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
        MeshJob<SquareGrid, MultiStream>.ScheduleParallel(mesh,meshdata,resolution, default).Complete(); 
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }
}
