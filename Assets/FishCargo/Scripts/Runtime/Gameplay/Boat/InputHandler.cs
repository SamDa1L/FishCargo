using UnityEngine;
using UnityEngine.InputSystem;

namespace FishCargo.Runtime.Gameplay.Boat
{
    /// <summary>
    /// 输入统一层（S3 + S4）
    /// S3: 键盘 A/D + 手柄左摇杆 X 轴 → MoveAxisX
    /// S4: 鼠标右键/LT → AimHold，鼠标左键/RT → Fire，鼠标位置/右摇杆X → 瞄准角度
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("配置")]
        public BoatConfig config;

        /// <summary>归一化横向输入，范围 [-1, +1]</summary>
        public float MoveAxisX { get; private set; }

        /// <summary>瞄准按住（鼠标右键 / LT）</summary>
        public bool AimHold { get; private set; }

        /// <summary>发射触发（鼠标左键 / RT）</summary>
        public bool FirePressed { get; private set; }

        /// <summary>鼠标世界位置（用于瞄准角度计算）</summary>
        public Vector3 MouseWorldPosition { get; private set; }

        /// <summary>右摇杆 X 轴（用于手柄瞄准角度控制）</summary>
        public float RightStickX { get; private set; }

        private float _keyboardAxis;
        private float _gamepadAxis;

        // 最近变化的输入源：0=键盘，1=手柄
        private int _lastChangedSource = 0;
        private float _prevKeyboard;
        private float _prevGamepad;

        void Update()
        {
            ReadKeyboard();
            ReadGamepad();
            ResolvePriority();
            ReadAimAndFire();
        }

        void ReadKeyboard()
        {
            float raw = 0f;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed) raw -= 1f;
                if (Keyboard.current.dKey.isPressed) raw += 1f;
            }
            if (raw != _prevKeyboard) _lastChangedSource = 0;
            _prevKeyboard = raw;
            _keyboardAxis = raw;
        }

        void ReadGamepad()
        {
            float raw = 0f;
            if (Gamepad.current != null)
            {
                raw = Gamepad.current.leftStick.x.ReadValue();
                float deadZone = config != null ? config.stickDeadZone : 0.15f;
                if (Mathf.Abs(raw) < deadZone) raw = 0f;
            }
            if (raw != _prevGamepad) _lastChangedSource = 1;
            _prevGamepad = raw;
            _gamepadAxis = raw;
        }

        void ResolvePriority()
        {
            float absK = Mathf.Abs(_keyboardAxis);
            float absG = Mathf.Abs(_gamepadAxis);

            float result;
            if (absK > absG)
                result = _keyboardAxis;
            else if (absG > absK)
                result = _gamepadAxis;
            else
                result = _lastChangedSource == 0 ? _keyboardAxis : _gamepadAxis;

            MoveAxisX = result;
        }

        void ReadAimAndFire()
        {
            // 瞄准按住（鼠标右键 / LT）
            bool aimMouse = Mouse.current != null && Mouse.current.rightButton.isPressed;
            bool aimGamepad = Gamepad.current != null && Gamepad.current.leftTrigger.ReadValue() > 0.5f;
            AimHold = aimMouse || aimGamepad;

            // 发射触发（鼠标左键 / RT）
            bool fireMouse = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
            bool fireGamepad = Gamepad.current != null && Gamepad.current.rightTrigger.ReadValue() > 0.5f;
            FirePressed = fireMouse || fireGamepad;

            // 鼠标世界位置
            if (Mouse.current != null && Camera.main != null)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                MouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
            }

            // 右摇杆 X 轴
            if (Gamepad.current != null)
            {
                float raw = Gamepad.current.rightStick.x.ReadValue();
                float deadZone = config != null ? config.aimStickDeadZone : 0.20f;
                RightStickX = Mathf.Abs(raw) < deadZone ? 0f : raw;
            }
            else
            {
                RightStickX = 0f;
            }
        }
    }
}
