using System;
using System.Collections;
using System.Collections.Generic;

namespace MustardBlack.Handlers.Binding.Binders
{
	sealed class ExpandableList<T> : IList<T>, IList
	{
		readonly List<T> list;

		public ExpandableList()
		{
			this.list = new List<T>();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(T item)
		{
			this.list.Add(item);
		}

		public int Add(object value)
		{
			throw new NotImplementedException();
		}

		public bool Contains(object value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public int IndexOf(object value)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public void Remove(object value)
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			return this.list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			return this.list.Remove(item);
		}

		public void CopyTo(Array array, int index)
		{
			(this.list as IList).CopyTo(array, index);
		}

		public int Count
		{
			get { return this.list.Count; }
		}

		public object SyncRoot
		{
			get { return (this.list as IList).SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return (this.list as IList).IsSynchronized; }
		}

		public bool IsReadOnly
		{
			get { return (this.list as IList).IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return (this.list as IList).IsFixedSize; }
		}

		public int IndexOf(T item)
		{
			return this.list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.list.RemoveAt(index);
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T)value; }
		}

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
			set {
				if (index > this.list.Count - 1)
				{
					for (var i = this.list.Count; i <= index; i++)
						this.list.Add(default(T));

					this.list[index] = value;
				}
				else
					this.list[index] = value;
			}
		}
	}
}