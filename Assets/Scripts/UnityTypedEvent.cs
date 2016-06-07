﻿using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class UnityTypedEvent
{
    [Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [Serializable]
    public class StringEvent : UnityEvent<string> { }
    [Serializable]
    public class HashtableEvent : UnityEvent<Hashtable> { }

    [Serializable]
    public class ScrollListItemViewEvent : UnityEvent<ScrollListItemView> { }
    [Serializable]
    public class ScrollListItemViewListEvent : UnityEvent<List<ScrollListItemView>> { }

    [Serializable]
    public class PhotonRoomInfoArrayEvent : UnityEvent<RoomInfo[]> { }
}
