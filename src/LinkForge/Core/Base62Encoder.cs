using System;
using System.Collections.Generic;

namespace LinkForge.Core;

public sealed class Base62Encoder
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

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

        var stack = new Stack<char>();

        while (id > 0)
        {
            stack.Push(Alphabet[(int)(id % 62)]);
            id /= 62;
        }

        return new string(stack.ToArray());
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

            result = result * 62 + index;
        }

        return result;
    }
}
