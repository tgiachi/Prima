using System.Text;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Persistence.Entities;

namespace Prima.UOData.Extensions;

/// <summary>
/// Provides extension methods for binary serialization of common types.
/// </summary>
public static class BinarySerializationExtensions
{
    /// <summary>
    /// Serializes a string to a BinaryWriter.
    /// </summary>
    /// <param name="writer">The BinaryWriter to write to.</param>
    /// <param name="value">The string to serialize.</param>
    public static void WriteString(this BinaryWriter writer, string? value)
    {
        if (value == null)
        {
            writer.Write((byte)0);
            return;
        }

        var bytes = Encoding.UTF8.GetBytes(value);
        writer.Write((ushort)bytes.Length);
        writer.Write(bytes);
    }

    /// <summary>
    /// Deserializes a string from a BinaryReader.
    /// </summary>
    /// <param name="reader">The BinaryReader to read from.</param>
    /// <returns>The deserialized string.</returns>
    public static string ReadString(this BinaryReader reader)
    {
        var length = reader.ReadUInt16();
        if (length == 0)
            return string.Empty;

        var bytes = reader.ReadBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Serializes a Serial to a BinaryWriter.
    /// </summary>
    /// <param name="writer">The BinaryWriter to write to.</param>
    /// <param name="serial">The Serial to serialize.</param>
    public static void WriteSerial(this BinaryWriter writer, Serial serial)
    {
        writer.Write(serial.Value);
    }

    /// <summary>
    /// Deserializes a Serial from a BinaryReader.
    /// </summary>
    /// <param name="reader">The BinaryReader to read from.</param>
    /// <returns>The deserialized Serial.</returns>
    public static Serial ReadSerial(this BinaryReader reader)
    {
        return new Serial(reader.ReadUInt32());
    }

    /// <summary>
    /// Serializes a generic list to a BinaryWriter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="writer">The BinaryWriter to write to.</param>
    /// <param name="list">The list to serialize.</param>
    /// <param name="itemSerializer">The delegate to serialize each item.</param>
    public static void WriteList<T>(this BinaryWriter writer, IList<T>? list, Action<BinaryWriter, T> itemSerializer)
    {
        if (list == null || list.Count == 0)
        {
            writer.Write((int)0);
            return;
        }

        writer.Write(list.Count);
        foreach (var item in list)
        {
            itemSerializer(writer, item);
        }
    }

    /// <summary>
    /// Deserializes a generic list from a BinaryReader.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="reader">The BinaryReader to read from.</param>
    /// <param name="itemDeserializer">The delegate to deserialize each item.</param>
    /// <returns>The deserialized list.</returns>
    public static List<T> ReadList<T>(this BinaryReader reader, Func<BinaryReader, T> itemDeserializer)
    {
        var count = reader.ReadInt32();
        if (count == 0)
            return new List<T>();

        var list = new List<T>(count);
        for (int i = 0; i < count; i++)
        {
            list.Add(itemDeserializer(reader));
        }
        return list;
    }

    /// <summary>
    /// Serializes a generic dictionary to a BinaryWriter.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="writer">The BinaryWriter to write to.</param>
    /// <param name="dictionary">The dictionary to serialize.</param>
    /// <param name="keySerializer">The delegate to serialize each key.</param>
    /// <param name="valueSerializer">The delegate to serialize each value.</param>
    public static void WriteDictionary<TKey, TValue>(
        this BinaryWriter writer,
        IDictionary<TKey, TValue>? dictionary,
        Action<BinaryWriter, TKey> keySerializer,
        Action<BinaryWriter, TValue> valueSerializer) where TKey : notnull
    {
        if (dictionary == null || dictionary.Count == 0)
        {
            writer.Write((int)0);
            return;
        }

        writer.Write(dictionary.Count);
        foreach (var kvp in dictionary)
        {
            keySerializer(writer, kvp.Key);
            valueSerializer(writer, kvp.Value);
        }
    }

    /// <summary>
    /// Deserializes a generic dictionary from a BinaryReader.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="reader">The BinaryReader to read from.</param>
    /// <param name="keyDeserializer">The delegate to deserialize each key.</param>
    /// <param name="valueDeserializer">The delegate to deserialize each value.</param>
    /// <returns>The deserialized dictionary.</returns>
    public static Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>(
        this BinaryReader reader,
        Func<BinaryReader, TKey> keyDeserializer,
        Func<BinaryReader, TValue> valueDeserializer) where TKey : notnull
    {
        var count = reader.ReadInt32();
        if (count == 0)
            return new Dictionary<TKey, TValue>();

        var dictionary = new Dictionary<TKey, TValue>(count);
        for (int i = 0; i < count; i++)
        {
            var key = keyDeserializer(reader);
            var value = valueDeserializer(reader);
            dictionary[key] = value;
        }
        return dictionary;
    }

    /// <summary>
    /// Serializes an ISerializableEntity to a BinaryWriter.
    /// </summary>
    /// <param name="writer">The BinaryWriter to write to.</param>
    /// <param name="entity">The entity to serialize.</param>
    public static void WriteEntity(this BinaryWriter writer, ISerializableEntity entity)
    {
        writer.WriteSerial(entity.Id);
    }

    /// <summary>
    /// Serializes a byte array to a BinaryWriter.
    /// </summary>
    /// <param name="writer">The BinaryWriter to write to.</param>
    /// <param name="data">The data to serialize.</param>
    public static void WriteByteArray(this BinaryWriter writer, byte[]? data)
    {
        if (data == null || data.Length == 0)
        {
            writer.Write((int)0);
            return;
        }

        writer.Write(data.Length);
        writer.Write(data);
    }

    /// <summary>
    /// Deserializes a byte array from a BinaryReader.
    /// </summary>
    /// <param name="reader">The BinaryReader to read from.</param>
    /// <returns>The deserialized byte array.</returns>
    public static byte[] ReadByteArray(this BinaryReader reader)
    {
        var length = reader.ReadInt32();
        if (length == 0)
            return Array.Empty<byte>();

        return reader.ReadBytes(length);
    }

    /// <summary>
    /// Serializes an enum type to a BinaryWriter.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="writer">The BinaryWriter to write to.</param>
    /// <param name="value">The enum value to serialize.</param>
    public static void WriteEnum<TEnum>(this BinaryWriter writer, TEnum value) where TEnum : struct, Enum
    {
        writer.Write(Convert.ToInt32(value));
    }

    /// <summary>
    /// Deserializes an enum type from a BinaryReader.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="reader">The BinaryReader to read from.</param>
    /// <returns>The deserialized enum value.</returns>
    public static TEnum ReadEnum<TEnum>(this BinaryReader reader) where TEnum : struct, Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), reader.ReadInt32());
    }
}
