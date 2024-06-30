using Synergy.Core.Collections.Observable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Misc.FilteringCollection
{
	public class FilteringCollection<T> : ICollection<T>, IList<T>
		where T : INotifyPropertyChanged
	{
		//private class ItemContainer : INotifyPropertyChanged, IDisposable
		//{
		//	public event PropertyChangedEventHandler? PropertyChanged;

		//	public T Item { get; }
		//	public bool IsPresent { get; set; }

  //          public ItemContainer(T item)
  //          {
		//		Item = item;
		//		item.PropertyChanged += Handler;
  //          }

		//	public void Dispose()
		//	{
		//		Item.PropertyChanged -= Handler;
		//	}

		//	private void Handler(object? sender, PropertyChangedEventArgs e)
		//	{
		//		PropertyChanged?.Invoke(sender, e);
		//	}
		//}

		private readonly FullyObservableCollection<T> _items = [];
		private readonly FullyObservableCollection<T> _filtered = [];

		private ICollectionFilter<T>? _filter;

		public FullyObservableCollection<T> Filtered => _filtered;

		public int Count => _items.Count;
		public bool IsReadOnly => false;

		public T this[int index] 
		{ 
			get => _items[index];

			set
			{
				_filtered.Remove(value);
				_items[index] = value;
				if(ApplyFilter(value))
					Filter(_filter);
			}
		}

		public FilteringCollection()
        {
			_filtered.ItemPropertyChanged += Filtered_ItemPropertyChanged;
			_items.ItemPropertyChanged += Items_ItemPropertyChanged;
        }

		#region Implementations

		#region ICollection

		public void Add(T item)
		{
			_items.Add(item);

			if(ApplyFilter(item))
				_filtered.Add(item);
		}

		public void Clear()
		{
			_items.Clear();
			_filtered.Clear();
			_filter = null;
		}

		public bool Contains(T item)
		{
			return _items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _filtered.GetEnumerator();
		}

		public bool Remove(T item)
		{
			if (_items.Remove(item))
			{
				_filtered.Remove(item);

				return true;
			}

			return false;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IList

		public int IndexOf(T item)
		{
			return _items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_items.Insert(index, item);

			Filter(_filter);
		}

		public void RemoveAt(int index)
		{
			var item = _items[index];

			_items.RemoveAt(index);
			_filtered.Remove(item);
		}

		#endregion

		#endregion

		public void ClearFilter()
		{
			Filter(null);
		}

		public void Filter(ICollectionFilter<T>? filter)
		{
			_filter = filter;

			_filtered.Clear();

			foreach(var item in _items.Where(ApplyFilter))
			{
				_filtered.Add(item);
			}
		}

		private bool ApplyFilter(T item)
		{
			if (_filter is null)
				return true;

			return _filter.Filter(item);
		}

		private void Filtered_ItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs e)
		{
			var item = _filtered[e.CollectionIndex];

			if(!ApplyFilter(item))
				_filtered.Remove(item);
		}

		private void Items_ItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs e)
		{
			var item = _items[e.CollectionIndex];

			if (_filter != null && ApplyFilter(item))
			{
				if (!_filtered.Contains(item))
				{
					_filtered.Add(item);
				}
			}
		}
	}
}
