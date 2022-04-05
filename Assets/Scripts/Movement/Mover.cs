
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] Transform target;

        NavMeshAgent navMeshAgent;
        Health health;

        private void Start()
        {
           navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();    
        }

        void Update()
        {

            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination)
        {
            
            GetComponent<ActionScheduler>().StartAction(this);
            
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

      

        private void UpdateAnimator()
        {
            // Get velocity from NavMeshAgent, transform into local variable
            Vector3 vecloity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(vecloity);

            // Create local variable 'speed' to pass into SetFloat for Animator componenet
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

    }
}
