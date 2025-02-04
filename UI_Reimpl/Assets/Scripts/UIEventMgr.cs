using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIReimpl
{
    public delegate void CustomUIEventHandler();

    public abstract class CustomUIEvent
    {
        public CustomUIEventHandler func;  //useless

        public abstract void process_debug(UIDebugWindowA win1);
        public abstract void process_release();

    }


    public class UIEventMgr : MonoBehaviour
    {
        public bool debug_mode = true;
        public UIDebugWindowA debug_win1;

        int max_handle_event_num_frame = 256;
        Queue<CustomUIEvent> event_queue;


        private void Awake()
        {
            event_queue = new();
        }

        public void AddEvent(CustomUIEvent evt)
        {
            event_queue.Enqueue(evt);
        }

        void Update()
        {
            for(int i=0; i< max_handle_event_num_frame; i++)
            {
                if(event_queue.Count == 0) { break; }

                CustomUIEvent evt = event_queue.Dequeue();

                if (debug_mode)
                {
                    evt.process_debug(debug_win1);
                }
                else
                {
                    evt.process_release();
                }
            }
        }
    }

}

