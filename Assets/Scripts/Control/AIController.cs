using System.Collections;
using System.Collections.Generic;
using RPG.Movement;
using RPG.Combat;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float weaponRange = 1.5f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] float WaypointDwellTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
       

        Mover mover;
        Fighter fighter;
        GameObject player;
        Health health;

        Vector3 guardposition;
        float timeSinceLastSawPlayer = Mathf.Infinity;  
        float timeSinceArrivedAtWapoint = Mathf.Infinity;    
        int currentWaypointIndex = 0;   
        private void Start()
        {
            fighter = GetComponent<Fighter>();
             mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();

            guardposition = transform.position;

        }
        private void Update()
        {
            print(timeSinceArrivedAtWapoint);
            if (health.IsDead()) return;

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {

                SuspicionBehaviour();

            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWapoint += Time.deltaTime;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardposition;
            
            if(patrolPath != null)
            {
                if(AtWapoint())
                {
                    timeSinceArrivedAtWapoint = 0;
                    CycleWapoint();
                }
                nextPosition = GetCurrentWapoint();
                
            }
            
            if (timeSinceArrivedAtWapoint > WaypointDwellTime)
            {
                mover.StartMoveAction(nextPosition);
            }
            
        }

        private Vector3 GetCurrentWapoint()
        {
            
           return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWapoint()
        {
          currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWapoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWapoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void AttackBehaviour()
        {
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer < chaseDistance;
        }

        // Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

