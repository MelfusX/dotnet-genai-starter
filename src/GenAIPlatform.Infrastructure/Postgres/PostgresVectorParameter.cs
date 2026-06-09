using Pgvector;

namespace GenAIPlatform.Infrastructure.Postgres;

internal static class PostgresVectorParameter
{
    public static Vector From(IReadOnlyList<float> vector)
    {
        ArgumentNullException.ThrowIfNull(vector);

        var values = vector.ToArray();
        foreach (var value in values)
        {
            if (!float.IsFinite(value))
            {
                throw new ArgumentException("Embedding vectors must contain only finite values.", nameof(vector));
            }
        }

        return new Vector(values);
    }
}
