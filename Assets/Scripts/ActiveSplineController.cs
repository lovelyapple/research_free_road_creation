using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class ActiveSplineController : MonoBehaviour
{
    [SerializeField] SplineContainer container;

    [SerializeField] Camera _cam;
    [SerializeField] float pickTolerance = 1f;

    public SplineKnotIndex? CurrentSelection { get; private set; }
    public Vector3 CurrentKnotWorldPos { get; private set; }

    [SerializeField] SplineKnotHandle _splineKnotHandle;

    void Update()
    {
        // サンプル: 左クリックでピック → マウス中央ボタンで少し移動
        if (Input.GetMouseButtonDown(0))
        {
            if (TryPickKnot(out _splineKnotHandle))
            {
                Debug.Log($"Picked S={_splineKnotHandle.SplineIndex}, K={_splineKnotHandle.KnotIndex}, pos={_splineKnotHandle.GetWorldPosition()}");
            }
            else
            {
                _splineKnotHandle = null;
            }
        }

        // 例: 少しだけ上に持ち上げる
        if (Input.GetMouseButton(3) && _splineKnotHandle != null)
        {
            var p = _splineKnotHandle.GetWorldPosition();
            p += Vector3.up * 0.25f;
            _splineKnotHandle.SetWorldPosition(p);
        }
    }
    /// <summary>
    /// マウス下の最寄り Knot をピックして返す。見つからなければ false。
    /// </summary>
    public bool TryPickKnot(out SplineKnotHandle handle)
    {
        handle = default;

        if (!_cam || !container || container.Splines.Count == 0)
            return false;

        var ray = _cam.ScreenPointToRay(Input.mousePosition);

        float bestDist = float.PositiveInfinity;
        int bestSpline = -1;
        int bestKnot = -1;

        for (int si = 0; si < container.Splines.Count; si++)
        {
            var spline = container.Splines[si];
            for (int ki = 0; ki < spline.Count; ki++)
            {
                var knot = spline[ki];
                var worldPos = container.transform.TransformPoint((Vector3)knot.Position);
                float d = DistancePointRay(worldPos, ray);
                if (d < bestDist)
                {
                    bestDist = d;
                    bestSpline = si;
                    bestKnot = ki;
                }
            }
        }

        if (bestSpline >= 0 && bestDist <= pickTolerance)
        {
            handle = new SplineKnotHandle(container, bestSpline, bestKnot);
            return true;
        }

        return false;
    }

    // 点とRayの最近距離（Rayの手前側は原点まで）
    static float DistancePointRay(Vector3 point, Ray ray)
    {
        Vector3 v = point - ray.origin;
        Vector3 dir = ray.direction.normalized;
        float d = Vector3.Dot(v, dir);
        if (d <= 0f) return v.magnitude;
        Vector3 proj = ray.origin + dir * d;
        return Vector3.Distance(point, proj);
    }
    /// <summary>
    /// ランタイムで Knot を直接読み書きできる軽量ハンドル。
    /// </summary>
    [Serializable]
    public class SplineKnotHandle
    {
        public  SplineContainer Container;
        public  int SplineIndex;
        public  int KnotIndex;

        public SplineKnotHandle(SplineContainer c, int s, int k)
        {
            Container = c;
            SplineIndex = s;
            KnotIndex = k;

            foreach(var spline in Container.Splines)
            {
                SetSplineTangentModeBroken(spline);
            }
        }

        /// 現在の Knot ワールド座標を取得
        public Vector3 GetWorldPosition()
        {
            var k = Container.Splines[SplineIndex][KnotIndex];
            return Container.transform.TransformPoint((Vector3)k.Position);
        }

        /// Knot のワールド座標を書き換え
        public void SetWorldPosition(Vector3 worldPos)
        {
            var spline = Container.Splines[SplineIndex];
            var k = spline[KnotIndex]; // BezierKnot は値型
            k.Position = (float3)Container.transform.InverseTransformPoint(worldPos);
            spline[KnotIndex] = k;     // 書き戻し
        }

        private static void SetSplineTangentModeBroken(Spline spline)
        {
            // spline.get
            var count = spline.Knots.Count();

            for (int i = 0; i < count; i++)
            {
                spline.SetTangentMode(i, TangentMode.Broken);
            }
        }
    }
}
