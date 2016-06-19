﻿using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ToggleGroup))]
public class ScrollListView : MonoBehaviour
{
    [SerializeField]
    protected ScrollListItemView m_ScrollListItemPrefab = null;
    [SerializeField]
    protected ToggleGroup m_DefaultToggleGroup = null;
    [SerializeField]
    protected bool m_AllowMultiSelect = false;

    protected Dictionary<string, ScrollListItemView> m_CurrentScrollList = new Dictionary<string, ScrollListItemView>();
    protected string m_SelectedItemId = null;
    protected HashSet<string> m_MultiSelectedItemIds = new HashSet<string>();

    public UnityTypedEvent.ScrollListItemViewEvent onSelectedItemChange = new UnityTypedEvent.ScrollListItemViewEvent();
    public UnityTypedEvent.ScrollListItemViewListEvent onMultiSelectedItemsChange = new UnityTypedEvent.ScrollListItemViewListEvent();

    public void UpdateList(Hashtable itemTable)
    {
        Dictionary<string, ScrollListItemView> newScrollList = new Dictionary<string, ScrollListItemView>();
        foreach (string itemId in itemTable.Keys)
        {
            if (m_CurrentScrollList.ContainsKey(itemId))
            {
                newScrollList[itemId] = m_CurrentScrollList[itemId];
                newScrollList[itemId].UpdateItem(itemTable[itemId]);
                m_CurrentScrollList.Remove(itemId);
            }
            else
            {
                ScrollListItemView itemInstance = Instantiate(m_ScrollListItemPrefab);
                itemInstance.transform.SetParent(transform);
                itemInstance.transform.localPosition = Vector3.zero;
                itemInstance.transform.localScale = Vector3.one;

                ToggleGroup toggleGroup = m_AllowMultiSelect ? null : m_DefaultToggleGroup;
                itemInstance.InitItemView(itemId, toggleGroup, this);
                itemInstance.UpdateItem(itemTable[itemId]);
                newScrollList[itemId] = itemInstance;
            }
        }

        foreach (ScrollListItemView itemView in m_CurrentScrollList.Values)
        {
            Destroy(itemView.gameObject);
        }
        m_CurrentScrollList.Clear();

        m_CurrentScrollList = newScrollList;
    }

	public void SelectItemInList(string idInList, bool isSelected)
    {
        bool isChanged = false;

        if (!m_AllowMultiSelect)
        {
            if (isSelected)
            {
                m_SelectedItemId = idInList;
                isChanged = true;
            }
            else
            {
                if ((m_SelectedItemId != null) && m_SelectedItemId.Equals(idInList))
                {
                    m_SelectedItemId = null;
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                onSelectedItemChange.Invoke((m_SelectedItemId != null) && m_CurrentScrollList.ContainsKey(m_SelectedItemId) ? m_CurrentScrollList[m_SelectedItemId] : null);
            }
        }
        else
        {
            if (isSelected)
            {
                isChanged = m_MultiSelectedItemIds.Add(idInList);
            }
            else
            {
                isChanged = m_MultiSelectedItemIds.Remove(idInList);
            }

            if (isChanged)
            {
                List<ScrollListItemView> itemViews = m_MultiSelectedItemIds.Count() > 0 ? new List<ScrollListItemView>() : null;
                foreach (string itemId in m_MultiSelectedItemIds)
                {
                    if (m_CurrentScrollList.ContainsKey(itemId))
                    {
                        itemViews.Add(m_CurrentScrollList[itemId]);
                    }
                }
                onMultiSelectedItemsChange.Invoke(itemViews);
            }            
        }
    }
}