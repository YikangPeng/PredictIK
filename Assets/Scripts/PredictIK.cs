using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictIK : MonoBehaviour
{
    
    [Header("迈步长度")]
    public float StepLength;

    [Header("脚位置")]
    //用于碰撞检测点位
    public Transform LeftFoot;
    public Transform RightFoot;

    [Header("重心骨骼")]
    //控制重心变化
    public Transform Bip;

    [Header("脚步IK控制器")]
    public Transform LeftFootContraoller;
    public Transform RightFootContraoller;

    [Header("脚部碰撞层")]
    public LayerMask ColliderMask;

    [Header("抬腿检测高度")]
    //腿能抬的高度
    public float StepHeight = 0.4f;

    [Header("重心延迟时间")]
    public float damptime = 0.1f;

    [Header("重心偏移")]
    public float offsetScale = 0.1f;

    //检测运动轨迹
    public AnimationCurve RightStepCurve;
    public AnimationCurve LeftStepCurve;

    private float LeftCurveTangent = 0.0f;
    private float RightCurveTangent = 0.0f;

    private Vector3 LastLeftPosition;
    private Vector3 LastRightPosition;

    private float LastBipHeight;

    private float vel;

    // Start is called before the first frame update
    void Start()
    {
        LeftStepCurve = new AnimationCurve();
        LeftStepCurve.AddKey(0.0f, 0.0f);
        LeftStepCurve.AddKey(StepLength, 0.0f);
        RightStepCurve = new AnimationCurve();
        RightStepCurve.AddKey(0.0f, 0.0f);
        RightStepCurve.AddKey(StepLength, 0.0f);

        LastLeftPosition = LeftFoot.position;
        LastLeftPosition.y = transform.position.y;
        LastRightPosition = RightFoot.position;
        LastRightPosition.y = transform.position.y;

        LastBipHeight = Bip.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 CurrentBip = Bip.position;
        //对于走路来说，总有一个脚是触地的，如果是有腾空的跑步，需要一个权重的曲线

        //重心总是跟着更低的那只脚移动
        //计算脚的高度
        Vector3 LeftDir = LeftFoot.position - LastLeftPosition;
        float LeftDis = Vector3.Dot(LeftDir, transform.forward);
        LeftDis = Mathf.Clamp(LeftDis, 0.0f, StepLength);
        float LeftH = LeftStepCurve.Evaluate(LeftDis) - LeftStepCurve[0].value;
        //虚拟左脚高度
        float VirtualLeftHeight = LastLeftPosition.y + LeftH;

        Vector3 RightDir = RightFoot.position - LastRightPosition;
        float RightDis = Vector3.Dot(RightDir, transform.forward);
        RightDis = Mathf.Clamp(RightDis, 0.0f, StepLength);
        float RightH = RightStepCurve.Evaluate(RightDis) - RightStepCurve[0].value;
        //虚拟右脚高度
        float VirtualRightHeight = LastRightPosition.y + RightH;

        //重心高度
        float VirtualHeight = Mathf.Min(VirtualLeftHeight, VirtualRightHeight);

        //通过虚拟重心位置计算腰应该在位置
        Vector3 BipOnRoot = Bip.position;
        float BipY = Bip.position.y - transform.position.y;//动画腰的高度
        BipOnRoot.y = VirtualHeight + BipY;
        Bip.position = BipOnRoot;


        //IK控制器位置设置
        Vector3 L = LeftFoot.position;
        L.y = L.y + VirtualLeftHeight - VirtualHeight;
        LeftFootContraoller.position = L;
        Vector3 R = RightFoot.position;
        R.y = R.y + VirtualRightHeight - VirtualHeight;
        RightFootContraoller.position = R;


        //重心的偏移，根据上坡下坡偏移重心
        float offset = (LeftCurveTangent + RightCurveTangent) * offsetScale;

        //重心的延迟
        BipOnRoot.y = Mathf.SmoothDamp(LastBipHeight, VirtualHeight + BipY /*+ offset*/, ref vel, damptime);
        

        LastBipHeight = BipOnRoot.y;
        Bip.position = BipOnRoot;

        
        
    }

    //碰撞检测计算路径
    private Vector3 StepPerdict(Transform foot, ref AnimationCurve curve)
    {
        

        //从脚向下发射线找到当前位置
        RaycastHit StartHit;
        Physics.Raycast(foot.position + Vector3.up * 0.1f, Vector3.down, out StartHit, 0.5f, ColliderMask);

        //Debug.Log(foot.position + "start" + StartHit.point);

        //从目标
        RaycastHit EndHit;
        //从脚下一步的位置上方某个高度向下发射射线，检测落脚的高度
        //Physics.Raycast(StartHit.point + StepLength * transform.forward + new Vector3(0.0f, StepHeight + 0.1f ,0.0f)/*脚下一步的位置上方*/, Vector3.down, out EndHit, StepHeight * 4.0f, ColliderMask);
        Physics.SphereCast(StartHit.point + StepLength * transform.forward + new Vector3(0.0f, StepHeight + 0.1f, 0.0f)/*脚下一步的位置上方*/ , 0.05f, Vector3.down, out EndHit, StepHeight * 4.0f, ColliderMask);

        //以起点和目标点，做一个胶囊体碰撞检测，找到移动过程中的高点
        Vector3 movedir = (EndHit.point - StartHit.point).normalized;
        Vector3 moveright = Vector3.Cross(Vector3.up, movedir).normalized;
        Vector3 moveup = Vector3.Cross(movedir, moveright).normalized;

        RaycastHit CenterHit;
        bool ishit = Physics.CapsuleCast(StartHit.point + moveup * StepHeight + movedir * 0.1f, EndHit.point + moveup * StepHeight - movedir * 0.1f, 0.06f, -moveup ,out CenterHit, StepHeight - 0.2f, ColliderMask);


        //只计算forward方向上的移动，和竖直方向上的高度，构造运动轨迹曲线
        //起始点的高度是上一个曲线的高度差
        float curvestartheight = curve[curve.length - 1].value - curve[0].value;        
        curve = AnimationCurve.Linear(0.0f, curvestartheight, StepLength, curvestartheight + (EndHit.point - StartHit.point).y);
        if (foot == LeftFoot)
        {
            LeftCurveTangent = curve.keys[0].outTangent;
        }
        else
        {
            RightCurveTangent = curve.keys[0].outTangent;
        }
        
        //路径上有障碍物，找到最高点
        if (ishit)
        {
            Debug.DrawLine(StartHit.point + new Vector3(0.0f,0.03f,0.0f), CenterHit.point + new Vector3(0.0f, 0.03f, 0.0f), Color.red, 1.0f);
            Debug.DrawLine(CenterHit.point + new Vector3(0.0f, 0.03f, 0.0f) , EndHit.point + new Vector3(0.0f, 0.03f, 0.0f), Color.red, 1.0f);

            float centerpos = Vector3.Dot(CenterHit.point - StartHit.point , transform.forward);
            //curve.AddKey(centerpos, curvestartheight + (CenterHit.point - StartHit.point).y);

            Keyframe key = new Keyframe();
            key.time = centerpos;
            key.value = (CenterHit.point - StartHit.point).y + curve[0].value;
            key.inTangent = (key.value - curve[0].value) / centerpos;
            key.outTangent = (curve[1].value - key.value) / (StepLength - centerpos);
            curve.AddKey(key);

        }
        else
        {
            Debug.DrawLine(StartHit.point + new Vector3(0.0f, 0.03f, 0.0f), EndHit.point + new Vector3(0.0f, 0.03f, 0.0f), Color.red, 1.0f);
        }
        

        //Debug.Log(StartHit.point + "start" + EndHit.point + "EndHit" );


        return StartHit.point;

    }
    

    //当需要迈腿时，做一次检测
    public void PredictLeftFoot()
    {
        LastLeftPosition = StepPerdict(LeftFoot, ref LeftStepCurve);
    }

    public void PredictRightFoot()
    {
        LastRightPosition = StepPerdict(RightFoot, ref RightStepCurve);
    }

}
