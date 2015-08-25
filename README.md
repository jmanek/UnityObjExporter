# UnityObjExporter
Exports a gameObject hierarchy to .obj/.mtl
========
An Editor script that parses through a gameObject and all its children, creating an .obj and .mtl file.  Right now it only supports MeshFilters, and material support is limited to diffuse color (_Color) and texture (_MainTex).

### Running

Add the script to an Editor folder.  This will create the menu option File/Export Selected/.obj  
As this suggests, the script starts from the selected gameObject, and traverses through all its children.
Let it run -- I've found it to be decently fast even when creating .obj's with 500k verts (70MB)
The .obj/.mtl and all textures are exported to Assets/objExport/gameObjectName.  
This can be changed by modifying the line:
'''C#
string dir = Path.Combine(Application.dataPath, "objExport/" + gO.name);
'''
Support for additional shader properties can be added by modifying the function BuildMtl:
'''C#
if (m.HasProperty("_Color"))
{
    Color c = m.GetColor("_Color");
    mtl.AppendFormat("Kd {0} {1} {2}", c.r, c.g, c.b).AppendLine();
}
'''
