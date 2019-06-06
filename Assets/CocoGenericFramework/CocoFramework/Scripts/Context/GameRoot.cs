using UnityEngine;
using System.Collections;
using strange.extensions.context.impl;
using strange.extensions.context.api;

namespace TabTale
{
    public class GameRoot : MonoBehaviour, IContextView
    {
        public void Init(GameObject parent) 
        {
            context = new GameContext(this);

            GameObject.DontDestroyOnLoad(this);

            // Attach to parent
            this.transform.parent = parent.transform;
        }

        /// <summary>
        /// When a ContextView is Destroyed, automatically removes the associated Context.
        /// </summary>
        protected virtual void OnDestroy()
        {
			if(context != null)
            	((GameContext)context).SendOnDestroySignal();
            
            if (context != null && Context.firstContext != null)
                Context.firstContext.RemoveContext(context);
        }

        #region IView implementation
        
        public bool requiresContext {get;set;}
        
        public bool registeredWithContext {get;set;}
        
        public bool autoRegisterWithContext{ get; set; }
        
        #endregion
        
        #region IContextView implementation
        
        public IContext context{get;set;}
        
        #endregion
    }
}