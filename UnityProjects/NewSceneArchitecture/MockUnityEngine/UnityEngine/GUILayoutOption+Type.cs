using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace UnityEngine {
public enum Type {
	fixedWidth,
	fixedHeight,
	minWidth,
	maxWidth,
	minHeight,
	maxHeight,
	stretchWidth,
	stretchHeight,
	alignStart,
	alignMiddle,
	alignEnd,
	alignJustify,
	equalSize,
	spacing
}
}
