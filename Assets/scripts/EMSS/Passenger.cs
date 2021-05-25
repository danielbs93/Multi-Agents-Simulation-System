using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace testElevator
{
    class Passenger
    {
        private string name;
        private Floor destination; // end position
        private Floor departured; // start position
        private GameObject passengerGameObject;
        private Animator passengerAnimator;
        private float[] position = { -1, -1, -1 };
        private bool boardAction;
        private bool leaveAction;
        private Vector3 insideElevatorIncitement;

        public Passenger(string name, Floor departured)
        {
            this.name = name;
            this.departured = departured;
            this.boardAction = false;
            this.leaveAction = false;
        }
       

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Floor Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public Floor Departured
        {
            get { return departured; }
            set { departured = value; }
        }

        public GameObject PassengerGameObject
        {
            get => passengerGameObject;
            set
            {
                passengerGameObject = value;
                passengerAnimator = passengerGameObject.GetComponent<Animator>();
            }
        }
        public float[] Position { get => position; set => position = value; }
        public bool BoardAction { get => boardAction; set => boardAction = value; }
        public bool LeaveAction { get => leaveAction; set => leaveAction = value; }
        public Vector3 InsideElevatorIncitement { get => insideElevatorIncitement; set => insideElevatorIncitement = value; }

        public Transform GetGameObjectTransform()
        {
            return passengerGameObject.transform;
        }

          /*
            Move passenger game object in unity towards the desired position on X axis
         */
        public void MoveOnXaxis(float currentDirectionX, float simulationSpeed, float elevatorSpeed)
        {
            if (this.leaveAction)
            {
                rotatePassengerTowardsX(currentDirectionX);
                this.leaveAction = false;
            }
            Vector3 localPosition = GetGameObjectTransform().position;
            Vector3 toPosition = new Vector3(currentDirectionX, localPosition.y, localPosition.z);
            passengerGameObject.GetComponent<Transform>().position = Vector3.MoveTowards(localPosition, toPosition, (simulationSpeed / elevatorSpeed));
        }

        public void rotatePassengerTowardsX(float currentDirectionX)
        {
            Vector3 localPosition = GetGameObjectTransform().position;
            if (localPosition.x > currentDirectionX)
            {
                if (boardAction)
                {
                    GetGameObjectTransform().Rotate(0, GetGameObjectTransform().rotation.y + 90, 0);
                }
                else
                {
                    GetGameObjectTransform().Rotate(0, GetGameObjectTransform().rotation.y - 90, 0);
                }
            }
            else if (localPosition.x < currentDirectionX)
            {
                if (boardAction)
                {
                    GetGameObjectTransform().Rotate(0, GetGameObjectTransform().rotation.y - 90, 0);
                }
                else
                {
                    GetGameObjectTransform().Rotate(0, GetGameObjectTransform().rotation.y + 90, 0);
                }
            }
        }

        /*
         Move passenger game object in unity towards the desired position on Z axis
        */
        public void MoveOnZaxis(float currentDirectionZ, float simulationSpeed, float elevatorSpeed)
        {
            if (this.boardAction)
            {
                rotatePassengerTowardsZ();
                RotatePassengerInfo(true);
                this.boardAction = false;
            }
            Vector3 localPosition = GetGameObjectTransform().position;
            Vector3 toPosition = new Vector3(localPosition.x, localPosition.y, currentDirectionZ);
            GetGameObjectTransform().position = Vector3.MoveTowards(localPosition, toPosition, (simulationSpeed / elevatorSpeed));
        }

        public void rotatePassengerTowardsZ()
        {
            if (GetGameObjectTransform().rotation.eulerAngles.y > 250)
            {
                if (boardAction)
                {
                    GetGameObjectTransform().Rotate(0, GetGameObjectTransform().rotation.y + 90, 0);
                }
                else
                {
                    GetGameObjectTransform().Rotate(0, GetGameObjectTransform().rotation.y - 90, 0);
                }
            }
            else if (GetGameObjectTransform().rotation.eulerAngles.y < 100)
            {
                if (boardAction)
                {
                    GetGameObjectTransform().Rotate(0, GetGameObjectTransform().rotation.y - 90, 0);
                }
                else
                {
                    GetGameObjectTransform().Rotate(0, GetGameObjectTransform().rotation.y + 90, 0);
                }
            }
        }
        /*
            The animator responsible for ainmate the walking or standing animation.
            True - the passenger will animate the walking state
            False - the passenger will animate the standing state
         */
        public void WalkingOrStandingMovement(bool walk)
        {
            if (walk)
            {
                passengerAnimator.SetBool("walking", true);
            }
            else
            {
                passengerAnimator.SetBool("walking", false);
            }
        }

        /*
            Rotate the name + desitnation gameobjects attached to the passenger game object
         */
        public void RotatePassengerInfo(bool board)
        {
            Transform nameObject = GetGameObjectTransform().GetChild(11).GetComponent<TMPro.TMP_Text>().transform;
            Transform destinationObject = GetGameObjectTransform().GetChild(12).GetComponent<TMPro.TMP_Text>().transform;
            if (board)
            {
                nameObject.Rotate(0, nameObject.rotation.y + 180, 0);
                destinationObject.Rotate(0, nameObject.rotation.y + 180, 0);
            }
            else
            {
                nameObject.Rotate(0, nameObject.rotation.y - 180, 0);
                destinationObject.Rotate(0, nameObject.rotation.y - 180, 0);
            }

            //nameObject.position = GetGameObjectTransform().position;
            //destinationObject.position = GetGameObjectTransform().position;
        }

    }
}

