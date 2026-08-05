using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AdaptySDK.TestSupport
{
    /// <summary>
    /// Renders the full state of a parsed model as deterministic JSON: every field, including
    /// private ones, and every public property, keys sorted. Computed properties are invoked, and a
    /// throwing one is recorded as throwing.
    /// </summary>
    public static class ModelSnapshot
    {
        private const int MaxDepth = 12;

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<
            Type,
            List<KeyValuePair<string, Func<object, object>>>
        > MemberCache = new System.Collections.Concurrent.ConcurrentDictionary<
            Type,
            List<KeyValuePair<string, Func<object, object>>>
        >();

        public static string Render(object value)
        {
            var builder = new StringBuilder();
            Write(builder, value, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
            builder.AppendLine();
            return builder.ToString();
        }

        private static void Write(StringBuilder builder, object value, int depth, HashSet<object> seen)
        {
            if (value is null)
            {
                builder.Append("null");
                return;
            }

            var type = value.GetType();

            if (type.IsEnum)
            {
                builder.Append(Quote(value.ToString() + " (" + Convert.ToInt64(value).ToString(CultureInfo.InvariantCulture) + ")"));
                return;
            }

            switch (value)
            {
                case string text:
                    builder.Append(Quote(text));
                    return;
                case bool flag:
                    builder.Append(flag ? "true" : "false");
                    return;
                case DateTime moment:
                    // Normalised to UTC so the snapshot does not depend on the machine's time
                    // zone, with Kind kept separately because the SDK's Local/Utc semantics matter.
                    builder.Append("{ \"utc\": ")
                        .Append(Quote(moment.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)))
                        .Append(", \"kind\": ")
                        .Append(Quote(moment.Kind.ToString()))
                        .Append(" }");
                    return;
                case Guid guid:
                    builder.Append(Quote(guid.ToString()));
                    return;
                case TimeSpan span:
                    builder.Append(Quote(span.ToString("c", CultureInfo.InvariantCulture)));
                    return;
                case float single:
                    builder.Append(single.ToString("R", CultureInfo.InvariantCulture));
                    return;
                case double number:
                    builder.Append(number.ToString("R", CultureInfo.InvariantCulture));
                    return;
                case decimal dec:
                    builder.Append(dec.ToString(CultureInfo.InvariantCulture));
                    return;
            }

            if (type.IsPrimitive)
            {
                builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                return;
            }

            if (depth >= MaxDepth)
            {
                builder.Append(Quote("<max depth>"));
                return;
            }

            if (!seen.Add(value))
            {
                builder.Append(Quote("<cycle>"));
                return;
            }

            try
            {
                if (value is IDictionary dictionary)
                {
                    WriteDictionary(builder, dictionary, depth, seen);
                    return;
                }

                if (value is IEnumerable sequence)
                {
                    WriteSequence(builder, sequence, depth, seen);
                    return;
                }

                WriteObject(builder, value, type, depth, seen);
            }
            finally
            {
                seen.Remove(value);
            }
        }

        private static void WriteDictionary(StringBuilder builder, IDictionary dictionary, int depth, HashSet<object> seen)
        {
            var keys = dictionary.Keys.Cast<object>()
                .OrderBy(key => Convert.ToString(key, CultureInfo.InvariantCulture), StringComparer.Ordinal)
                .ToList();

            if (keys.Count == 0)
            {
                builder.Append("{}");
                return;
            }

            builder.Append('{');
            for (var i = 0; i < keys.Count; i++)
            {
                NewLine(builder, depth + 1);
                builder.Append(Quote(Convert.ToString(keys[i], CultureInfo.InvariantCulture))).Append(": ");
                Write(builder, dictionary[keys[i]], depth + 1, seen);
                if (i < keys.Count - 1)
                {
                    builder.Append(',');
                }
            }
            NewLine(builder, depth);
            builder.Append('}');
        }

        private static void WriteSequence(StringBuilder builder, IEnumerable sequence, int depth, HashSet<object> seen)
        {
            var items = sequence.Cast<object>().ToList();
            if (items.Count == 0)
            {
                builder.Append("[]");
                return;
            }

            builder.Append('[');
            for (var i = 0; i < items.Count; i++)
            {
                NewLine(builder, depth + 1);
                Write(builder, items[i], depth + 1, seen);
                if (i < items.Count - 1)
                {
                    builder.Append(',');
                }
            }
            NewLine(builder, depth);
            builder.Append(']');
        }

        private static void WriteObject(StringBuilder builder, object value, Type type, int depth, HashSet<object> seen)
        {
            var members = MemberCache.GetOrAdd(type, CollectMembers);
            if (members.Count == 0)
            {
                builder.Append(Quote(value.ToString()));
                return;
            }

            builder.Append('{');
            NewLine(builder, depth + 1);
            builder.Append("\"$type\": ").Append(Quote(type.Name));

            foreach (var member in members)
            {
                builder.Append(',');
                NewLine(builder, depth + 1);
                builder.Append(Quote(member.Key)).Append(": ");

                object memberValue;
                try
                {
                    memberValue = member.Value(value);
                }
                catch (Exception e)
                {
                    var inner = e is TargetInvocationException invocation && invocation.InnerException != null
                        ? invocation.InnerException
                        : e;
                    builder.Append(Quote("<throws " + inner.GetType().Name + ">"));
                    continue;
                }

                Write(builder, memberValue, depth + 1, seen);
            }

            NewLine(builder, depth);
            builder.Append('}');
        }

        private static List<KeyValuePair<string, Func<object, object>>> CollectMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var members = new List<KeyValuePair<string, Func<object, object>>>();

            foreach (var field in type.GetFields(flags))
            {
                if (field.IsStatic || field.Name.Contains("<"))
                {
                    continue;
                }

                var captured = field;
                members.Add(new KeyValuePair<string, Func<object, object>>(
                    captured.Name,
                    instance => captured.GetValue(instance)
                ));
            }

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanRead || property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                var captured = property;
                members.Add(new KeyValuePair<string, Func<object, object>>(
                    captured.Name + " (property)",
                    instance => captured.GetValue(instance)
                ));
            }

            return members.OrderBy(member => member.Key, StringComparer.Ordinal).ToList();
        }

        private static void NewLine(StringBuilder builder, int depth)
        {
            builder.Append('\n').Append(new string(' ', depth * 2));
        }

        private static string Quote(string text)
        {
            if (text is null)
            {
                return "null";
            }

            var builder = new StringBuilder(text.Length + 2);
            builder.Append('"');
            foreach (var character in text)
            {
                switch (character)
                {
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        builder.Append(character);
                        break;
                }
            }
            builder.Append('"');
            return builder.ToString();
        }

        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            internal static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

            public new bool Equals(object x, object y) => ReferenceEquals(x, y);

            public int GetHashCode(object obj) =>
                System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
