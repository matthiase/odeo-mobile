using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

namespace Izume.Mobile.Odeo.Common.Rss
{
    public class ChannelItemCollection : CollectionBase, ITypedList
    {
        public ChannelItemCollection()
        {

        }

        public ChannelItemCollection(ChannelItem[] items) : this()
        {
            this.AddRange(items);
        }


        public int Add(ChannelItem channelItem)
        {
            return this.List.Add(channelItem);
        }


        public void AddRange(ChannelItem[] items)
        {
            foreach (ChannelItem item in items)
                this.Add(item);
        }


        public void Merge(ChannelItemCollection items)
        {
            Exception error = null;
            bool empty = this.Count == 0;

            // Iterate backwards over the channel items in the collection.  If an item exists,
            // but is not present in the merge set, remove it.
            for (int i = this.Count - 1; i >= 0; i--)
            {
                try
                {
                    ChannelItem item = this[i];
                    if (items.Contains(item) == false)
                    {
                        // If an item has already been downloaded (even partial) don't remove it.
                        string filename = Path.Combine(item.Channel.GetDirectory(), item.Enclosure.GetFilename());
                        if (File.Exists(filename) == false)
                            this.RemoveAt(i);
                    }
                }
                catch (Exception ex) 
                { 
                    // Continue merging, but remember the exception.
                    if (error == null)
                        error = ex;                
                }
            }

            // Add all items that are in the merge set, but not in this collection.
            foreach (ChannelItem item in items)
            {
                if (this.List.Contains(item) == false)
                {
                    if (empty == true)
                        this.List.Add(item);
                    else
                        this.List.Insert(0, item);
                }
            }

            if (error != null)
                throw error;
        }


        public int IndexOf(ChannelItem item)
        {
            return this.List.IndexOf(item);
        }

        public bool Contains(ChannelItem item)
        {
            return this.List.Contains(item);
        }


        public ChannelItem this[int index]
        {
            get { return this.List[index] as ChannelItem; }
            set { this.List[index] = value; }
        }


        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors != null && listAccessors.Length > 0)
            {
                PropertyDescriptor listAccessor = listAccessors[listAccessors.Length - 1];
                if (listAccessor.PropertyType.Equals(typeof(ChannelItemCollection)))
                    return TypeDescriptor.GetProperties(typeof(ChannelItem));
            }
            return TypeDescriptor.GetProperties(typeof(ChannelItem));
        }


        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return "Items";
        }		

        #endregion
    }
}
