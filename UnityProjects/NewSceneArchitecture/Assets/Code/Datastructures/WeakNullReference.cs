/**  --------------------------------------------------------  *
 *   WeakNullReference.cs  
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
 * Provides a weak reference to a null target object, which, unlike
 * other weak references, is always considered to be alive. This
 * facilitates handling null dictionary values, which are perfectly
 * legal.
 */
public class WeakNullReference<T> : WeakReference<T> where T : class {
    public static readonly WeakNullReference<T> Singleton = new WeakNullReference<T>();
    
    private WeakNullReference() : base(null) { }
    
    public override bool IsAlive {
        get { return true; }
    }
}