using System;
using System.Collections.Generic;
using System.Text;

public class OscPacketBuilder
{
    private List<byte> _data = new List<byte>();
    private string _typeTag = ",";

    public void Reset()
    {
        _data.Clear();
        _typeTag = ","; // ⚠ 重置类型标签
    }

    public void SetAddress(string address)
    {
        AddPaddedString(address);
    }

    public void AddString(string value)
    {
        _typeTag += "s";
        AddPaddedString(value);
    }

    public void AddInt(int value)
    {
        _typeTag += "i";
        AddBigEndian(BitConverter.GetBytes(value));
    }

    public void AddFloat(float value)
    {
        _typeTag += "f";
        AddBigEndian(BitConverter.GetBytes(value));
    }

    public void AddBool(bool value)
    {
        _typeTag += value ? "T" : "F";
        // ⚠ 不添加任何数据
    }

    public byte[] Build()
    {
        var final = new List<byte>();

        // 地址段 + 对齐
        int addressLength = GetPaddedLength(_data);
        final.AddRange(_data.GetRange(0, addressLength));

        // 类型标签段
        var typeBytes = GetPaddedString(_typeTag);
        final.AddRange(typeBytes);

        // 数据段
        final.AddRange(_data.GetRange(addressLength, _data.Count - addressLength));

        return final.ToArray();
    }

    private void AddPaddedString(string value)
    {
        _data.AddRange(GetPaddedString(value));
    }

    private byte[] GetPaddedString(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        int pad = 4 - ((bytes.Length + 1) % 4);
        if (pad == 4) pad = 0;

        var result = new List<byte>(bytes);
        result.Add(0);
        for (int i = 0; i < pad; i++)
            result.Add(0);

        return result.ToArray();
    }

    private void AddBigEndian(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        _data.AddRange(bytes);
    }

    private int GetPaddedLength(List<byte> list)
    {
        int index = 0;
        while (index < list.Count && list[index] != 0) index++;
        index++;
        while (index % 4 != 0) index++;
        return index;
    }
}