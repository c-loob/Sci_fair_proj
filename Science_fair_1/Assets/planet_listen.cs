using UnityEngine;
using System.Collections;
using System;

namespace VRStandardAssets.Utils
{
    [RequireComponent(typeof(AudioSource))]


    public class planet_listen : MonoBehaviour
    {
        public AudioClip OnOver;
        public AudioClip OnOut;

        AudioSource audio;
        private bool upsized = false;

        //Gaze control
        [SerializeField]
        private VRInteractiveItem m_InteractiveItem;

        [SerializeField]
        private VRInput m_VrInput;

        /*
        [SerializeField]
        public GameObject planet;
        */
        private bool allow;

        void OnEnable()
        {
            //if (allow == true) { 
            m_InteractiveItem.OnOver += ScaleUp;
            m_InteractiveItem.OnOut += ScaleDown;
            //}

        }

        void OnDisable()
        {   
            //if ( allow == true) { 
            m_InteractiveItem.OnOver -= ScaleUp;
            m_InteractiveItem.OnOut -= ScaleDown;
            //}

        }

        void Start()
        {
            allow = true;
            //audio = planet.GetComponent<AudioSource>();
            audio = GetComponent<AudioSource>();
        }

        void AllowListen(bool a)
        {
            allow = a;
        }

        void ScaleUp()
        {
            
            if ((allow ==true) &&(upsized != true)){ 
                //planet.transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);
                transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);
                if (!audio.isPlaying)
                {
                    audio.PlayOneShot(OnOver, 1F);

                }
                upsized = true;
            }
        }

        void ScaleDown()
        {
            if ((allow == true)&&(upsized == true)) { 
                //planet.transform.localScale -= new Vector3(0.25f, 0.25f, 0.25f);
                transform.localScale -= new Vector3(0.25f, 0.25f, 0.25f);
                if (!audio.isPlaying)
                {
                    audio.PlayOneShot(OnOut, 1F);

                }
                upsized = false;
            }

        }

        /*
        void PlayAudio()
        {
            if (!audio.isPlaying)
            {
                audio.PlayOneShot(sound1, 1F);

            }
        }
        */
    }
}