using LinkForge.Core;
using Xunit;

namespace LinkForge.Tests;

public class Base62EncoderTests
{
    private readonly Base62Encoder _encoder = new Base62Encoder();

    [Theory]
    [InlineData(1, "1")]
    [InlineData(10, "a")]
    [InlineData(35, "z")]
    [InlineData(36, "A")]
    [InlineData(61, "Z")]
    [InlineData(62, "10")]
    [InlineData(3844, "100")]
    public void Encode_returns_expected_value(long id, string expected)
    {
        Assert.Equal(expected, _encoder.Encode(id));
    }

    [Fact]
    public void Decode_reverses_encode_for_a_range_of_ids()
    {
        for (long id = 1; id < 10_000; id++)
        {
            Assert.Equal(id, _encoder.Decode(_encoder.Encode(id)));
        }
    }
}
