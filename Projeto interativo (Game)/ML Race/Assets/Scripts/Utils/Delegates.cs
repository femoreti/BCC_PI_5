using UnityEngine;
using System.Collections;

public static class Delegates {

	public delegate void Simple();
	public delegate void OneParam(object obj);
    public delegate void WithParams(params object[] obj);
}