using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.VR;

public class MoveObjectWithTrigger : MonoBehaviour
{
    public SteamVR_Input_Sources leftInputSource = SteamVR_Input_Sources.LeftHand; // 手柄输入源
    public SteamVR_Input_Sources rightInputSource = SteamVR_Input_Sources.RightHand; // 手柄输入源

    public SteamVR_Action_Boolean triggerAction = SteamVR_Actions.default_GrabPinch; // 扳机键输入动作
    public SteamVR_Action_Boolean gripAction = SteamVR_Actions.default_GrabGrip; // 手柄抓取输入动作
    public SteamVR_Action_Pose poseAction = SteamVR_Actions.default_Pose; // 手柄位置和姿态动作

    public SteamVR_Action_Boolean snapLeftAction = SteamVR_Input.GetBooleanAction("SnapTurnLeft");
    public SteamVR_Action_Boolean snapRightAction = SteamVR_Input.GetBooleanAction("SnapTurnRight");

    public float snapAngle = 90.0f;

    public float movementSpeed = 1.0f; // 手柄移动对应物体的移动速度
    public VRUpdatePath UpdatePath;

    private Vector3 initLeftPosition;
    private Vector3 initRightPosition;
    private Vector3 twoHandDistance;
    
    private Transform pathTransform;

    bool isLeftMoveNow = false;
    bool isRightMoveNow = false;

    private void Start()
    {
        initLeftPosition = poseAction.GetLocalPosition(leftInputSource);
        initRightPosition = poseAction.GetLocalPosition(rightInputSource);
        twoHandDistance = initLeftPosition - initRightPosition;
        pathTransform = UpdatePath.GetComponent<Transform>();
    }

    private void Update()
    {
        bool leftHandTurnLeft = snapLeftAction.GetStateDown(leftInputSource);
        bool rightHandTurnLeft = snapLeftAction.GetStateDown(rightInputSource);

        bool leftHandTurnRight = snapRightAction.GetStateDown(leftInputSource);
        bool rightHandTurnRight = snapRightAction.GetStateDown(rightInputSource);

        if (leftHandTurnLeft)
        {
            RotatePath(-snapAngle);
        }
        else if (leftHandTurnRight)
        {
            RotatePath(snapAngle);
        }


        BothHandGrip();
        BothHandTrigger();        
        LeftHandTrigger();
        RightHandTrigger();

        if (!triggerAction.GetState(rightInputSource) && !triggerAction.GetState(leftInputSource) &&
    !gripAction.GetState(rightInputSource) && !gripAction.GetState(leftInputSource)
    )
        {
            isLeftMoveNow = false;
            isRightMoveNow = false;
            initLeftPosition = poseAction.GetLocalPosition(leftInputSource);
            initRightPosition = poseAction.GetLocalPosition(rightInputSource);
            twoHandDistance = initLeftPosition - initRightPosition;
        }
    }


