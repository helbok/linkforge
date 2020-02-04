namespace LinkForge.Core;

public interface IIdEncoder
{
    string Encode(long id);

    long Decode(string code);
}
