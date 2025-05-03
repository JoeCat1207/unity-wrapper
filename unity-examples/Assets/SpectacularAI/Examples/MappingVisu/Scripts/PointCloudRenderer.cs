namespace SpectacularAI.Examples.MappingVisu
{
    using SpectacularAI.Mapping;
    using UnityEngine;
    using UnityEngine.Rendering;

    public sealed class PointCloudRenderer : MonoBehaviour
    {
        public void Initialize(PointCloud pointCloud, Material material)
        {
            // Generate mesh via TSDF voxelization and marching cubes
            Mesh mesh = TSDFMesher.CreateMeshFromPointCloud(pointCloud);

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
        }
    }
}
