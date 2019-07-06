using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraController : MonoBehaviour
{
    public float LockRadius = 7f;
    private GameObject playerHandle;//y轴控制器
    private GameObject cameraHandle;//x轴控制器
    public LayerMask layerMask;
    public Image lockDot;
    [SerializeField]
    private LockTarget lockTarget ;
    public bool LockState =false;
    public float cameraDamp = 0.02f;
    private IUserInput PI;
    private GameObject model;
    private float tempRotateX;
    private GameObject m_camera;
    private Vector3 cameraVelocity;
    private void Awake()
    {
        m_camera = Camera.main.gameObject;
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempRotateX = cameraHandle.transform.localEulerAngles.x;
        ActorController ac = playerHandle.GetComponent<ActorController>();
        model = ac.model;
        PI = ac.pi;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (!LockState)
        {
            Vector3 modelEuler = model.transform.eulerAngles;
            playerHandle.transform.Rotate(Vector3.up, PI.Jright * 100f * Time.fixedDeltaTime);
            model.transform.eulerAngles = modelEuler;
            tempRotateX -= PI.Jup * 100f * Time.fixedDeltaTime;
            tempRotateX = Mathf.Clamp(tempRotateX, -25, 45);
            cameraHandle.transform.localEulerAngles = new Vector3(tempRotateX, 0, 0);

        }
        else
        {
            Vector3 temp = lockTarget.obj.transform.position - model.transform.position;
            temp.y = 0;
            playerHandle.transform.forward =temp ;
            cameraHandle.transform.LookAt(lockTarget.obj.transform);
            tempRotateX = cameraHandle.transform.localEulerAngles.x;

        }
        lockDot.enabled = LockState;
            //m_camera.transform.position = Vector3.SmoothDamp(m_camera.transform.position, transform.position, ref cameraVelocity, cameraDamp);
        m_camera.transform.position = Vector3.Lerp(m_camera.transform.position, transform.position, cameraDamp);
        //m_camera.transform.rotation = Quaternion.Slerp(m_camera.transform.rotation, transform.rotation, cameraDamp);
        m_camera.transform.LookAt(cameraHandle.transform);

    }

    private void Update()
    {
        if(LockState)
        {
            lockDot.transform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position + Vector3.up * lockTarget.halfHeight);
            if (Vector3.Distance(lockTarget.obj.transform.position, playerHandle.transform.position) > LockRadius)
            {
                lockTarget = null;
                LockState = false;
            }
        }
    }
    public void LockUnlock()
    {
        Collider[] cols = Physics.OverlapSphere(model.transform.position, LockRadius,layerMask);

        if(cols.Length==0){
            lockTarget =null;
            LockState = false;
            return;
        }
        Debug.Log("aaaaaa");

        int min = -1;
        float mindis = 999;
        for(int i = 0; i < cols.Length; i++)
        {
            if (cols[i].transform.GetChild(0).GetComponent<Renderer>().isVisible == false)//会出bug 如果碰撞体没有渲染组件就会
                continue;
            float dis= Vector3.SqrMagnitude(cols[i].gameObject.transform.position - playerHandle.transform.position);
            if (dis < mindis)
            {
                mindis = dis;
                min = i;
            }
        }
        if (min == -1||(lockTarget !=null&& cols[min].gameObject == lockTarget.obj))
        {
            LockState = false;
            lockTarget = null;
        }
        else
        {
            LockState = true;
            lockTarget = new LockTarget( cols[min].gameObject,cols[min].bounds.extents.y);
        }

    }
    /// <summary>
    /// 查找所有在范围内的敌人
    /// 判断与当前锁定目标的距离是否为最近的，且在right方向的敌人
    /// 
    /// </summary>
    /// <param name="dir"></param>
    public void ChangeLock(int dir)
    {
        Collider[] cols = Physics.OverlapSphere(model.transform.position, LockRadius, layerMask);
        float minAngle = 120f;
        int min=-1;

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject == lockTarget.obj)
                continue;
            Vector3 targetDir = cols[i].transform.position - playerHandle.transform.position;

            float angle = Vector3.SignedAngle(playerHandle.transform.forward, targetDir, Vector3.up);
            if (angle > 120 || angle < -120 || angle * dir < 0)
                continue;
            angle = Mathf.Abs(angle);
            if (angle < minAngle)
            {
                min = i;
                minAngle = angle;
            }
            else if(angle == minAngle)
            {
                float dis = Vector3.Distance(playerHandle.transform.position, cols[min].transform.position);
                float NextDis = targetDir.magnitude;
                if (NextDis < dis)
                    min = i;
            }
        }
        if(min>=0)
            lockTarget = new LockTarget(cols[min].gameObject, cols[min].bounds.extents.y);

    }
    [System.Serializable]
    private class LockTarget
    {
        public GameObject obj;
        public float halfHeight;
        public LockTarget(GameObject obj, float halfHeight)
        {
            this.obj = obj;
            Debug.Log(halfHeight);
            this.halfHeight = halfHeight;
        }
    }
}
