using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class WalkerBehavior : MonoBehaviour
    {
        public float walkSpeedMultiplier;
        public Boolean moveEnable;

        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private Vector3 position;
        private Vector3 positionOrg;
        private Boolean goAway;



        private void Start()
        {
            positionOrg = transform.position;
            goAway = true;
            // get the transform of the main camera
            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {

            position = transform.position;

            float distance = (position - positionOrg).magnitude;


            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            if (goAway == true)
            {
                if (distance <= 46)
                {
                    v = 0;
                    h = 1;
                }
                if (distance >= 46)
                {
                    //crouch = true;
                    goAway = false;
                }
            }

            if (goAway == false)
            {
                if (distance >= 2)
                {
                    v = 0;
                    h = -1;
                }
                if (distance <= 2)
                {
                    goAway = true;
                }
            }



            // calculate move direction to pass to character
            //if (m_Cam != null)
            //{
            // calculate camera relative direction to move:
            //   m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            //   m_Move = v*m_CamForward + h*m_Cam.right;
            // }
            // else
            // {
            // we use world-relative directions in the case of no main camera
            //      m_Move = v*Vector3.forward + h*Vector3.right;
            // }

            m_Move = (v * Vector3.forward + h * Vector3.right) * walkSpeedMultiplier;
            if(moveEnable == false)
            {
                m_Move = m_Move*0;
            }
#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
    }
}
