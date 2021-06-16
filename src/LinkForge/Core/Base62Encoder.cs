using System;

namespace LinkForge.Core;

public sealed class Base62Encoder : IIdEncoder
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // ceil(log_62(long.MaxValue)) == 11, the longest possible encoded code.
    private const int MaxEncodedLength = 11;

    public string Encode(long id)
    {
        if (id < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Identifier must be non-negative.");
        }

        if (id == 0)
        {
            return Alphabet[0].ToString();
        }

        Span<char> buffer = stackalloc char[MaxEncodedLength];
        int position = buffer.Length;

        while (id > 0)
        {
            buffer[--position] = Alphabet[(int)(id % 62)];
            id /= 62;
        }

        return buffer.Slice(position).ToString();
    }

    public long Decode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Code must not be empty.", nameof(code));
        }

        long result = 0;

        foreach (char c in code)
        {
            int index = Alphabet.IndexOf(c);

            if (index < 0)
            {
                throw new FormatException($"'{c}' is not a valid base-62 character.");
            }

            result = checked(result * 62 + index);
        }

        return result;
    }
}
