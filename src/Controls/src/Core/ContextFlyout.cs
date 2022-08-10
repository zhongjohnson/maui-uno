using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.Controls
{
    public partial class ContextFlyout : Element, IContextFlyout // Same pattern as MenuBarItem
	{
        ReadOnlyCastingList<Element, IMenuElement> _logicalChildren;
        readonly ObservableCollection<IMenuElement> _menus = new ObservableCollection<IMenuElement>();

        internal override IReadOnlyList<Element> LogicalChildrenInternal =>
            _logicalChildren ??= new ReadOnlyCastingList<Element, IMenuElement>(_menus);

        public IMenuElement this[int index]
        {
            get { return _menus[index]; }
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public int Count => _menus.Count;

        public bool IsReadOnly => false;

        public void Add(IMenuElement item)
        {
            var index = _menus.Count;
            _menus.Add(item);
            NotifyHandler(nameof(IContextFlyoutHandler.Add), index, item);

            // Take care of the Element internal bookkeeping
            if (item is Element element)
            {
                OnChildAdded(element);
            }
        }

        public void Clear()
        {
            for (int i = _menus.Count - 1; i >= 0; i--)
                RemoveAt(i);
        }

        public bool Contains(IMenuElement item)
        {
            return _menus.Contains(item);
        }

        public void CopyTo(IMenuElement[] array, int arrayIndex)
        {
            _menus.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IMenuElement> GetEnumerator()
        {
            return _menus.GetEnumerator();
        }

        public int IndexOf(IMenuElement item)
        {
            return _menus.IndexOf(item);
        }

        public void Insert(int index, IMenuElement item)
        {
            _menus.Insert(index, item);
            NotifyHandler(nameof(IContextFlyoutHandler.Insert), index, item);

            // Take care of the Element internal bookkeeping
            if (item is Element element)
            {
                OnChildAdded(element);
            }
        }

        public bool Remove(IMenuElement item)
        {
            var index = _menus.IndexOf(item);
            var result = _menus.Remove(item);
            NotifyHandler(nameof(IContextFlyoutHandler.Remove), index, item);

            // Take care of the Element internal bookkeeping
            if (item is Element element)
            {
                OnChildRemoved(element, index);
            }

            return result;
        }

        public void RemoveAt(int index)
        {
            var item = _menus[index];
            _menus.RemoveAt(index);
            NotifyHandler(nameof(IContextFlyoutHandler.Remove), index, item);

            // Take care of the Element internal bookkeeping
            if (item is Element element)
            {
                OnChildRemoved(element, index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _menus.GetEnumerator();
        }

        void NotifyHandler(string action, int index, IMenuElement view)
        {
            var args = new Maui.Handlers.ContextFlyoutItemHandlerUpdate(index, view);
            Handler?.Invoke(action, args);
        }
    }
}
