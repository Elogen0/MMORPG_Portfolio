using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class NavmeshExporter
{
    [MenuItem("Tools/NavMesh/Export")]
    static void Export()
    {
        Debug.Log("ExportNavMesh");

        NavMeshTriangulation tmpNavMeshTriangulation = NavMesh.CalculateTriangulation();

        //    
        string tmpPath = Application.dataPath + "/" + SceneManager.GetActiveScene().name + ".obj";
        StreamWriter tmpStreamWriter = new StreamWriter(tmpPath);

        //  
        for (int i=0;i<tmpNavMeshTriangulation.vertices.Length;i++)
        {
            tmpStreamWriter.WriteLine("v  "+ tmpNavMeshTriangulation.vertices[i].x+" "+ tmpNavMeshTriangulation.vertices[i].y+" "+ tmpNavMeshTriangulation.vertices[i].z);
        }

        tmpStreamWriter.WriteLine("g pPlane1");

        //  
        for (int i = 0; i < tmpNavMeshTriangulation.indices.Length;)
        {
            tmpStreamWriter.WriteLine("f " + (tmpNavMeshTriangulation.indices[i]+1) + " " + (tmpNavMeshTriangulation.indices[i+1]+1) + " " + (tmpNavMeshTriangulation.indices[i+2]+1));
            i = i + 3;
        }

        tmpStreamWriter.Flush();
        tmpStreamWriter.Close();

        Debug.Log("ExportNavMesh Success");
    }
}