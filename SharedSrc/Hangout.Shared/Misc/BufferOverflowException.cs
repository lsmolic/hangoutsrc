using System;

public class BufferOverflowException : Exception
{
    public BufferOverflowException() : base() { }
    public BufferOverflowException(string s) : base(s) { }
    public BufferOverflowException(string s, Exception ex) : base(s, ex) { }
}
