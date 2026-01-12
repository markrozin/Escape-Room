//Author: Mark Rozin
//File Name: MyStack.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Implements a generic stack using a list to manage elements, supporting standard stack operations like push, pop, and peek.

using System.Collections.Generic;
using System;

namespace TestProject
{
    public class MyStack<T>
    {
        // Define the list to hold the stack elements
        private List<T> items;

        // Initializes the stack as an empty list
        public MyStack()
        {
            items = new List<T>();
        }

        // Adds an item to the top of the stack
        public void Push(T item)
        {
            items.Add(item);
        }

        // Removes and returns the item at the top of the stack
        public T Pop()
        {
            if (items.Count == 0) throw new InvalidOperationException("Stack is empty.");
            var item = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return item;
        }

        // Returns the item at the top of the stack without removing it
        public T Peek()
        {
            if (items.Count == 0) throw new InvalidOperationException("Stack is empty.");
            return items[items.Count - 1];
        }

        // Property to get the number of elements in the stack
        public int count => items.Count;
    }
}
