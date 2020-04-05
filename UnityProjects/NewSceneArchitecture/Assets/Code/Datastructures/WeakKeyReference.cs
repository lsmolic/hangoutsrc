/**  --------------------------------------------------------  *
 *   WeakKeyReference.cs  
 *
 *   Author: Nick Guerrera, http://blogs.msdn.com/nicholg/archive/2006/06/04/617466.aspx
 *   Date: 06/10/2009
 *	 
 *   --------------------------------------------------------  *
 */


using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * Provides a weak reference to an object of the given type to be used in
 * a WeakDictionary along with the given comparer.
 */
public sealed class WeakKeyReference<T> : WeakReference<T> where T : class {
    public readonly int HashCode;
    
    public WeakKeyReference(T key, WeakKeyComparer<T> comparer)
        : base(key) {
        // retain the object's hash code immediately so that even
        // if the target is GC'ed we will be able to find and
        // remove the dead weak reference.
        this.HashCode = comparer.GetHashCode(key);
    }
}
