using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class WalkerBehavior : MonoBehaviour
    {
        private float walkSpeedMultiplier;
        private Boolean moveEnable;
        private Boolean walkerEnable;

        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private Vector3 position;
        private Vector3 positionOrg;
        private Boolean goAway;
        private GameObject Walker;

        private Parameters_Walker.MoveDirection xMoveDir;
        private Parameters_Walker.MoveDirection zMoveDir;
        private float distToStartPoint;

        private void Start()
        {
            walkerEnable = GameObject.Find("Parameters").GetComponent<Parameters_Walker>().isWalkerEnabled();
            Walker = this.gameObject;
            if (walkerEnable == true)
            {
                positionOrg = transform.position;
                goAway = true;
                walkSpeedMultiplier = GameObject.Find("Parameters").GetComponent<Parameters_Walker>().getWalkSpeedMultiplier();
                moveEnable = GameObject.Find("Parameters").GetComponent<Parameters_Walker>().isMoveEnabled();
                // get the transform of the main camera
                // get the third person character ( this should never be null due to require component )
                m_Character = GetComponent<ThirdPersonCharacter>();
                Walker.SetActive(true);
                xMoveDir = GameObject.Find("Parameters").GetComponent<Parameters_Walker>().getXMoveDirection();
                zMoveDir = GameObject.Find("Parameters").GetComponent<Parameters_Walker>().getZMoveDirection();
                distToStartPoint = GameObject.Find("Parameters").GetComponent<Parameters_Walker>().getDistToStartPoint();
            }
            else
            {
                Walker.SetActive(false);
            }
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
                if (distance <= distToStartPoint)
                {
                    h = (float)xMoveDir;
                    v = (float)zMoveDir;
                }
                if (distance >= distToStartPoint)
                {
                    //crouch = true;
                    goAway = false;
                }
            }

            if (goAway == false)
            {
                if (distance >= 2)
                {
                    h = -(float)xMoveDir;
                    v = -(float)zMoveDir;
                }
                if (distance <= 2)
                {
                    goAway = true;
                }
            }


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
