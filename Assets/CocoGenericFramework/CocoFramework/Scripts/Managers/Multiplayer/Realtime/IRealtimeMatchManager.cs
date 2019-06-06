using UnityEngine;
using System.Collections;
using strange.extensions.context.impl;
using strange.extensions.context.api;

namespace TabTale
{
    public interface IRealtimeMatchManager
    {
        void FindMultiplayerMatch();

        void FindQuickMatch();

    }
}