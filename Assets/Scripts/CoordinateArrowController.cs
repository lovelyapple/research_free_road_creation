using UnityEngine;

/// <summary>
/// 単純に座標軸の矢印を生成・保持し、
/// Show/Hide で表示切替できるコントローラ。
/// Knot ピック後などに使う。
/// </summary>
public class CoordinateArrowController : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] private float axisLength = 0.8f;
    [SerializeField] private float axisRadius = 0.03f;

    private Transform _root;
    private Transform _x, _y, _z;

    private void Awake()
    {
        BuildArrows();
        BuildPlanes();
        SetVisible(true);
    }
    [ContextMenu("Show")]
    public void Show()
    {
        Show(Vector3.zero);
    }
    /// <summary>矢印を指定位置に表示</summary>
    public void Show(Vector3 worldPos)
    {
        if (_root == null) return;
        _root.position = worldPos;
        SetVisible(true);
    }

    /// <summary>非表示にする</summary>
    public void Hide() => SetVisible(false);

    /// <summary>表示状態の切替</summary>
    private void SetVisible(bool visible)
    {
        if (_root != null) _root.gameObject.SetActive(visible);
    }

    private void BuildArrows()
    {
        _root = new GameObject("CoordinateArrows").transform;
        _root.SetParent(transform, false);

        _x = CreateAxis("X", Color.red, Vector3.right);
        _y = CreateAxis("Y", Color.green, Vector3.up);
        _z = CreateAxis("Z", Color.blue, Vector3.forward);
    }

    private Transform CreateAxis(string name, Color color, Vector3 dir)
    {
        var parent = new GameObject(name + "_Axis").transform;
        parent.SetParent(_root, false);
        parent.localRotation = Quaternion.FromToRotation(Vector3.up, dir);

        // Shaft
        var cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cyl.name = name + "_Shaft";
        cyl.transform.SetParent(parent, false);
        cyl.transform.localScale = new Vector3(axisRadius * 2f, axisLength * 0.5f, axisRadius * 2f);
        cyl.transform.localPosition = new Vector3(0, axisLength * 0.5f, 0);
        SetupPart(cyl, color);

        // Tip
        var tip = GameObject.CreatePrimitive(PrimitiveType.Cylinder); // Unity 2022以降はPrimitiveType.Coneが使える
        if (tip == null) tip = GameObject.CreatePrimitive(PrimitiveType.Sphere); // fallback
        tip.name = name + "_Tip";
        tip.transform.SetParent(parent, false);
        tip.transform.localScale = Vector3.one * (axisRadius * 3f);
        tip.transform.localPosition = new Vector3(0, axisLength, 0);
        SetupPart(tip, color);

        return parent;
    }

    private void SetupPart(GameObject go, Color color)
    {
        Destroy(go.GetComponent<Collider>()); // 当たり判定不要なら削除
        var mr = go.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Unlit/Color")) { color = color };
    }
    private Transform _xyPlane, _xzPlane, _yzPlane;

    private void BuildPlanes()
    {
        // _xyPlane = CreatePlaneHandle("XY", Color.yellow, Vector3.back, false);  // Z軸正面を向く
        _xzPlane = CreatePlaneHandle("XZ", Color.green, Vector3.down, true);        // Y軸正面を向く
        // _yzPlane = CreatePlaneHandle("YZ", Color.red, Vector3.left, false);       // X軸正面を向く
    }

    private Transform CreatePlaneHandle(string name, Color color, Vector3 normal, bool useOutLine)
    {
        var go = new GameObject("_Plane");
        go.transform.SetParent(_root, false);
        go.transform.localRotation = Quaternion.LookRotation(Vector3.forward, normal);
        go.transform.localScale = Vector3.one * axisLength * 0.5f;

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mf.sharedMesh = CreateQuadWithNormals();

        if (!mr) mr = go.AddComponent<MeshRenderer>();
 
        var baseColor = new Color(color.r, color.g, color.b, 0.3f); // 半透明

        if (useOutLine)
        {
            // アウトライン付きマテリアルを作成
            var mat = new Material(Shader.Find("Custom/OutlinedUnlit"));
            mat.SetColor("_BaseColor", new Color(baseColor.r, baseColor.g, baseColor.b, 0.35f)); // 半透明本体
            mat.SetColor("_OutlineColor", baseColor * 1.5f);  // 外縁色（HDR推奨、後で調整OK）
            mat.SetFloat("_OutlineWidth", 0.3f);             // 外縁の太さ（m）
            mat.renderQueue = 2999;
            mr.sharedMaterial = mat;
        }
        else
        {
            mr.material = new Material(Shader.Find("Unlit/Color"));
        }

        Destroy(go.GetComponent<Collider>()); // 当たり判定不要なら削除
        return go.transform;
    }

    Mesh CreateQuadWithNormals()
    {
        var mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
        new Vector3(-0.5f, -0.5f, 0),
        new Vector3( 0.5f, -0.5f, 0),
        new Vector3(-0.5f,  0.5f, 0),
        new Vector3( 0.5f,  0.5f, 0),
        };
        mesh.normals = new Vector3[]
        {
        Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward
        };
        mesh.uv = new Vector2[]
        {
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
        };
        mesh.triangles = new int[]
        {
        0, 2, 1, 2, 3, 1
        };
        return mesh;
    }
}
