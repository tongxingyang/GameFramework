using UnityEngine;

namespace GameFramework.UnityComponent
{
    public class RigidbodyComponent : MonoBehaviour 
    {
        public enum enRigidbodyMode
        {
            TwoD,
            ThreeD,
        }
        private enRigidbodyMode mode;
        private Rigidbody2D rigidbody2D;
        private Rigidbody rigidbody;
        private Collider2D collider2D;
        private Collider collider;

        public Rigidbody2D InternalRigidBody2D => rigidbody2D;
        public Rigidbody InternalRigidBody => rigidbody;
        public Vector3 Position => mode == enRigidbodyMode.TwoD? (Vector3) rigidbody2D.position: rigidbody. position;
        public Bounds ColliderBounds =>  mode == enRigidbodyMode.TwoD ?  collider2D.bounds: collider.bounds;
        public bool isKinematic  =>  mode == enRigidbodyMode.TwoD ?  rigidbody2D.isKinematic:rigidbody.isKinematic;
        public bool Is3D => mode == enRigidbodyMode.ThreeD ? true : false;
        public bool Is2D => mode == enRigidbodyMode.TwoD ? true : false;

        public Vector3 Velocity   
        {
            get 
            { 
                if (mode == enRigidbodyMode.TwoD) 
                {
                    return(rigidbody2D.velocity);
                }
                if (mode == enRigidbodyMode.ThreeD) 
                {
                    return(rigidbody.velocity);
                }
                return new Vector3(0,0,0);
            }
            set 
            {
                if (mode == enRigidbodyMode.TwoD) {
                    rigidbody2D.velocity = value;
                }
                if (mode == enRigidbodyMode.ThreeD) {
                    rigidbody.velocity = value;
                }
            }
        }
        
        protected virtual void Awake () 
        {
            rigidbody2D=GetComponent<Rigidbody2D>();
            rigidbody=GetComponent<Rigidbody>();

            if (rigidbody2D != null)
            {
                mode = enRigidbodyMode.TwoD;
                collider2D = GetComponent<Collider2D> ();
            }
            if (rigidbody != null) 
            {
                mode = enRigidbodyMode.ThreeD;
                collider = GetComponent<Collider> ();
            }
        }
        
        public virtual void AddForce(Vector3 force)
        {
            if (mode == enRigidbodyMode.TwoD) 
            {
                rigidbody2D.AddForce(force,ForceMode2D.Impulse);
            }
            if (mode ==enRigidbodyMode.ThreeD)
            {
                rigidbody.AddForce(force);
            }
        }
		
        public virtual void AddRelativeForce(Vector3 force)
        {
            if (mode == enRigidbodyMode.TwoD) 
            {
                rigidbody2D.AddRelativeForce(force,ForceMode2D.Impulse);
            }
            if (mode == enRigidbodyMode.ThreeD)
            {
                rigidbody.AddRelativeForce(force);
            }
        }
        
        public virtual void MovePosition(Vector3 newPosition)
        {
            if (mode == enRigidbodyMode.TwoD) 
            {
                rigidbody2D.MovePosition(newPosition);
            }
            if (mode == enRigidbodyMode.ThreeD) 
            {
                rigidbody.MovePosition(newPosition);
            }
        }
        
        public virtual void ResetAngularVelocity()
        {
            if (mode == enRigidbodyMode.TwoD) 
            {
                rigidbody2D.angularVelocity = 0;
            }
            if (mode ==enRigidbodyMode.ThreeD) 
            {
                rigidbody.angularVelocity = Vector3.zero;
            }	
        }
        
        public virtual void ResetRotation()
        {
            if (mode == enRigidbodyMode.TwoD) 
            {
                rigidbody2D.rotation = 0;
            }
            if (mode == enRigidbodyMode.ThreeD) 
            {
                rigidbody.rotation = Quaternion.identity;
            }	
        }
        
        public virtual void IsKinematic(bool status)
        {
            if (mode == enRigidbodyMode.TwoD) 
            {
                rigidbody2D.isKinematic=status;
            }
            if (mode == enRigidbodyMode.ThreeD) 
            {			
                rigidbody.isKinematic=status;
            }
        }
    }
}