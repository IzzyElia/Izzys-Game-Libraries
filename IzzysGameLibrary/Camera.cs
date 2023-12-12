using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IzzysGameLibrary
{
    public abstract class Camera
    {
        public Matrix view { get; private set; }
        public Matrix projection { get; protected set; }
        protected Vector3 _position;
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                RecalculateViewMatrix();
            }
        }
        protected Vector3 _target;
        public Vector3 Target
        {
            get => _target;
            set
            {
                _target = value;
                RecalculateViewMatrix();
            }
        }
        protected Camera ()
        {

        }
        void RecalculateViewMatrix ()
        {
            view = Matrix.CreateLookAt(_position, _target, Vector3.Up);
        }
        public void Move (Vector3 movement)
        {
            Position = new Vector3(_position.X + movement.X, _position.Y + movement.Y, _position.Z + movement.Z);
        }
    }
    public class PerspectiveCamera : Camera
    {
        float _fov, _aspectRatio, _nearPlane, _farPlane;
        float FieldOfView
        {
            get => _fov;
            set
            {
                _fov = value;
                RecalculateProjectionMatrix();
            }
        }
        float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                _aspectRatio = value;
                RecalculateProjectionMatrix();
            }
        }
        float NearPlane
        {
            get => _nearPlane;
            set
            {
                _nearPlane = value;
                RecalculateProjectionMatrix();
            }
        }
        float FarPlane
        {
            get => _farPlane;
            set
            {
                _farPlane = value;
                RecalculateProjectionMatrix();
            }
        }
        public PerspectiveCamera (Vector3 position, float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
        {
            base.Position = position;
            _fov = fieldOfView;
            _aspectRatio = aspectRatio;
            _nearPlane = nearPlane;
            _farPlane = farPlane;
            RecalculateProjectionMatrix();
        }
        void RecalculateProjectionMatrix ()
        {
            base.projection = Matrix.CreatePerspectiveFieldOfView(_fov, _aspectRatio, _nearPlane, _farPlane);
        }
    }
    public class OrthographicCamera : Camera
    {
        float _width, _height, _nearPlane, _farPlane;
        float NearPlane
        {
            get => _nearPlane;
            set
            {
                _nearPlane = value;
                RecalculateProjectionMatrix();
            }
        }
        float FarPlane
        {
            get => _farPlane;
            set
            {
                _farPlane = value;
                RecalculateProjectionMatrix();
            }
        }
        public OrthographicCamera(Vector3 position, float width, float height, float nearPlane, float farPlane)
        {
            base.Position = position;
            _width = width;
            _height = height;
            _nearPlane = nearPlane;
            _farPlane = farPlane;
            RecalculateProjectionMatrix();
        }
        void RecalculateProjectionMatrix()
        {
            base.projection = Matrix.CreateOrthographic(_width, _height, _nearPlane, _farPlane);
        }
        public void SetDimentions (float width, float height)
        {
            _width = width;
            _height = height;
            RecalculateProjectionMatrix();
        }
    }
}
