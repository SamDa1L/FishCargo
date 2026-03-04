using UnityEngine;
using UnityEngine.InputSystem;

namespace FishCargo.Runtime.Gameplay.Boat
{
    /// <summary>
    /// 输入统一层（S3）
    /// 统一读取键盘 A/D 与手柄左摇杆 X 轴，输出归一化 MoveAxisX
    /// 双设备优先级：绝对值更大的输入源优先；绝对值相等时取最近变化的输入源
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("配置")]
        public BoatConfig config;

        /// <summary>归一化横向输入，范围 [-1, +1]</summary>
        public float MoveAxisX { get; private set; }

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
    }
}
