using OpenTK.Mathematics;
using BulletSharp;
using System.Runtime.InteropServices;

namespace Tiny3DEngine
{
    public class CubePhysics : IDisposable
    {
        private Vector3 _position = new Vector3(0, 3, 0);
        private Quaternion _rotation = Quaternion.Identity;
        private Vector3 _linearVelocity = Vector3.Zero;
        private Vector3 _angularVelocity = Vector3.Zero;
        private DiscreteDynamicsWorld _dynamicsWorld;
        private RigidBody _cubeRigidBody;
        private RigidBody _groundRigidBody;
        private CollisionShape _cubeCollisionShape;
        private CollisionShape _groundCollisionShape;
        private DefaultCollisionConfiguration _collisionConfig;
        private CollisionDispatcher _dispatcher;
        private DbvtBroadphase _broadphase;
        private SequentialImpulseConstraintSolver _solver;
        private float cubeHalfSize = 0.5f;
        public bool IsGrounded { get; private set; }
        private bool _disposed = false;

        public Vector3 Position { get => _position; set => _position = value; }
        public Quaternion Rotation { get => _rotation; set => _rotation = value; }
        public Vector3 LinearVelocity { get => _linearVelocity; set => _linearVelocity = value; }
        public Vector3 AngularVelocity { get => _angularVelocity; set => _angularVelocity = value; }

        public void Start()
        {
            InitializeBulletPhysics();
        }

        private void InitializeBulletPhysics()
        {
            _collisionConfig = new DefaultCollisionConfiguration();
            _dispatcher = new CollisionDispatcher(_collisionConfig);
            _broadphase = new DbvtBroadphase();
            _solver = new SequentialImpulseConstraintSolver();

            _dynamicsWorld = new DiscreteDynamicsWorld(_dispatcher, _broadphase, _solver, _collisionConfig);
            _dynamicsWorld.Gravity = new BulletSharp.Math.Vector3(0, -9.81f, 0);

            _cubeCollisionShape = new BoxShape(cubeHalfSize, cubeHalfSize, cubeHalfSize);

            var startTransform = BulletSharp.Math.Matrix.Translation(0, 3, 0);
            var cubeMotionState = new DefaultMotionState(startTransform);
            var cubeInertia = _cubeCollisionShape.CalculateLocalInertia(1.0f);
            var cubeConstructionInfo = new RigidBodyConstructionInfo(1.0f, cubeMotionState, _cubeCollisionShape, cubeInertia);

            _cubeRigidBody = new RigidBody(cubeConstructionInfo);
            _cubeRigidBody.Restitution = 0.6f;
            _cubeRigidBody.Friction = 0.5f;
            _cubeRigidBody.RollingFriction = 0.1f;

            _dynamicsWorld.AddRigidBody(_cubeRigidBody);
            CreateGround();
        }

        private void CreateGround()
        {
            _groundCollisionShape = new StaticPlaneShape(new BulletSharp.Math.Vector3(0, 1, 0), -2.0f);
            var groundMotionState = new DefaultMotionState();
            var groundConstructionInfo = new RigidBodyConstructionInfo(0, groundMotionState, _groundCollisionShape);
            _groundRigidBody = new RigidBody(groundConstructionInfo);
            _groundRigidBody.Restitution = 0.4f;
            _groundRigidBody.Friction = 0.6f;
            _dynamicsWorld.AddRigidBody(_groundRigidBody);
        }

        public void Update(float deltaTime, float groundLevel)
        {
            _dynamicsWorld.StepSimulation(deltaTime);
            _cubeRigidBody.MotionState.GetWorldTransform(out var transform);

            _position = new Vector3(transform.M41, transform.M42, transform.M43);
            _rotation = MatrixToQuaternion(transform);

            var linVel = _cubeRigidBody.LinearVelocity;
            var angVel = _cubeRigidBody.AngularVelocity;
            _linearVelocity = new Vector3(linVel.X, linVel.Y, linVel.Z);
            _angularVelocity = new Vector3(angVel.X, angVel.Y, angVel.Z);

            CheckGrounded();
        }

