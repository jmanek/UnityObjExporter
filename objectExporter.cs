using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
public class objectExporter : EditorWindow {

    [MenuItem("File/Export Selected/.obj")]
    private static void ExportMesh()
    {
        GameObject gO = Selection.activeGameObject;
        List<Material> mats = new List<Material>();
        string dir = Path.Combine(Application.dataPath, "objExport/" + gO.name);
        Directory.CreateDirectory(dir);
        BuildObj(gO, ref mats, dir);
        BuildMtl(mats, dir, gO.name);
    }

    private static void BuildObj(GameObject gO, ref List<Material> mtls, string dir)
    {
        MeshFilter[] meshFilters = gO.GetComponentsInChildren<MeshFilter>();
        StringBuilder vs = new StringBuilder("mtllib " + gO.name + ".mtl").AppendLine();
        StringBuilder vts = new StringBuilder();
        StringBuilder vns = new StringBuilder();
        StringBuilder fs = new StringBuilder();
        Mesh m;
        MeshFilter mf;
        Material[] mats;
        Material mat;
        Vector3 v;
        int[] tr;
        int o = 1;
        for (int i = 0; i < meshFilters.Length; i++)
        {
            mf = meshFilters[i];
            m = mf.sharedMesh;
            if (mf.gameObject.GetComponent<Renderer>() == null)
            {
                continue;
            }
            mats = mf.gameObject.GetComponent<Renderer>().sharedMaterials;
            for (int j = 0; j < m.vertexCount; j++) 
            {
                v = m.vertices[j];
                v = mf.transform.TransformPoint(v);
                vs.AppendFormat("v {0} {1} {2}", v.x, v.y, v.z).AppendLine();
                v = m.normals[j];
                vns.AppendFormat("vn {0} {1} {2}", v.x, v.y, v.z).AppendLine();
                v = m.uv[j];
                vts.AppendFormat("vt {0} {1}", v.x, v.y).AppendLine();
            }
            for (int u = 0; u < m.subMeshCount; u++)
            {
                mat = mats[u];
                if (!mtls.Contains(mat))
                {
                    mtls.Add(mat);
                }
                fs.AppendFormat("usemtl {0}", mat.name).AppendLine();
                tr = m.GetTriangles(u);
                for (int k = 0; k < tr.Length; k += 3)
                {
                    fs.AppendFormat("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}",
                        tr[k] + o, tr[k + 1] + o, tr[k + 2] + o).AppendLine();
                }
            }
            o += m.vertexCount;
        }
        using (StreamWriter sw = new StreamWriter(Path.Combine(dir, gO.name + ".obj"), false))
        {
            sw.Write(vs.ToString() + vns.ToString() + vts.ToString() + fs.ToString());
        }
    }

    private static void BuildMtl(List<Material> mats, string dir, string name)
    {
        StringBuilder mtl = new StringBuilder();
        foreach (Material m in mats)
        {
            mtl.AppendFormat("newmtl {0}", m.name).AppendLine();
            if (m.HasProperty("_Color"))
            {
                Color c = m.GetColor("_Color");
                mtl.AppendFormat("Kd {0} {1} {2}", c.r, c.g, c.b).AppendLine();
            }
            if (m.HasProperty("_MainTex"))
            {
                string assetPath = AssetDatabase.GetAssetPath(m.GetTexture("_MainTex"));
                string texName = Path.GetFileName(assetPath);
                string exportPath = Path.Combine(dir, texName);
                mtl.AppendFormat("map_Kd {0}", texName).AppendLine();
                if (!File.Exists(exportPath))
                {
                    File.Copy(assetPath, exportPath);
                }
            }

        }
        using (StreamWriter sw = new StreamWriter(Path.Combine(dir, name + ".mtl"), false))
        {
            sw.Write(mtl.ToString());
        }
    }
 }











