﻿using OpenTK.Mathematics;
using System;

namespace CommonClasses
{
    // Это класс камеры.
    // Важно отметить, что есть несколько способов, которыми вы могли бы настроить эту камеру.
    // Например, вы могли бы также управлять вводом проигрывателя внутри класса camera,
    // и многие свойства можно было бы превратить в функции.

    // Если что-то в дальнейшем будет непонятно - смотри веб-версию. https://opentk.net/learn/chapter1/9-camera.html
    public class Camera
    {
        // Эти векторы представляют собой направления, направленные наружу от камеры, чтобы определить, как она расположена.
        private Vector3 _front = -Vector3.UnitZ;

        private Vector3 _up = Vector3.UnitY;

        private Vector3 _right = Vector3.UnitX;

        // Поворот вокруг оси X (в радианах)
        private float _pitch;

        // Поворот вокруг оси Y (в радианах)
        private float _yaw = -MathHelper.PiOver2; // Без этого вы начнёте повёрнутыми вправо на 90 градусов. Можете закомментить инициализацию и проверить

        // Поле видимости камеры (в радианах).
        private float _fov = MathHelper.PiOver2;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        // Положение камеры
        public Vector3 Position { get; set; }

        // Это просто соотношение сторон видового экрана, используемое для матрицы проекции.
        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;

        public Vector3 Up => _up;

        public Vector3 Right => _right;

        // Мы преобразуем градусы в радианы, как только свойство настроено, для повышения производительности.
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // Мы фиксируем значение pitch между -89 и 89, чтобы предотвратить переворачивание камеры вверх ногами, и кучу
                // странных ошибок, когда вы используете углы Эйлера для поворота.
                // Если вы хотите узнать больше об этом, вы можете попробовать изучить тему под названием gimbal lock.
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        // Мы преобразуем градусы в радианы, как только свойство настроено, для повышения производительности.
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        // Поле зрения (FOV) - это вертикальный угол обзора камеры.
        // Это более подробно обсуждалось в предыдущем уроке, но в этом уроке вы также узнали, как мы можем использовать это для имитации функции
        // масштабирования.
        // Мы преобразуем градусы в радианы, как только свойство настроено, для повышения производительности.
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        // Получите матрицу проекции, используя тот же метод, который мы использовали до этого момента.
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }

        // Эта функция будет обновлять вершины направления, используя некоторые математические методы, изученные в веб-руководствах.
        private void UpdateVectors()
        {
            // Во-первых, передняя матрица вычисляется с использованием некоторой базовой тригонометрии.
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            // Нам нужно убедиться, что все векторы нормализованы, так как в противном случае мы получили бы некоторые странные результаты.
            _front = Vector3.Normalize(_front);

            // Вычислите как правый, так и указывающий вверх вектор, используя перекрестное произведение.
            // Обратите внимание, что мы вычисляем справа от глобального направления "вверх"; такое поведение делает
            // не то, что вам нужно для всех камер, так что имейте это в виду, если вам не нужна камера с частотой кадров в секунду.
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}