        private Quaternion MatrixToQuaternion(BulletSharp.Math.Matrix matrix)
        {
            float trace = matrix.M11 + matrix.M22 + matrix.M33;

            if (trace > 0)
            {
                float s = 0.5f / MathF.Sqrt(trace + 1.0f);
                return new Quaternion(
                    (matrix.M32 - matrix.M23) * s,
                    (matrix.M13 - matrix.M31) * s,
                    (matrix.M21 - matrix.M12) * s,
                    0.25f / s
                );
            }
            else
            {
                if (matrix.M11 > matrix.M22 && matrix.M11 > matrix.M33)
                {
                    float s = 2.0f * MathF.Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                    return new Quaternion(
                        0.25f * s,
                        (matrix.M12 + matrix.M21) / s,
                        (matrix.M13 + matrix.M31) / s,
                        (matrix.M32 - matrix.M23) / s
                    );
                }
                else if (matrix.M22 > matrix.M33)
                {
                    float s = 2.0f * MathF.Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                    return new Quaternion(
                        (matrix.M12 + matrix.M21) / s,
                        0.25f * s,
                        (matrix.M23 + matrix.M32) / s,
                        (matrix.M13 - matrix.M31) / s
                    );
                }
                else
                {
                    float s = 2.0f * MathF.Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                    return new Quaternion(
                        (matrix.M13 + matrix.M31) / s,
                        (matrix.M23 + matrix.M32) / s,
                        0.25f * s,
                        (matrix.M21 - matrix.M12) / s
                    );
                }
            }
        }

        private void CheckGrounded()
        {
            IsGrounded = false;
            if (_position.Y <= -1.9f && Math.Abs(_linearVelocity.Y) < 0.5f)
            {
                IsGrounded = true;
            }
        }

        public void AddImpulse(Vector3 impulse)
        {
            var bulletImpulse = new BulletSharp.Math.Vector3(impulse.X, impulse.Y, impulse.Z);
            _cubeRigidBody.ApplyCentralImpulse(bulletImpulse);
            _cubeRigidBody.Activate();
        }

        public void AddAngularImpulse(Vector3 impulse)
        {
            var bulletImpulse = new BulletSharp.Math.Vector3(impulse.X, impulse.Y, impulse.Z);
            _cubeRigidBody.ApplyTorqueImpulse(bulletImpulse);
            _cubeRigidBody.Activate();
        }

        public void AddRandomImpulse()
        {
            Random rand = new Random();
            Vector3 linearImpulse = new Vector3(
                (float)(rand.NextDouble() - 0.5) * 3f,
                5f + (float)rand.NextDouble() * 3f,
                (float)(rand.NextDouble() - 0.5) * 3f
            );
            Vector3 angularImpulse = new Vector3(
                (float)(rand.NextDouble() - 0.5) * 1f,
                (float)(rand.NextDouble() - 0.5) * 1f,
                (float)(rand.NextDouble() - 0.5) * 1f
            );
            AddImpulse(linearImpulse);
            AddAngularImpulse(angularImpulse);
        }

        public void SetPosition(Vector3 position)
        {
            var transform = _cubeRigidBody.WorldTransform;
            transform.M41 = position.X;
            transform.M42 = position.Y;
            transform.M43 = position.Z;
            _cubeRigidBody.WorldTransform = transform;
            _cubeRigidBody.LinearVelocity = BulletSharp.Math.Vector3.Zero;
            _cubeRigidBody.AngularVelocity = BulletSharp.Math.Vector3.Zero;
            _cubeRigidBody.ClearForces();
        }

        public void ResetPhysics()
        {
            SetPosition(new Vector3(0, 3, 0));
        }

        public void WakeUp()
        {
            _cubeRigidBody.Activate();
        }

        public Matrix4 GetModelMatrix()
        {
            _cubeRigidBody.MotionState.GetWorldTransform(out var transform);
            return new Matrix4(
                transform.M11, transform.M12, transform.M13, transform.M14,
                transform.M21, transform.M22, transform.M23, transform.M24,
                transform.M31, transform.M32, transform.M33, transform.M34,
                transform.M41, transform.M42, transform.M43, transform.M44
            );
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dynamicsWorld?.RemoveRigidBody(_cubeRigidBody);
                    _dynamicsWorld?.RemoveRigidBody(_groundRigidBody);
                    _dynamicsWorld?.Dispose();
                    _cubeRigidBody?.Dispose();
                    _groundRigidBody?.Dispose();
                    _cubeCollisionShape?.Dispose();
                    _groundCollisionShape?.Dispose();
                    _dispatcher?.Dispose();
                    _broadphase?.Dispose();
                    _solver?.Dispose();
                    _collisionConfig?.Dispose();
                }
                _disposed = true;
            }
        }

        ~CubePhysics()
        {
            Dispose(false);
        }
    }
}