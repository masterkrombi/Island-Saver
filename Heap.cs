using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Template heap class
public class Heap<T> where T : IHeapItem<T> {

	T[] items;					// Generic list of items
	int currentItemCount;		// How many items

	// Heap constructor
	public Heap(int maxHeapSize)
	{
		items = new T[maxHeapSize];
	}

	// Function to add item to heap and sort it
	public void Add(T item)
	{
		item.HeapIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp (item);
		currentItemCount++;
	}

	// Function to update heap with new item
	public void UpdateItem(T item)
	{
		SortUp (item);
	}

	// Getter for items in heap
	public int Count
	{
		get
		{
			return currentItemCount;
		}
	}

	// Function for interface comparable<T>
	public bool Contains(T item)
	{
		return Equals(items[item.HeapIndex], item);
	}

	// Function to remove the first item in heap and update the rest of the heap
	public T RemoveFirst()
	{
		T firstItem = items [0];
		currentItemCount--;
		items [0] = items [currentItemCount];
		items [0].HeapIndex = 0;
		SortDown (items [0]);
		return firstItem;
	}

	// Sorts heap going from an item down the tree
	void SortDown(T item)
	{
		while (true)
		{
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if(childIndexLeft < currentItemCount)
			{
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount)
				{
					if (items [childIndexLeft].CompareTo (items [childIndexRight]) < 0)
					{
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo (items [swapIndex]) < 0)
				{
					Swap (item, items [swapIndex]);
				}
				else
				{
					return;
				}
			}
			else
			{
				return;
			}
		}
	}

	// Sorts heap going from an item up the tree
	void SortUp(T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;

		while (true)
		{
			T parentItem = items [parentIndex];
			if (item.CompareTo (parentItem) > 0)
			{
				Swap (item, parentItem);
			}
			else
			{
				break;
			}
			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	// Swap function to swap item A and B
	void Swap(T itemA, T itemB)
	{
		items [itemA.HeapIndex] = itemB;
		items [itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}

//Interface for heap class
public interface IHeapItem<T> : IComparable<T>
{
	int HeapIndex
	{
		get;
		set;
	}
}