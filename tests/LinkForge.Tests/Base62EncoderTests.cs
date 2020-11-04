using System;
using LinkForge.Core;
using Xunit;

namespace LinkForge.Tests;

public class Base62EncoderTests
{
    private readonly Base62Encoder _encoder = new Base62Encoder();

    [Theory]
    [InlineData(0, "0")]
    [InlineData(1, "1")]
    [InlineData(9, "9")]
    [InlineData(10, "a")]
    [InlineData(35, "z")]
    [InlineData(36, "A")]
    [InlineData(61, "Z")]
    [InlineData(62, "10")]
    [InlineData(3844, "100")]
    [InlineData(238328, "1000")]
    public void Encode_returns_expected_value(long id, string expected)
    {
        Assert.Equal(expected, _encoder.Encode(id));
    }

    [Fact]
    public void Decode_reverses_encode_for_a_range_of_ids()
    {
        for (long id = 0; id < 10_000; id++)
        {
            Assert.Equal(id, _encoder.Decode(_encoder.Encode(id)));
        }
    }

    [Fact]
    public void Encode_and_decode_round_trips_large_ids()
    {
        long[] ids =
        {
            62L * 62L * 62L - 1,
            62L * 62L * 62L * 62L * 62L - 1,
            long.MaxValue
        };

        foreach (long id in ids)
        {
            Assert.Equal(id, _encoder.Decode(_encoder.Encode(id)));
        }
    }

    [Fact]
    public void Encode_rejects_negative_ids()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _encoder.Encode(-1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Decode_rejects_blank_codes(string? code)
    {
        Assert.Throws<ArgumentException>(() => _encoder.Decode(code!));
    }

    [Fact]
    public void Decode_rejects_invalid_characters()
    {
        Assert.Throws<FormatException>(() => _encoder.Decode("not_a_code$"));
    }
}
