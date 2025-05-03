namespace SpectacularAI.Examples.MappingVisu
{
    using SpectacularAI.Mapping;
    using UnityEngine;
    using UnityEngine.Rendering;

    public sealed class PointCloudRenderer : MonoBehaviour
    {
        public void Initialize(PointCloud pointCloud, Material material)
        {
            Mesh mesh = new Mesh
            {
                indexFormat = pointCloud.Size > 65535 ? IndexFormat.UInt32 : IndexFormat.UInt16
            };

            // Set vertices
            mesh.SetVertices(pointCloud.Positions, 0, pointCloud.Size);

            // Generate triangle indices by grouping every three points
            int triangleCount = pointCloud.Size / 3;
            int[] indices = new int[triangleCount * 3];
            for (int i = 0; i < triangleCount; ++i)
            {
                indices[i * 3] = i * 3;
                indices[i * 3 + 1] = i * 3 + 1;
                indices[i * 3 + 2] = i * 3 + 2;
            }
            mesh.SetIndices(indices, MeshTopology.Triangles, 0);

            // Normals for shading
            if (pointCloud.HasNormals)
            {
                mesh.SetNormals(pointCloud.Normals, 0, pointCloud.Size);
            }
            else
            {
                mesh.RecalculateNormals();
            }

            // Optional colors
            if (pointCloud.HasColors)
            {
                mesh.SetColors(pointCloud.Colors, 0, pointCloud.Size);
            }

            mesh.UploadMeshData(false);

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
        }
    }
}
