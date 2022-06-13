using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS_Movement_Control
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        Vector2 _mouseAbsolute;
        Vector2 _smoothMouse;

        public GameObject characterBody;

        [Header("相机操作")]
        [SerializeField]
        private Vector2 clampInDegrees = new Vector2(360, 180);
        [SerializeField]
        private Vector2 sensitivity = new Vector2(2, 2);
        [SerializeField]
        private Vector2 smoothing = new Vector2(3, 3);
        [SerializeField]
        Transform overlayCam;

        float adjustToFOV;
        float adjustSpeed;
        float baseFOV = 60f;
        Camera cam;


        private Vector2 point_Direction, point_CharacterDirection;

        private Quaternion targetOrientation, targetCharacterOrientation;


        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            cam = GetComponent<Camera>();
            // 开头获取相机初始化时的欧拉角
            targetOrientation = Quaternion.Euler(transform.localRotation.eulerAngles);

            // 开头获取相机初始化时的欧拉角
            if (characterBody)
                targetCharacterOrientation = Quaternion.Euler(characterBody.transform.localRotation.eulerAngles);
            else
                targetCharacterOrientation = default;

            baseFOV = cam.fieldOfView;
            adjustToFOV = baseFOV;
            
        }

        void Update()
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, adjustToFOV, Time.deltaTime * adjustSpeed);

            // Get raw mouse input for a cleaner reading on more sensitive mice.
            var mouseDelta = PlayController.GetInstance().input_MouseView;
            // Scale input against the sensitivity setting and multiply that against the smoothing value.
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

            // Interpolate mouse movement over time to apply smoothing delta.
            _smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
            _smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

            // Find the absolute mouse movement value from point zero.
            _mouseAbsolute += _smoothMouse;
            // Clamp and apply the local x value first, so as not to be affected by world transforms.
            if (clampInDegrees.x < 360)
                _mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

            // Then clamp and apply the global y value.
            if (clampInDegrees.y < 360)
                _mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

            transform.localRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

            // If there's a character body that acts as a parent to the camera
            if (characterBody)
            {
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, Vector3.up);
                characterBody.transform.localRotation = yRotation * targetCharacterOrientation;
            }
            else
            {
                var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
                transform.localRotation *= yRotation;
            }
        }

        public void SetFOV(bool setTo, float fov, float speed)
        {
            adjustSpeed = Mathf.Abs(fov - baseFOV) / (speed / 2);
            adjustToFOV = (setTo) ? fov : baseFOV;

        }


        public void AddRecoil(Vector3 recoil, float time)
        {
            float recoilElapsed = 0;
            StartCoroutine(recoilIncrease());
            IEnumerator recoilIncrease()
            {
                while (recoilElapsed < time)
                {
                    recoilElapsed += Time.deltaTime;
                    _mouseAbsolute += (Vector2)(recoil * Time.deltaTime / time);
                    yield return null;
                }
            }
        }
    }

}


