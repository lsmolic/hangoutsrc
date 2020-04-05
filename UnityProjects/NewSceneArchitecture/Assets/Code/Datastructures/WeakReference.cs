/**  --------------------------------------------------------  *
 *   WeakReference.cs  
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
 * Adds strong typing to WeakReference.Target using generics. Also,
 * the Create factory method is used in place of a constructor
 * to handle the case where target is null, but we want the
 * reference to still appear to be alive.
 */
public class WeakReference<T> : WeakReference where T : class {
    public static WeakReference<T> Create(T target) {
        if (target == null) {
			return WeakNullReference<T>.Singleton;
		}
        return new WeakReference<T>(target);
    }

    protected WeakReference(T target)
        : base(target, false) { }

    public new T Target {
        get { return (T)base.Target; }
    }
}