    private Coroutine rotateCoroutine;
    public void RotatePath(float angle)
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            //AllOff();
        }
        rotateCoroutine = StartCoroutine(DoRotatePlayer(angle));
        //pathTransform.Rotate(Vector3.up, angle);
    }

    private IEnumerator DoRotatePlayer(float angle)
    {
        float elapsedTime = 0.0f;
        float duration = 0.5f;
        Vector3 startRotation = pathTransform.eulerAngles;
        Vector3 endRotation = startRotation + new Vector3(0.0f, angle, 0.0f);

        while (elapsedTime < duration)
        {
            pathTransform.eulerAngles = Vector3.Lerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        pathTransform.eulerAngles = endRotation;
    }


    public void LeftHandTrigger()
    {
        // 检测扳机键是否被按下
        if (triggerAction.GetState(leftInputSource) && !triggerAction.GetState(rightInputSource))
        {
            if (!isLeftMoveNow)
            {
                isLeftMoveNow = true;
                initLeftPosition = poseAction.GetLocalPosition(leftInputSource);
            }
            // 获取当前手柄位置
            Vector3 currentControllerPosition = poseAction.GetLocalPosition(leftInputSource);
            float yOffset = currentControllerPosition.y - initLeftPosition.y;


            STCBox.instance.nowDate = STCBox.instance.nowDate.AddMinutes(yOffset * 20);
            UpdatePath.UpdateEveryPath();
        }

        //if (!triggerAction.GetState(leftInputSource))
        //{
        //    isLeftMoveNow = false;
        //    initLeftPosition = poseAction.GetLocalPosition(leftInputSource);
        //}
    }

    public void RightHandTrigger()
    {
        
        if (!triggerAction.GetState(leftInputSource) && triggerAction.GetState(rightInputSource))
        {
            Debug.Log("RightHand");
            if (!isRightMoveNow)
            {
                isRightMoveNow = true;
                initRightPosition = poseAction.GetLocalPosition(rightInputSource);
            }
            // 获取当前手柄位置
            Vector3 currentControllerPosition = poseAction.GetLocalPosition(rightInputSource);
            Vector3 offset = currentControllerPosition - initRightPosition;

            // 更新物体的位置
            Vector3 newPosition = pathTransform.position;
            newPosition.x -= offset.x * movementSpeed;
            newPosition.z -= offset.z * movementSpeed;
            pathTransform.position = newPosition;
            //UpdatePath.UpdateEveryPath();
        }

        //if (!triggerAction.GetState(rightInputSource))
        //{
        //    isRightMoveNow = false;
        //    initRightPosition = poseAction.GetLocalPosition(rightInputSource);
        //}
    }

    public void BothHandTrigger()
    {
        if (triggerAction.GetState(leftInputSource) && triggerAction.GetState(rightInputSource))
        {
            if (!isLeftMoveNow && !isRightMoveNow)
            {
                isLeftMoveNow = true;
                isRightMoveNow = true;
                initLeftPosition = poseAction.GetLocalPosition(leftInputSource);
                initRightPosition = poseAction.GetLocalPosition(rightInputSource);
                twoHandDistance = initLeftPosition - initRightPosition;
            }
            // Get the initial positions of both controllers

            // Calculate the distance between the two controllers in the Y-axis
            float distanceY = Mathf.Abs(poseAction.GetLocalPosition(leftInputSource).y - poseAction.GetLocalPosition(rightInputSource).y) - Mathf.Abs(twoHandDistance.y);

            STCBox.instance.yScale += distanceY * 0.001f;
            UpdatePath.UpdateEveryPath();
        }
        //if (!triggerAction.GetState(rightInputSource) && !triggerAction.GetState(leftInputSource) &&
        //    !gripAction.GetState(rightInputSource) && !gripAction.GetState(leftInputSource)
        //    )
        //{
        //    isLeftMoveNow = false;
        //    isRightMoveNow = false;
        //    initLeftPosition = poseAction.GetLocalPosition(leftInputSource);
        //    initRightPosition = poseAction.GetLocalPosition(rightInputSource);
        //    twoHandDistance = initLeftPosition - initRightPosition;
        //}
    }

    public void BothHandGrip()
    {
        if (gripAction.GetState(leftInputSource) && gripAction.GetState(rightInputSource))
        {
            if (!isLeftMoveNow && !isRightMoveNow)
            {
                isLeftMoveNow = true;
                isRightMoveNow = true;
                initLeftPosition = poseAction.GetLocalPosition(leftInputSource);
                initRightPosition = poseAction.GetLocalPosition(rightInputSource);
                twoHandDistance = initLeftPosition - initRightPosition;
            }
            // Get the initial positions of both controllers

            // Calculate the distance between the two controllers in the Y-axis
            float distanceX = Mathf.Abs(poseAction.GetLocalPosition(leftInputSource).x - poseAction.GetLocalPosition(rightInputSource).x) - Mathf.Abs(twoHandDistance.x);
            Debug.Log(distanceX);
            STCBox.instance.xScale += distanceX * 10.0f;
            STCBox.instance.zScale += distanceX * 10.0f;
            UpdatePath.UpdateEveryPath();
        }

    }
}
