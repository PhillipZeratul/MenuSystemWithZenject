using UnityEngine;
using System;
using System.Collections.Generic;


namespace MenuSystemWithZenject
{
    public class MenuManager
    {
        public Action<Menu> OnMenuEnabled;
        private LinkedList<Menu> menuLinkedList = new LinkedList<Menu>();


        public void OpenMenu(Menu instance)
        {
            if (menuLinkedList.Find(instance) != null)
                return;
            
            if (menuLinkedList.Count == 0)
            {
                instance.GetComponent<Canvas>().sortingOrder = 0;
                menuLinkedList.AddLast(instance);
            }
            else
            {
                // De-activate top menu
                if (instance.DisableMenusUnderneath)
                    for (var menuNode = menuLinkedList.Last; menuNode != null; menuNode = menuNode.Previous)
                    {
                        if (!menuNode.Value.AlwaysKeepOnTop)
                            menuNode.Value.gameObject.SetActive(false);

                        if (menuNode.Value.DisableMenusUnderneath)
                            break;
                    }

                if (instance.AlwaysKeepOnTop)
                {
                    instance.GetComponent<Canvas>().sortingOrder = menuLinkedList.Last.Value.gameObject.GetComponent<Canvas>().sortingOrder + 1;
                    menuLinkedList.AddLast(instance);
                }
                else
                {
                    LinkedListNode<Menu> node = FindLastNotOnTopMenuNode();
                    if (node != null)
                    {
                        instance.GetComponent<Canvas>().sortingOrder = node.Value.gameObject.GetComponent<Canvas>().sortingOrder + 1;
                        menuLinkedList.AddAfter(node, instance);
                    }
                    else
                    {
                        instance.GetComponent<Canvas>().sortingOrder = 0;
                        menuLinkedList.AddFirst(instance);
                    }

                    for (node = menuLinkedList.Find(instance).Next; node != null; node = node.Next)
                        node.Value.GetComponent<Canvas>().sortingOrder = node.Previous.Value.GetComponent<Canvas>().sortingOrder + 1;
                }
            }

            //ShowDebug();
        }

        private LinkedListNode<Menu> FindLastNotOnTopMenuNode()
        {
            LinkedListNode<Menu> node;

            for (node = menuLinkedList.Last; node != null; node = node.Previous)
                if (!node.Value.AlwaysKeepOnTop)
                    return node;
            
            return null;
        }

        public void MenuEnabled(Menu instance)
        {
            if (!instance.AlwaysKeepOnTop && OnMenuEnabled != null)
                OnMenuEnabled(instance);
        }

        public void CloseMenu(Menu instance)
        {
            if (menuLinkedList.Count == 0)
            {
                Debug.LogErrorFormat(instance, "{0} cannot be closed because menu linked list is empty", instance.GetType());
                return;
            }

            LinkedListNode<Menu> node = menuLinkedList.Find(instance);
            if (node == null)
            {
                Debug.LogErrorFormat(instance, "{0} cannot be closed because it is not in the linked list", instance.GetType());
                return;
            }
            else
            {
                node = node.Previous;
                if (instance.DestroyWhenClosed)
                {
                    GameObject.Destroy(instance.gameObject);
                    menuLinkedList.Remove(node.Next);
                }
                else
                    instance.gameObject.SetActive(false);

                // Re-activate top menu
                // If a re-activated menu is an overlay we need to activate the menu under it
                for (var menuNode = FindLastNotOnTopMenuNode(); menuNode != null; menuNode = menuNode.Previous)
                {
                    if (!menuNode.Value.DestroyWhenClosed)
                        continue;
                    
                    menuNode.Value.gameObject.SetActive(true);

                    if (menuNode.Value.DisableMenusUnderneath)
                        break;
                }
            }
            Debug.Log("Menu Closed: " + instance.name);
        }

        public void CloseMenuUntil(Menu instance, bool shouldCloseAlwaysOnTopMenu)
        {
            LinkedListNode<Menu> instanceNode = menuLinkedList.Find(instance);
            if (instanceNode == null)
            {
                Debug.LogErrorFormat(instance, "{0} not found in the list", instance.GetType());
                return;
            }

            LinkedListNode<Menu> node;
            LinkedListNode<Menu> nodeNext;
            for (node = menuLinkedList.Last; node != instanceNode;)
            {
                if (!node.Value.AlwaysKeepOnTop || shouldCloseAlwaysOnTopMenu)
                {
                    ///
                    Debug.LogFormat("Menu to Close : {0}", node.Value.name);
                    ///
                    nodeNext = node;
                    node = node.Previous;



                    // @TODO: Have bug when Openning and Closing UpdateMenu.
                    //CloseMenu(node.Next.Value);
                    if (nodeNext.Value.DestroyWhenClosed)
                    {
                        GameObject.Destroy(nodeNext.Value.gameObject);
                        menuLinkedList.Remove(nodeNext);
                    }
                    else
                        nodeNext.Value.gameObject.SetActive(false);
                }
                else
                    node = node.Previous;
            }

            for (var menuNode = FindLastNotOnTopMenuNode(); menuNode != null; menuNode = menuNode.Previous)
            {
                menuNode.Value.gameObject.SetActive(true);

                if (menuNode.Value.DisableMenusUnderneath)
                    break;
            }
        }

        private void ShowDebug()
        {
            for (var node = menuLinkedList.First; node != null; node = node.Next)
            {
                Debug.Log(node.Value.gameObject.name + "'s sortingOrder = " + node.Value.GetComponent<Canvas>().sortingOrder);
            }
        }
    }
}
