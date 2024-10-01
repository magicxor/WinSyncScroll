namespace ShowWndProcMessages;

public static class WinApiUtils
{
    public static (short Low, short High) GetHiLoWords(uint value)
    {
        var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
        var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);
        return (Low: low, High: high);
    }
}
